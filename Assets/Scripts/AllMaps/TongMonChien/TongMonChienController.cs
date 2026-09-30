using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDHModules;
using AllMaps.ItemProvider;
using LuyenDan;
using Network;
using Network.ClientState;
using Network.Models;
using UnityEngine;
using WarState = Network.ClientState.TongMonChien;

namespace AllMaps.TongMonChien
{
    /// <summary>
    /// Điều khiển scene Tông môn chiến. Không tự tính owner/vị trí: mọi thứ lấy từ snapshot + delta của server;
    /// client chỉ nội suy chuyển động trên cạnh và ngoại suy HP hồi theo thời gian để hiển thị.
    /// Đặt component này trong TongMonChien.unity cùng MapManager (bật IsWarMode).
    /// </summary>
    public class TongMonChienController : MonoBehaviour, ITongMonChienCallback
    {
        [SerializeField] private MapManager mapManager;
        [Tooltip("Prefab hiển thị rối (tuỳ chọn). Bỏ trống thì dùng prefab người chơi của MapManager.")]
        [SerializeField] private GameObject puppetPrefab;
        [Tooltip("Prefab icon giao tranh (tuỳ chọn). Bỏ trống thì dùng vòng đỏ nhấp nháy.")]
        [SerializeField] private GameObject fightingIconPrefab;
        [SerializeField] private Vector3 fightingIconOffset = new Vector3(0f, 0.7f, 0f);
        [SerializeField] private float ownerMarkerScale = 0.9f;
        [SerializeField] private int markerSortingOrder = 40;
        [Tooltip("Prefab HUD (Assets/Scripts/AllMaps/TongMonChien/Prefabs/TongMonChienHud.prefab).")]
        [SerializeField] private TongMonChienHud hudPrefab;
        public const string HudPrefabPath = "Assets/Scripts/AllMaps/TongMonChien/Prefabs/TongMonChienHud.prefab";

        private WarState state;
        private TongMonChienHud hud;
        private long version = -1;
        private bool snapshotLoaded;
        private bool waitingSnapshot;
        private TimeSpan serverOffset;
        private float secondsPerEdge = 5f;
        private int myCharId;
        private int myClanId;
        private bool isLeader;
        private int maxPuppetCount;
        private int maxPuppetLevel;
        private int deployingPuppetId;
        private PuppetDanTarget puppetDanTarget;
        private int strongholdMoneyVip;
        private int nodeMoneyVip;
        private DateTime nextSettlementAt;
        private int snapshotRetries;
        private const int MaxSnapshotRetries = 3;
        private GameObject hiddenNpcPanel;
        private UIHeadPanel headPanel;

        private readonly Dictionary<int, WarClanView> clans = new Dictionary<int, WarClanView>();
        private readonly Dictionary<int, WarNodeView> nodes = new Dictionary<int, WarNodeView>();
        private readonly Dictionary<(WarUnitKind, int), WarUnitView> units = new Dictionary<(WarUnitKind, int), WarUnitView>();
        private readonly Dictionary<int, Color> clanColors = new Dictionary<int, Color>();
        private readonly Dictionary<int, SpriteRenderer> ownerMarkers = new Dictionary<int, SpriteRenderer>();
        private readonly Dictionary<int, GameObject> fightingIcons = new Dictionary<int, GameObject>();
        private readonly Dictionary<(WarUnitKind, int), Player> unitMarkers = new Dictionary<(WarUnitKind, int), Player>();
        /// <summary>Ngoại hình đã áp cho từng marker; đổi thì dựng lại visual.</summary>
        private readonly Dictionary<(WarUnitKind, int), string> visualSignatures = new Dictionary<(WarUnitKind, int), string>();
        private Transform overlayRoot;
        private static Sprite circleSprite;

        public DateTime ServerNow => DateTime.UtcNow + serverOffset;
        public bool IsLeader => isLeader;
        public int MaxPuppetCount => maxPuppetCount;
        public int MaxPuppetLevel => maxPuppetLevel;
        public int DeployingPuppetId => deployingPuppetId;
        public int StrongholdMoneyVip => strongholdMoneyVip;
        public int NodeMoneyVip => nodeMoneyVip;
        public WarClanView MyClan => GetClan(myClanId);

        /// <summary>Mốc chốt thưởng tuần kế tiếp (UTC, giờ server); đã qua mốc thì tự lùi sang tuần sau.</summary>
        public DateTime NextSettlementAt
        {
            get
            {
                if (nextSettlementAt == default) return default;
                var now = ServerNow;
                while (nextSettlementAt <= now) nextSettlementAt = nextSettlementAt.AddDays(7);
                return nextSettlementAt;
            }
        }

        public WarUnitView Me => units.TryGetValue((WarUnitKind.Player, myCharId), out var u) ? u : null;

        public IEnumerable<WarUnitView> MyPuppets =>
            units.Values.Where(x => x.Kind == WarUnitKind.Puppet && x.ClanId == myClanId).OrderBy(x => x.Id);

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Tự gán prefab HUD mặc định để khỏi phải kéo tay.
            if (hudPrefab == null)
                hudPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<TongMonChienHud>(HudPrefabPath);
        }
#endif

        private void Start()
        {
            if (mapManager == null) mapManager = Pool.Get<MapManager>();
            overlayRoot = new GameObject("TongMonChienOverlay").transform;
            if (mapManager != null) mapManager.WarNodeClicked += OnNodeClicked;
            if (hudPrefab == null)
            {
                Debug.LogError("TongMonChienController chưa gán HUD prefab. Chạy menu Tools → Tông môn chiến → Tạo prefab HUD.", this);
                enabled = false;
                return;
            }

            hud = Instantiate(hudPrefab);
            hud.Bind(this);
            HideNpcPanel();
            headPanel = Pool.Get<UIHeadPanel>();
            if (headPanel != null) headPanel.SetHpOverride(GetHeadHp);
            state = new WarState(this);
            Client.Instance.ChangeState(state);
            waitingSnapshot = true;
        }

        private void OnDestroy()
        {
            ClearDanTarget();
            if (headPanel != null) headPanel.SetHpOverride(null);
            // Trả lại bảng NPC góc trái cho các scene khác.
            if (hiddenNpcPanel != null) hiddenNpcPanel.SetActive(true);
            if (mapManager != null) mapManager.WarNodeClicked -= OnNodeClicked;
            if (overlayRoot != null) Destroy(overlayRoot.gameObject);
            foreach (var marker in unitMarkers.Values)
            {
                if (marker == null || (mapManager != null && marker == mapManager.MainPlayer)) continue;
                Destroy(marker.transform.root.gameObject);
            }
        }

        // =====================================================================
        // Lệnh người chơi
        // =====================================================================

        private void OnNodeClicked(Map map)
        {
            if (map == null || !snapshotLoaded) return;
            if (deployingPuppetId > 0)
            {
                var puppetId = deployingPuppetId;
                deployingPuppetId = 0;
                hud.Refresh();
                state.DeployPuppet(puppetId, map.Id);
                return;
            }

            var me = Me;
            if (me == null) return;
            if (me.State == WarUnitState.Moving)
            {
                hud.ShowMessage("Đang di chuyển, không thể đổi lộ trình.");
                return;
            }

            if (me.NodeId == map.Id) return;
            state.Move(map.Id);
        }

        public void BeginDeployPuppet(int puppetId)
        {
            deployingPuppetId = puppetId;
            hud.ShowMessage("Chọn node của tông để điều rối tới.");
            hud.Refresh();
        }

        public void CancelDeploy()
        {
            deployingPuppetId = 0;
            hud.Refresh();
        }

        public void CreatePuppet() => state?.CreatePuppet();
        public void UpgradePuppet(int puppetId) => state?.UpgradePuppet(puppetId);

        /// <summary>
        /// Mở túi đồ sẵn có để chọn đan cho bản thân. Trong scene chiến, server tự đổi đan Hp/HoiFullHp
        /// sang hồi HP chiến (không đụng HP thường); đan loại khác dùng như bình thường.
        /// </summary>
        public void OpenInventoryForSelf()
        {
            ClearDanTarget();
            OpenInventory();
        }

        /// <summary>Mở túi đồ để Tộc Chủ chọn đan hồi khí huyết cho Rối.</summary>
        public void OpenInventoryForPuppet(int puppetId)
        {
            ClearDanTarget();
            puppetDanTarget = new PuppetDanTarget(this, puppetId);
            DanUseRouter.Set(puppetDanTarget);
            hud.ShowMessage($"Chọn đan hồi khí huyết cho Rối #{puppetId}");
            OpenInventory();
        }

        public void SendPuppetHpPill(int puppetId, ushort danId, ushort count)
        {
            state?.UseHpPill(puppetId, danId, count);
        }

        private static void OpenInventory()
        {
            Pool.Get<TabPanel>()?.Open(TabPanel.ETypePanel.VatPham);
        }

        private void ClearDanTarget()
        {
            if (puppetDanTarget == null) return;
            DanUseRouter.Clear(puppetDanTarget);
            puppetDanTarget = null;
        }

        /// <summary>Ẩn bảng NPC góc trái (UINPCJiaoHu) trong scene chiến; hiện lại khi rời scene.</summary>
        private void HideNpcPanel()
        {
            var npcPanel = Pool.Get<UINPCJiaoHu>();
            if (npcPanel == null || !npcPanel.gameObject.activeSelf) return;
            hiddenNpcPanel = npcPanel.gameObject;
            hiddenNpcPanel.SetActive(false);
        }

        /// <summary>HP chiến hiển thị trên UIHead thay cho HP thường, kèm tốc độ hồi.</summary>
        private (int Hp, int HpMax, string Suffix)? GetHeadHp()
        {
            var me = Me;
            if (me == null) return null;
            var suffix = me.RegenPerMinute > 0 ? $" (+{me.RegenPerMinute:0.#}/phút)" : string.Empty;
            return (me.GetHp(ServerNow), me.HpMax, suffix);
        }

        public void Exit()
        {
            Observer.Notify(EventList.GamePlay.BACK_SCENE);
        }

        // =====================================================================
        // Callback từ server
        // =====================================================================

        public void OnWarSnapshot(WarSnapshot snapshot)
        {
            waitingSnapshot = false;
            if (snapshot == null || !snapshot.Success)
            {
                if (snapshotLoaded) return;
                // Vừa từ trận đấu quay lại, server có thể chưa kịp chuyển state: thử lại vài lần trước khi thoát.
                if (snapshotRetries < MaxSnapshotRetries)
                {
                    snapshotRetries++;
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        if (this != null && !snapshotLoaded) RequestSnapshot();
                    }).SetLink(gameObject);
                    return;
                }

                NotifyGroup.Instance?.Show(snapshot?.Message ?? "Không thể vào Tông môn chiến");
                Exit();
                return;
            }

            snapshotRetries = 0;
            strongholdMoneyVip = snapshot.StrongholdMoneyVip;
            nodeMoneyVip = snapshot.NodeMoneyVip;
            nextSettlementAt = snapshot.NextSettlementAt;

            serverOffset = snapshot.ServerTime - DateTime.UtcNow;
            secondsPerEdge = snapshot.SecondsPerEdge > 0 ? snapshot.SecondsPerEdge : 5f;
            myCharId = snapshot.MyCharId;
            myClanId = snapshot.MyClanId;
            isLeader = snapshot.IsLeader;
            maxPuppetCount = snapshot.MaxPuppetCount;
            maxPuppetLevel = snapshot.MaxPuppetLevel;
            version = snapshot.Version;
            clans.Clear();
            nodes.Clear();
            var oldUnits = units.Keys.ToList();
            units.Clear();
            foreach (var c in snapshot.Clans) clans[c.ClanId] = c;
            foreach (var n in snapshot.Nodes) nodes[n.NodeId] = n;
            foreach (var u in snapshot.Units) units[(u.Kind, u.Id)] = u;
            foreach (var key in oldUnits.Where(k => !units.ContainsKey(k)).ToList()) RemoveUnitMarker(key);
            snapshotLoaded = true;
            foreach (var clan in clans.Values) LoadClanColor(clan);
            RefreshAllNodes();
            foreach (var u in units.Values) EnsureUnitMarker(u);
            hud.Refresh();
        }

        public void OnWarDelta(WarDelta delta)
        {
            if (!snapshotLoaded || delta == null) return;
            if (delta.Version <= version) return;
            if (delta.Version != version + 1)
            {
                // Thiếu delta: tải lại snapshot đầy đủ.
                RequestSnapshot();
                return;
            }

            version = delta.Version;
            serverOffset = delta.ServerTime - DateTime.UtcNow;
            foreach (var c in delta.Clans)
            {
                clans[c.ClanId] = c;
                LoadClanColor(c);
            }

            foreach (var n in delta.Nodes)
            {
                nodes[n.NodeId] = n;
                RefreshNode(n.NodeId);
            }

            foreach (var r in delta.RemovedUnits)
            {
                units.Remove((r.Kind, r.Id));
                RemoveUnitMarker((r.Kind, r.Id));
            }

            foreach (var u in delta.Units)
            {
                units[(u.Kind, u.Id)] = u;
                EnsureUnitMarker(u);
            }

            hud.Refresh();
        }

        private void RequestSnapshot()
        {
            if (waitingSnapshot) return;
            waitingSnapshot = true;
            state?.RequestSnapshot();
        }

        public void OnWarMoveResult(WarMoveResult result)
        {
            if (!result.Success) hud.ShowMessage(result.Message);
        }

        public void OnWarDeployPuppetResult(WarMoveResult result)
        {
            hud.ShowMessage(result.Success ? "Rối đã lên đường." : result.Message);
        }

        public void OnWarPuppetResult(WarPuppetResult result)
        {
            hud.ShowMessage(result.Success ? "Thành công." : result.Message);
        }

        public void OnWarUseHpPillResult(WarUseHpPillResult result)
        {
            if (!result.Success)
            {
                hud.ShowMessage(result.Message);
                // Túi đồ đã trừ số lượng trên giao diện trước khi server trả lời: dựng lại theo dữ liệu thật.
                var tabPanel = Pool.Get<TabPanel>();
                if (tabPanel != null && tabPanel.gameObject.activeInHierarchy) tabPanel.InitInventory();
                return;
            }

            // Server đã trừ đan: đồng bộ tồn kho local theo số còn lại. HP chiến cập nhật qua delta.
            var inventory = Client.Instance.CharacterData.InventoryManager;
            var owned = inventory.LinhDans.FirstOrDefault(x => x.Id == result.DanId);
            var used = (owned?.Count ?? 0) - result.RemainingCount;
            if (used > 0)
            {
                inventory.Decrease(new ItemRef { Id = result.DanId, Type = ItemType.LinhDan, Count = used });
            }
            hud.ShowMessage($"HP chiến: {result.Hp}/{result.HpMax}");
            hud.Refresh();
        }

        public void OnWarKicked(WarKicked kicked)
        {
            NotifyGroup.Instance?.Show(kicked.Message);
            Exit();
        }

        public void OnWarBattleMatched(WarBattleMatched matched)
        {
            var gameManager = Pool.Get<GameManager>();
            if (gameManager == null) return;
            gameManager.currentETypeCombat = matched.IsPvp ? ETypeCombat.TongMonChienRealTime : ETypeCombat.TongMonChienPuppet;
            // Đánh xong quay lại đúng scene chiến (không lùi về AllMaps).
            gameManager.combatReturnScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            var me = CharacterStatsSecureBuilder.FromCharacterSnapshot(matched.Me);
            me.isPlayer = true;
            var enemy = CharacterStatsSecureBuilder.FromCharacterSnapshot(matched.Enemy);
            enemy.isPlayer = matched.IsPvp;
            gameManager.user = new GameManager.CharCombatInit { eTypeVisual = ETypeVisualChar.Character, charStats = me };
            gameManager.enemyCombat = new GameManager.CharCombatInit { eTypeVisual = ETypeVisualChar.Character, charStats = enemy };
            hud.ShowMessage(matched.IsPvp
                ? $"Giao tranh với {matched.Enemy?.Name}!"
                : $"Giao tranh với {matched.Enemy?.Name} phòng thủ!");
            Client.Instance.ChangeState(new TongMonChienCombat(matched));
            DOVirtual.DelayedCall(1f, () => gameManager.GoToCombatArena());
        }

        // =====================================================================
        // Hiển thị
        // =====================================================================

        public WarClanView GetClan(int clanId) => clans.TryGetValue(clanId, out var c) ? c : null;

        public int? GetOwner(int nodeId)
        {
            foreach (var c in clans.Values)
            {
                if (c.MainNodeId == nodeId) return c.ClanId;
            }

            return nodes.TryGetValue(nodeId, out var n) ? n.OwnerClanId : null;
        }

        public Color GetClanColor(int clanId) =>
            clanColors.TryGetValue(clanId, out var c) ? c : ClanColorUtil.FallbackColor(clanId);

        private async void LoadClanColor(WarClanView clan)
        {
            if (clanColors.ContainsKey(clan.ClanId) && ClanColorUtil.TryGetCached(clan.ImageUrl, out _)) return;
            clanColors[clan.ClanId] = ClanColorUtil.FallbackColor(clan.ClanId);
            if (string.IsNullOrEmpty(clan.ImageUrl))
            {
                RefreshAllNodes();
                return;
            }

            var sprite = await ImageLoader.GetSpriteFromUrl(clan.ImageUrl);
            if (this == null) return;
            clanColors[clan.ClanId] = ClanColorUtil.FromSprite(clan.ClanId, clan.ImageUrl, sprite);
            RefreshAllNodes();
            foreach (var u in units.Values.Where(x => x.ClanId == clan.ClanId)) EnsureUnitMarker(u);
        }

        private void RefreshAllNodes()
        {
            if (mapManager == null) return;
            foreach (var map in mapManager.maps)
            {
                if (map != null) RefreshNode(map.Id);
            }
        }

        private void RefreshNode(int nodeId)
        {
            var map = mapManager != null ? mapManager.FindMap(nodeId) : null;
            if (map == null) return;
            var owner = GetOwner(nodeId);
            if (!ownerMarkers.TryGetValue(nodeId, out var marker) || marker == null)
            {
                var go = new GameObject($"Owner_{nodeId}");
                go.transform.SetParent(overlayRoot, false);
                marker = go.AddComponent<SpriteRenderer>();
                marker.sprite = GetCircleSprite();
                marker.sortingOrder = markerSortingOrder;
                ownerMarkers[nodeId] = marker;
            }

            marker.transform.position = GetNodeAnchor(map);
            marker.transform.localScale = Vector3.one * ownerMarkerScale;
            if (owner is int clanId)
            {
                var color = GetClanColor(clanId);
                color.a = 0.55f;
                marker.color = color;
                marker.gameObject.SetActive(true);
            }
            else
            {
                marker.gameObject.SetActive(false);
            }

            var fighting = nodes.TryGetValue(nodeId, out var n) && n.IsFighting;
            fightingIcons.TryGetValue(nodeId, out var icon);
            if (fighting && icon == null)
            {
                icon = fightingIconPrefab != null ? Instantiate(fightingIconPrefab, overlayRoot) : CreateDefaultFightingIcon();
                icon.transform.position = GetNodeAnchor(map) + fightingIconOffset;
                fightingIcons[nodeId] = icon;
            }
            else if (!fighting && icon != null)
            {
                Destroy(icon);
                fightingIcons.Remove(nodeId);
            }
        }

        private readonly Dictionary<Map, Vector3> nodeAnchors = new Dictionary<Map, Vector3>();

        /// <summary>
        /// Tâm hình của node trên bản đồ. Không dùng map.transform.position vì một số node (vd. Động Phủ, id 8)
        /// có gốc transform lệch xa hình node: lấy tâm sprite "Level1Move" (hình node), thiếu thì dùng
        /// PlayerPosition (chỗ nhân vật đứng), cuối cùng mới dùng transform.
        /// </summary>
        private Vector3 GetNodeAnchor(Map map)
        {
            if (nodeAnchors.TryGetValue(map, out var anchor)) return anchor;
            var visual = map.GetComponentsInChildren<SpriteRenderer>(true)
                .FirstOrDefault(x => x.gameObject.name == "Level1Move");
            if (visual != null && visual.sprite != null && visual.gameObject.activeInHierarchy)
                anchor = visual.bounds.center;
            else if (map.PlayerPosition != Vector2.zero)
                anchor = map.PlayerPosition;
            else
                anchor = map.transform.position;
            anchor.z = map.transform.position.z;
            nodeAnchors[map] = anchor;
            return anchor;
        }

        private GameObject CreateDefaultFightingIcon()
        {
            var go = new GameObject("Fighting");
            go.transform.SetParent(overlayRoot, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = GetCircleSprite();
            sr.color = new Color(1f, 0.15f, 0.1f, 0.9f);
            sr.sortingOrder = markerSortingOrder + 2;
            go.transform.localScale = Vector3.one * 0.35f;
            go.transform.DOScale(0.5f, 0.4f).SetLoops(-1, LoopType.Yoyo).SetLink(go);
            return go;
        }

        private static Sprite GetCircleSprite()
        {
            if (circleSprite != null) return circleSprite;
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var center = (size - 1) / 2f;
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                var d = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / center;
                var a = Mathf.Clamp01((1f - d) * 8f);
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }

            tex.Apply();
            circleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return circleSprite;
        }

        private void EnsureUnitMarker(WarUnitView unit)
        {
            var key = (unit.Kind, unit.Id);
            if (!unitMarkers.TryGetValue(key, out var marker) || marker == null)
            {
                var isMe = unit.Kind == WarUnitKind.Player && unit.Id == myCharId;
                marker = isMe && mapManager != null ? mapManager.MainPlayer : CreateUnitMarker(unit);
                if (marker == null) return;
                unitMarkers[key] = marker;
            }

            // Hiện/ẩn và nhãn do Update quyết định (mỗi node chỉ hiện một đại diện).
            nextLabelRefresh = 0;
        }

        private bool IsMe(WarUnitView unit) => unit.Kind == WarUnitKind.Player && unit.Id == myCharId;

        private void UpdateLabel(WarUnitView unit, Player marker, int othersAtNode, DateTime now)
        {
            var isMe = IsMe(unit);
            // Rối có ngoại hình random do server lưu; người chơi khác dựng theo ngoại hình server gửi.
            if (!isMe) ApplyVisual((unit.Kind, unit.Id), marker, unit);
            var color = isMe ? new Color32(255, 213, 79, 255) : (Color32)GetClanColor(unit.ClanId);
            var name = string.IsNullOrEmpty(unit.Name) ? (unit.Kind == WarUnitKind.Puppet ? "Rối" : "?") : unit.Name;
            var more = othersAtNode > 0 ? $"  (+{othersAtNode})" : string.Empty;
            var countdown = MoveCountdownText(unit, now);
            var label = $"{name}{more}\n{unit.GetHp(now)}/{unit.HpMax}";
            if (countdown != null) label += "\n" + countdown;
            marker.SetName(label, color);
        }

        /// <summary>Đếm ngược tới node kế tiếp (và tới đích nếu còn nhiều chặng) của đối tượng đang đi.</summary>
        private string MoveCountdownText(WarUnitView unit, DateTime now)
        {
            if (unit.State != WarUnitState.Moving || !unit.NextArrivalAt.HasValue || unit.Route == null ||
                unit.Route.Length == 0) return null;
            var next = Mathf.Max(0f, (float)(unit.NextArrivalAt.Value - now).TotalSeconds);
            var text = $"Tới node kế: {FormatSeconds(next)}";
            if (unit.Route.Length > 1)
                text += $" | Đích: {FormatSeconds(next + (unit.Route.Length - 1) * secondsPerEdge)}";
            return text;
        }

        private static string FormatSeconds(float seconds) =>
            seconds < 10f ? $"{seconds:0.0}s" : $"{Mathf.CeilToInt(seconds)}s";

        private Player CreateUnitMarker(WarUnitView unit)
        {
            var prefab = unit.Kind == WarUnitKind.Puppet && puppetPrefab != null
                ? puppetPrefab
                : mapManager != null ? mapManager.PlayerPrefab : null;
            if (prefab == null) return null;
            var go = Instantiate(prefab);
            var player = go.GetComponentInChildren<Player>(true);
            if (player == null)
            {
                Destroy(go);
                return null;
            }

            player.SetActive(true);
            return player;
        }

        private static string VisualSignature(WarUnitView unit)
        {
            var appearance = unit.Appearance;
            if (appearance == null) return "none";
            var skins = appearance.Skins == null
                ? string.Empty
                : string.Join(";", appearance.Skins.OrderBy(x => x.Key).Select(x => x.Key + "=" + x.Value));
            var colors = appearance.Colors == null
                ? string.Empty
                : string.Join(";", appearance.Colors.OrderBy(x => x.Key).Select(x => x.Key + "=" + x.Value));
            return $"{unit.Gender}|{skins}|{colors}";
        }

        /// <summary>Dựng visual theo Gender/Appearance server gửi (rối: random một lần và lưu ở server).</summary>
        private void ApplyVisual((WarUnitKind, int) key, Player marker, WarUnitView unit)
        {
            var signature = VisualSignature(unit);
            if (visualSignatures.TryGetValue(key, out var applied) && applied == signature) return;
            visualSignatures[key] = signature;
            try
            {
                // Chưa có ngoại hình (người chơi offline) thì tạm dùng ngoại hình mặc định của nhân vật mình,
                // sẽ dựng lại ngay khi server gửi ngoại hình thật.
                var appearance = unit.Appearance ?? Client.Instance.CharacterData.Appearance;
                marker.Init(unit.Appearance != null ? unit.Gender : Client.Instance.CharacterData.Gender, false, appearance);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TongMonChien init unit visual failed ({unit.Kind} {unit.Id}): {e.Message}");
            }
        }

        private void RemoveUnitMarker((WarUnitKind, int) key)
        {
            visualSignatures.Remove(key);
            if (!unitMarkers.TryGetValue(key, out var marker)) return;
            unitMarkers.Remove(key);
            if (marker == null || (mapManager != null && marker == mapManager.MainPlayer)) return;
            Destroy(marker.transform.root.gameObject);
        }

        private float nextLabelRefresh;

        private void Update()
        {
            // Đóng túi đồ thì thôi dùng đan cho Rối; lần mở túi sau sẽ dùng cho bản thân như thường.
            if (puppetDanTarget != null)
            {
                var tabPanel = Pool.Get<TabPanel>();
                if (tabPanel == null || !tabPanel.gameObject.activeInHierarchy) ClearDanTarget();
            }

            if (!snapshotLoaded || mapManager == null) return;
            var now = ServerNow;
            var refreshLabels = Time.unscaledTime >= nextLabelRefresh;
            if (refreshLabels) nextLabelRefresh = Time.unscaledTime + 1f;
            // Nhãn của đối tượng đang đi cập nhật dày hơn để đếm ngược chạy mượt.
            var refreshMovers = refreshLabels || Time.unscaledTime >= nextMoverLabelRefresh;
            if (refreshMovers) nextMoverLabelRefresh = Time.unscaledTime + 0.1f;

            // Đối tượng đứng yên tại node được gom theo node; mỗi node chỉ hiện một đại diện
            // (nhãn ghi thêm số còn lại). Đối tượng đang đi trên cạnh hiện riêng từng người.
            var standing = new Dictionary<int, List<WarUnitView>>();
            foreach (var unit in units.Values)
            {
                if (!unitMarkers.TryGetValue((unit.Kind, unit.Id), out var marker) || marker == null) continue;
                if (TryGetMovingPosition(unit, now, out var position, out var directionX))
                {
                    marker.SetPosition(position);
                    marker.SetActive(true);
                    marker.FaceTowards(directionX);
                    marker.SetMovingState(true);
                    if (refreshMovers) UpdateLabel(unit, marker, 0, now);
                    continue;
                }

                marker.SetMovingState(false);

                if (!standing.TryGetValue(unit.NodeId, out var list))
                {
                    list = new List<WarUnitView>();
                    standing[unit.NodeId] = list;
                }

                list.Add(unit);
            }

            foreach (var pair in standing)
            {
                var map = mapManager.FindMap(pair.Key);
                var representative = PickRepresentative(pair.Key, pair.Value);
                foreach (var unit in pair.Value)
                {
                    var marker = unitMarkers[(unit.Kind, unit.Id)];
                    var show = ReferenceEquals(unit, representative);
                    marker.SetActive(show);
                    if (!show) continue;
                    if (map != null) marker.SetPosition(map.PlayerPosition);
                    if (refreshLabels) UpdateLabel(unit, marker, pair.Value.Count - 1, now);
                }
            }

            if (refreshLabels)
            {
                hud.Refresh();
                if (headPanel != null) headPanel.RefreshHp();
            }
        }

        private float nextMoverLabelRefresh;

        /// <summary>
        /// Đại diện của node: bản thân mình → người chơi phe giữ node → người chơi khác → rối phe giữ node → rối khác;
        /// cùng hạng thì ID nhỏ hơn (ổn định giữa các lần vẽ).
        /// </summary>
        private WarUnitView PickRepresentative(int nodeId, List<WarUnitView> list)
        {
            var owner = GetOwner(nodeId);
            return list
                .OrderBy(u => IsMe(u) ? 0 : 1)
                .ThenBy(u => u.Kind == WarUnitKind.Player ? 0 : 1)
                .ThenBy(u => owner.HasValue && u.ClanId == owner.Value ? 0 : 1)
                .ThenBy(u => u.Id)
                .First();
        }

        private bool TryGetMovingPosition(WarUnitView unit, DateTime now, out Vector2 position, out float directionX)
        {
            position = default;
            directionX = 0f;
            if (unit.State != WarUnitState.Moving || unit.Route == null || unit.Route.Length == 0 ||
                !unit.NextArrivalAt.HasValue) return false;
            var from = mapManager.FindMap(unit.NodeId);
            var to = mapManager.FindMap(unit.Route[0]);
            if (from == null || to == null) return false;
            var remaining = (float)(unit.NextArrivalAt.Value - now).TotalSeconds;
            var t = Mathf.Clamp01(1f - remaining / secondsPerEdge);
            position = EvaluateEdge(from, to, t);
            // Hướng mặt theo đoạn đường đang đi (nhìn trước một chút trên cạnh; cuối cạnh thì nhìn lùi).
            var ahead = EvaluateEdge(from, to, Mathf.Min(1f, t + 0.05f));
            directionX = ahead.x - position.x;
            if (Mathf.Abs(directionX) < 0.0001f) directionX = position.x - EvaluateEdge(from, to, Mathf.Max(0f, t - 0.05f)).x;
            return true;
        }

        /// <summary>Vị trí trên cạnh from→to theo tỉ lệ quãng đường t (theo đường MoveNode giống Player.DoMove).</summary>
        private static Vector2 EvaluateEdge(Map from, Map to, float t)
        {
            var points = new List<Vector2> { from.PlayerPosition };
            var node = from.FindNodeByMap(to);
            if (node?.MoveNode != null)
            {
                var positions = node.MoveNode.NodesPosition;
                if (to.Id != node.MoveNode.FinishNode) positions = positions.Reverse().ToArray();
                points.AddRange(positions);
            }

            points.Add(to.PlayerPosition);
            var total = 0f;
            for (var i = 1; i < points.Count; i++) total += Vector2.Distance(points[i - 1], points[i]);
            if (total <= 0.0001f) return to.PlayerPosition;
            var target = total * t;
            for (var i = 1; i < points.Count; i++)
            {
                var len = Vector2.Distance(points[i - 1], points[i]);
                if (target <= len) return Vector2.Lerp(points[i - 1], points[i], len <= 0 ? 1 : target / len);
                target -= len;
            }

            return to.PlayerPosition;
        }
    }
}
