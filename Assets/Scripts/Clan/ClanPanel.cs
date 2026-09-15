using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenKhi;
using Network;
using Network.Core;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;

namespace Clan
{
    public class ClanPanel : BaseClanInfo
    {
        private const int MinContributionMaxLevel = 4;

        [Header("Ẩn khi xem tông môn của người khác")]
        [Tooltip("Toàn bộ khối tính năng chỉ dành cho thành viên (đóng góp, rời tông môn...). " +
                 "Tự ẩn khi đang xem tông môn của người khác (IsViewingExternalClan).")]
        [SerializeField] private GameObject clanFeaturesRoot;

        [Header("Cấp độ & Quỹ tông môn")]
        [SerializeField] private Text levelText;
        [SerializeField] private Text fundsText;
        [SerializeField] private Text honText;

        [Header("Đóng góp tông môn")]
        [SerializeField] private Button contributeOnceButton;
        [SerializeField] private Button contributeTenButton;
        [SerializeField] private GameObject freeContributionBadge;
        [SerializeField] private Text contributeOnceText;
        [SerializeField] private Text contributeTenText;

        [Header("Rời tông môn")]
        [SerializeField] private Button leaveClanButton;

        [Header("Kho tông môn")]
        [SerializeField] private Button openShopButton;
        [SerializeField] private ClanShop clanShop;

        [Header("Truyền thừa tông môn")]
        [SerializeField] private Button openInheritanceButton;
        [SerializeField] private ClanInheritancePanel inheritancePanel;

        [Header("Hồn tông môn")]
        [SerializeField] private Button luyenHonButton;
        [SerializeField] private Button upgradeMaxHonButton;
        [SerializeField] private long luyenHonAmount = 1;
        [SerializeField] private long upgradeMaxHonFunds = 1;

        [Header("Thao tác thành viên")]
        [SerializeField] private ClanMemberActionPopup memberActionPopup;

        [Header("Xin gia nhập (chỉ hiện khi xem tông môn của người khác)")]
        [SerializeField] private Button requestJoinButton;

        [Header("Duyệt đơn xin gia nhập (nút mở popup, chỉ hiện khi có quyền AddMember, xem tông môn của mình)")]
        [SerializeField] private Button joinRequestsButton;
        [SerializeField] private GameObject joinRequestsBadge;
        [SerializeField] private Text joinRequestsBadgeText;

        [Header("Popup danh sách đơn xin gia nhập")]
        [SerializeField] private GameObject joinRequestsRoot;
        [SerializeField] private Button joinRequestsCloseButton;
        [SerializeField] private ClanJoinRequestElement joinRequestElementPrefab;
        [SerializeField] private RectTransform joinRequestsContainer;
        [SerializeField] private GameObject joinRequestsEmptyState;

        private readonly List<ClanJoinRequestElement> joinRequestElements = new();
        private bool isContributing;
        private bool isLeaving;
        private bool isRequestingJoin;
        private bool isProcessingHon;

        public ClanMember CurrentMember { get; private set; }
        public ClanRole CurrentRole => CurrentMember?.Role ?? ClanRole.Member;
        public ClanPermission CurrentPermissions { get; private set; }

        public event Action<ClanPermission> PermissionsChanged;

        protected override void Start()
        {
            base.Start();
            EnsureHonControls();
            SetEventButton(contributeOnceButton, () => ConfirmContribute(1));
            SetEventButton(contributeTenButton, () => ConfirmContribute(10));
            contributeOnceText ??= FindContributionButtonText(contributeOnceButton, freeContributionBadge);
            contributeTenText ??= FindContributionButtonText(contributeTenButton, null);
            SetEventButton(leaveClanButton, ConfirmLeaveClan);
            SetEventButton(openShopButton, OpenShop);
            SetEventButton(openInheritanceButton, OpenInheritance);
            SetEventButton(luyenHonButton, ConfirmLuyenHon);
            SetEventButton(upgradeMaxHonButton, ConfirmUpgradeMaxHon);
            SetEventButton(requestJoinButton, () => _ = RequestJoin());
            SetEventButton(joinRequestsButton, OpenJoinRequests);
            SetEventButton(joinRequestsCloseButton, () => joinRequestsRoot?.SetActive(false));

            if (joinRequestsContainer == null && joinRequestElementPrefab != null)
                joinRequestsContainer = joinRequestElementPrefab.transform.parent as RectTransform;
            CacheReusableJoinRequestElements();
            if (joinRequestElementPrefab != null && joinRequestElementPrefab.gameObject.scene.IsValid())
                joinRequestElementPrefab.SetActive(false);
            if (joinRequestsRoot != null) joinRequestsRoot.SetActive(false);
            RefreshFreeContribution();
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            base.BindClanEvents(handler);
            handler.ShopChanged += OnShopChanged;
            handler.JoinRequestAdded += OnJoinRequestAdded;
            handler.JoinRequestRemoved += OnJoinRequestRemoved;
            handler.FundsChanged += OnClanFundsChanged;
            handler.HonChanged += OnClanHonChanged;
            handler.ExpChanged += OnClanExpChanged;
            handler.LevelChanged += OnClanLevelChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.LevelChanged -= OnClanLevelChanged;
            handler.ExpChanged -= OnClanExpChanged;
            handler.HonChanged -= OnClanHonChanged;
            handler.FundsChanged -= OnClanFundsChanged;
            handler.JoinRequestRemoved -= OnJoinRequestRemoved;
            handler.JoinRequestAdded -= OnJoinRequestAdded;
            handler.ShopChanged -= OnShopChanged;
            base.UnbindClanEvents(handler);
        }

        public override void Init(Network.Models.Clans.Clan clan)
        {
            base.Init(clan);
            RefreshPermissions();
        }

        public override void RefreshClanSummary()
        {
            base.RefreshClanSummary();
            if (levelText != null) levelText.text = clan != null ? $"Cấp {clan.Level}" : "—";
            if (fundsText != null) fundsText.text = clan != null ? $"Quỹ: {clan.Funds:n0}" : "—";
            if (honText != null) honText.text = clan != null ? $"Hồn: {clan.Hon:n0}/{clan.MaxHon:n0}" : "—";
            UpdateHonButtons();
        }

        public bool HasPermission(ClanPermission permission)
        {
            if (CurrentMember == null) return false;
            return permission == ClanPermission.None || (CurrentPermissions & permission) == permission;
        }

        public IReadOnlyList<ClanRole> GetAssignableRoles()
        {
            if (!HasPermission(ClanPermission.AssignRole)) return Array.Empty<ClanRole>();
            return Enum.GetValues(typeof(ClanRole))
                .Cast<ClanRole>()
                .Where(role => role < CurrentRole)
                .OrderByDescending(role => role)
                .ToArray();
        }

        public bool CanAssignRole(ClanMember target, ClanRole role)
        {
            return target != null && CurrentMember != null &&
                   target.CharId != CurrentMember.CharId &&
                   HasPermission(ClanPermission.AssignRole) &&
                   target.Role < CurrentRole &&
                   role < CurrentRole &&
                   Enum.IsDefined(typeof(ClanRole), role) &&
                   target.Role != role &&
                   !IsUniqueRoleOccupied(role, target.CharId);
        }

        private bool IsUniqueRoleOccupied(ClanRole role, int targetCharId)
        {
            if (role != ClanRole.ViceLeader && role != ClanRole.Elder) return false;
            return clan?.Members != null &&
                   clan.Members.Any(member => member.Role == role && member.CharId != targetCharId);
        }

        public Task<SetClanMemberRoleResult> AssignRole(ClanMember target, ClanRole role)
        {
            if (!CanAssignRole(target, role))
            {
                return Task.FromResult(new SetClanMemberRoleResult
                {
                    Success = false,
                    Message = "Chỉ được cấp chức vụ thấp hơn chức vụ của bạn"
                });
            }

            return Handler.SetMemberRole(target.CharId, role);
        }

        public Task<SetClanInheritanceResult> SetInheritance(
            ClanInheritanceTrigger trigger,
            int slotIndex,
            EffectType? effectType)
        {
            if (!HasPermission(ClanPermission.ManageInheritance))
            {
                return Task.FromResult(new SetClanInheritanceResult
                {
                    Success = false,
                    Message = "Chỉ Tộc Chủ được quản lý truyền thừa"
                });
            }

            return Handler.SetInheritance(trigger, slotIndex, effectType);
        }

        public int GetInheritanceSlots()
        {
            return clan == null || Handler == null ? 0 : Handler.GetInheritanceSlots(clan.Level);
        }

        public void RefreshPermissions()
        {
            var characterId = Client.Instance?.CharacterData?.Id ?? 0;
            CurrentMember = clan?.Members?.FirstOrDefault(member => member.CharId == characterId);
            CurrentPermissions = CurrentMember != null && Handler != null
                ? Handler.GetPermissions(CurrentMember.Role)
                : ClanPermission.None;

            if (clanFeaturesRoot != null) clanFeaturesRoot.SetActive(!isViewingExternalClan);
            if (requestJoinButton != null) requestJoinButton.gameObject.SetActive(isViewingExternalClan);

            // Duyệt đơn xin gia nhập dùng chung quyền AddMember với việc thêm thành viên trực tiếp.
            var canManageJoinRequests = !isViewingExternalClan && HasPermission(ClanPermission.AddMember);
            if (joinRequestsButton != null) joinRequestsButton.gameObject.SetActive(canManageJoinRequests);
            if (canManageJoinRequests) RenderJoinRequests();
            else if (joinRequestsRoot != null) joinRequestsRoot.SetActive(false);

            RefreshFreeContribution();
            UpdateHonButtons();
            PermissionsChanged?.Invoke(CurrentPermissions);
        }

        private void RefreshFreeContribution()
        {
            var canContributeByLevel = CanContributeByLevel();
            var hasFreeTurn = HasFreeContributionTurn();
            if (freeContributionBadge != null) freeContributionBadge.SetActive(canContributeByLevel && hasFreeTurn);

            var price = Handler?.CurrentConfig?.ContributionMoneyVipPerTime ?? 0;
            if (contributeOnceText != null)
            {
                contributeOnceText.text = !canContributeByLevel
                    ? "Đóng Góp x1\nTrúc Cơ trở lên"
                    : hasFreeTurn
                    ? "Đóng Góp x1\nMiễn Phí"
                    : $"Đóng Góp x1\n{price:n0} Tiên Ngọc";
            }

            if (contributeTenText != null)
            {
                var paidTimes = hasFreeTurn ? 9 : 10;
                contributeTenText.text = !canContributeByLevel
                    ? "Đóng Góp x10\nTrúc Cơ trở lên"
                    : $"Đóng Góp x10\n{(long)paidTimes * price:n0} Tiên Ngọc";
            }

            UpdateContributionButtons();
        }

        // Server dùng UTC để xác định ngày cống hiến, client phải dùng cùng mốc ngày.
        private bool HasFreeContributionTurn()
        {
            return CurrentMember != null && Handler?.HasFreeContributionTurn() == true;
        }

        private static bool CanContributeByLevel()
        {
            return (Client.Instance?.CharacterData?.Progression?.MaxLevel ?? 0) >= MinContributionMaxLevel;
        }

        private static Text FindContributionButtonText(Button button, GameObject excludedRoot)
        {
            if (button == null) return null;
            return button.GetComponentsInChildren<Text>(true)
                .FirstOrDefault(value => excludedRoot == null || !value.transform.IsChildOf(excludedRoot.transform));
        }

        // MoneyVip là Tiên Ngọc. Một lượt miễn phí vẫn nhận đủ điểm cống hiến,
        // chỉ miễn phần Tiên Ngọc phải trả.
        // trước khi thực sự gửi yêu cầu đóng góp lên server.
        private void ConfirmContribute(int times)
        {
            if (isContributing) return;
            if (!CanContributeByLevel())
            {
                NotifyGroup.Instance?.Show("Từ Trúc Cơ trở lên mới được cống hiến");
                return;
            }

            if (Handler == null)
            {
                NotifyGroup.Instance?.Show("Kết nối tông môn chưa sẵn sàng");
                return;
            }

            var config = Handler.CurrentConfig;
            if (config == null || config.ContributionMoneyVipPerTime <= 0)
            {
                NotifyGroup.Instance?.Show("Cấu hình cống hiến chưa sẵn sàng");
                return;
            }

            if (times <= 0 || times > config.MaxContributionTimesPerRequest)
            {
                NotifyGroup.Instance?.Show($"Chỉ được cống hiến tối đa {config.MaxContributionTimesPerRequest} lượt mỗi lần");
                return;
            }

            var moneyPerTime = config.ContributionMoneyVipPerTime;
            var freeTimes = HasFreeContributionTurn() ? Math.Min(times, 1) : 0;
            var paidTimes = times - freeTimes;
            var cost = (long)paidTimes * moneyPerTime;
            var contributionPoints = (long)times * moneyPerTime;

            string costLine;
            if (paidTimes == 0)
            {
                costLine = "Miễn phí hôm nay";
            }
            else if (freeTimes > 0)
            {
                costLine = $"1 lượt miễn phí + {paidTimes} lượt trả phí: {cost:n0} Tiên Ngọc";
            }
            else
            {
                costLine = $"Chi phí: {cost:n0} Tiên Ngọc";
            }

            var currentMoneyVip = Client.Instance?.CharacterData?.MoneyVip ?? 0;
            if (cost > currentMoneyVip)
            {
                NotifyGroup.Instance?.Show($"Không đủ Tiên Ngọc, cần {cost:n0} Tiên Ngọc");
                return;
            }

            var content = $"Đóng góp {times} lần cho tông môn.\n" +
                          $"{costLine}.\n" +
                          $"Nhận {contributionPoints:n0} điểm cống hiến.";

            Pool.Get<PopupConfirmAction>()?.ShowPopConfirmAction(content, () => _ = Contribute(times));
        }

        public async Task Contribute(int times)
        {
            if (isContributing) return;
            if (!CanContributeByLevel())
            {
                NotifyGroup.Instance?.Show("Từ Trúc Cơ trở lên mới được cống hiến");
                return;
            }

            if (Handler == null)
            {
                NotifyGroup.Instance?.Show("Kết nối tông môn chưa sẵn sàng");
                return;
            }

            SetContributing(true);
            try
            {
                var result = await Handler.Contribute(times);
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể đóng góp"
                        : result.Message);
                }
                else
                {
                    Refresh();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể đóng góp, vui lòng thử lại");
            }
            finally
            {
                SetContributing(false);
            }
        }

        private void SetContributing(bool value)
        {
            isContributing = value;
            UpdateContributionButtons();
        }

        private void UpdateContributionButtons()
        {
            var interactable = !isContributing && CanContributeByLevel();
            if (contributeOnceButton != null) contributeOnceButton.interactable = interactable;
            if (contributeTenButton != null) contributeTenButton.interactable = interactable;
        }

        private void ConfirmLuyenHon()
        {
            if (isProcessingHon || Handler == null || clan == null) return;
            if (!HasPermission(ClanPermission.ManageHon))
            {
                NotifyGroup.Instance?.Show("Bạn không có quyền luyện hồn tông môn");
                return;
            }

            var amount = Math.Max(1, luyenHonAmount);
            var remaining = Math.Max(0, clan.MaxHon - clan.Hon);
            if (remaining <= 0)
            {
                NotifyGroup.Instance?.Show("Hồn tông môn đã đạt giới hạn");
                return;
            }

            var added = Math.Min(amount, remaining);
            Pool.Get<PopupConfirmAction>()?.ShowPopConfirmAction(
                $"Luyện hồn cho tông môn.\nDự kiến tăng {added:n0} Hồn.",
                () => _ = LuyenHon(amount));
        }

        private async Task LuyenHon(long amount)
        {
            if (isProcessingHon || Handler == null) return;
            SetHonProcessing(true);
            try
            {
                var result = await Handler.LuyenHon(amount);
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể luyện hồn"
                        : result.Message);
                }
                else
                {
                    RefreshClanSummary();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể luyện hồn, vui lòng thử lại");
            }
            finally
            {
                SetHonProcessing(false);
            }
        }

        private void ConfirmUpgradeMaxHon()
        {
            if (isProcessingHon || Handler == null || clan == null) return;
            if (!HasPermission(ClanPermission.ManageHon))
            {
                NotifyGroup.Instance?.Show("Bạn không có quyền nâng giới hạn hồn tông môn");
                return;
            }

            var funds = Math.Max(1, upgradeMaxHonFunds);
            if (clan.Funds < funds)
            {
                NotifyGroup.Instance?.Show($"Quỹ tông môn không đủ, cần {funds:n0}");
                return;
            }

            var maxHonPerFund = Math.Max(1, Handler.CurrentConfig?.MaxHonPerFund ?? 1);
            long addedMaxHon;
            try
            {
                addedMaxHon = checked(funds * maxHonPerFund);
            }
            catch (OverflowException)
            {
                NotifyGroup.Instance?.Show("Giới hạn hồn vượt quá mức cho phép");
                return;
            }

            Pool.Get<PopupConfirmAction>()?.ShowPopConfirmAction(
                $"Tiêu {funds:n0} quỹ tông môn.\nTăng giới hạn hồn thêm {addedMaxHon:n0}.",
                () => _ = UpgradeMaxHon(funds));
        }

        private async Task UpgradeMaxHon(long funds)
        {
            if (isProcessingHon || Handler == null) return;
            SetHonProcessing(true);
            try
            {
                var result = await Handler.UpgradeMaxHon(funds);
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể nâng giới hạn hồn"
                        : result.Message);
                }
                else
                {
                    RefreshClanSummary();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể nâng giới hạn hồn, vui lòng thử lại");
            }
            finally
            {
                SetHonProcessing(false);
            }
        }

        private void SetHonProcessing(bool value)
        {
            isProcessingHon = value;
            UpdateHonButtons();
        }

        private void UpdateHonButtons()
        {
            var canManage = !isViewingExternalClan && HasPermission(ClanPermission.ManageHon);
            var canLuyenHon = canManage && !isProcessingHon && clan != null && clan.Hon < clan.MaxHon;
            var funds = Math.Max(1, upgradeMaxHonFunds);
            var canUpgradeMaxHon = canManage && !isProcessingHon && clan != null && clan.Funds >= funds;

            if (luyenHonButton != null)
            {
                luyenHonButton.gameObject.SetActive(canManage);
                luyenHonButton.interactable = canLuyenHon;
            }

            if (upgradeMaxHonButton != null)
            {
                upgradeMaxHonButton.gameObject.SetActive(canManage);
                upgradeMaxHonButton.interactable = canUpgradeMaxHon;
            }
        }

        private void EnsureHonControls()
        {
            if (honText != null && luyenHonButton != null && upgradeMaxHonButton != null) return;

            var parent = fundsText != null ? fundsText.transform.parent : levelText?.transform.parent;
            if (parent == null) return;

            var font = fundsText != null ? fundsText.font : levelText != null ? levelText.font : null;
            if (honText == null)
            {
                honText = CreateHeaderText(parent, "HonText_Runtime", "Hồn: --/--", font, new Vector2(560, 20),
                    new Vector2(320, 42), new Color(0.46f, 0.36f, 0.56f));
            }

            if (luyenHonButton == null)
            {
                luyenHonButton = CreateHeaderButton(parent, "LuyenHonButton_Runtime", "Luyện Hồn", font,
                    new Vector2(560, -42), new Vector2(170, 58), new Color(0.46f, 0.36f, 0.56f));
            }

            if (upgradeMaxHonButton == null)
            {
                upgradeMaxHonButton = CreateHeaderButton(parent, "UpgradeMaxHonButton_Runtime", "Nâng Giới Hạn", font,
                    new Vector2(744, -42), new Vector2(190, 58), new Color(0.78f, 0.64f, 0.35f));
            }
        }

        private static Text CreateHeaderText(
            Transform parent,
            string name,
            string value,
            Font font,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            var text = rect.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = font;
            text.fontSize = 24;
            text.color = color;
            text.alignment = TextAnchor.MiddleLeft;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Button CreateHeaderButton(
            Transform parent,
            string name,
            string label,
            Font font,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var labelText = CreateHeaderText(rect, "Text", label, font, Vector2.zero, size, Color.white);
            var labelRect = labelText.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            labelRect.anchoredPosition = Vector2.zero;
            labelText.fontSize = 20;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.horizontalOverflow = HorizontalWrapMode.Wrap;
            return button;
        }

        private void ConfirmLeaveClan()
        {
            if (isLeaving) return;
            Pool.Get<PopupConfirmAction>()?.ShowPopConfirmAction(
                "Rời tông môn? Toàn bộ đóng góp hiện tại sẽ mất.",
                DoLeaveClan);
        }

        private async void DoLeaveClan()
        {
            if (isLeaving || Handler == null) return;
            isLeaving = true;
            if (leaveClanButton != null) leaveClanButton.interactable = false;
            try
            {
                var result = await Handler.LeaveClan();
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể rời tông môn"
                        : result.Message);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể rời tông môn, vui lòng thử lại");
            }
            finally
            {
                isLeaving = false;
                if (leaveClanButton != null) leaveClanButton.interactable = true;
            }
        }

        private void OpenShop()
        {
            if (clanShop != null) clanShop.Show();
            else NotifyGroup.Instance?.Show("Chưa cấu hình Kho Tông Môn");
        }

        private void OpenInheritance()
        {
            if (inheritancePanel != null) inheritancePanel.Show();
            else NotifyGroup.Instance?.Show("Chưa cấu hình Truyền Thừa Tông Môn");
        }

        public Task<KickClanMemberResult> KickMember(int charId)
        {
            if (!HasPermission(ClanPermission.KickMember))
            {
                return Task.FromResult(new KickClanMemberResult
                {
                    Success = false,
                    Message = "Bạn không có quyền đuổi thành viên"
                });
            }

            return Handler.KickMember(charId);
        }

        public async Task RequestJoin()
        {
            if (isRequestingJoin || Handler == null || clan == null) return;

            isRequestingJoin = true;
            if (requestJoinButton != null) requestJoinButton.interactable = false;
            try
            {
                var result = await Handler.RequestJoinClan(clan.Id);
                var message = !string.IsNullOrWhiteSpace(result.Message)
                    ? result.Message
                    : result.Success ? "Đã gửi yêu cầu xin gia nhập" : "Không thể gửi yêu cầu xin gia nhập";
                NotifyGroup.Instance?.Show(message);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể gửi yêu cầu xin gia nhập, vui lòng thử lại");
            }
            finally
            {
                isRequestingJoin = false;
                if (requestJoinButton != null) requestJoinButton.interactable = true;
            }
        }

        private void OpenJoinRequests()
        {
            if (!HasPermission(ClanPermission.AddMember)) return;
            RenderJoinRequests();
            if (joinRequestsRoot != null) joinRequestsRoot.SetActive(true);
        }

        private void OnJoinRequestAdded(ClanJoinRequest request)
        {
            if (request != null && request.ClanId == clan?.Id) RenderJoinRequests();
        }

        private void OnJoinRequestRemoved(ClanJoinRequestRemoved removed)
        {
            if (removed != null && removed.ClanId == clan?.Id) RenderJoinRequests();
        }

        private void OnClanExpChanged(ClanExpChanged changed)
        {
            if (changed != null && changed.ClanId == clan?.Id) RefreshClanSummary();
        }

        private void OnClanFundsChanged(ClanFundsChanged changed)
        {
            if (changed != null && changed.ClanId == clan?.Id) RefreshClanSummary();
        }

        private void OnClanHonChanged(ClanHonChanged changed)
        {
            if (changed != null && changed.ClanId == clan?.Id) RefreshClanSummary();
        }

        private void OnClanLevelChanged(ClanLevelChanged changed)
        {
            if (changed == null || changed.ClanId != clan?.Id) return;
            RefreshClanSummary();
            RefreshPermissions();
        }

        private async void OnApproveJoinRequest(ClanJoinRequestElement element)
        {
            if (element?.Request == null || Handler == null) return;
            if (!HasPermission(ClanPermission.AddMember))
            {
                NotifyGroup.Instance?.Show("Bạn không có quyền duyệt đơn xin gia nhập");
                return;
            }

            var charId = element.CharId;
            element.SetProcessing(true);
            try
            {
                var result = await Handler.ApproveJoinRequest(charId);
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể duyệt đơn xin gia nhập"
                        : result.Message);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể duyệt đơn xin gia nhập, vui lòng thử lại");
            }
            finally
            {
                if (element != null) element.SetProcessing(false);
            }
        }

        private async void OnRejectJoinRequest(ClanJoinRequestElement element)
        {
            if (element?.Request == null || Handler == null) return;
            if (!HasPermission(ClanPermission.AddMember))
            {
                NotifyGroup.Instance?.Show("Bạn không có quyền từ chối đơn xin gia nhập");
                return;
            }

            var charId = element.CharId;
            element.SetProcessing(true);
            try
            {
                var result = await Handler.RejectJoinRequest(charId);
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể từ chối đơn xin gia nhập"
                        : result.Message);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể từ chối đơn xin gia nhập, vui lòng thử lại");
            }
            finally
            {
                if (element != null) element.SetProcessing(false);
            }
        }

        private void RenderJoinRequests()
        {
            var requests = clan?.JoinRequests;
            var count = requests?.Count ?? 0;
            for (var i = 0; i < count; i++)
            {
                var element = GetOrCreateJoinRequestElement(i);
                if (element == null) break;
                element.Init(requests[i]);
                element.SetActive(true);
            }

            for (var i = count; i < joinRequestElements.Count; i++)
            {
                joinRequestElements[i].Clear();
                joinRequestElements[i].SetActive(false);
            }

            if (joinRequestsEmptyState != null) joinRequestsEmptyState.SetActive(count == 0);
            if (joinRequestsBadge != null) joinRequestsBadge.SetActive(count > 0);
            if (joinRequestsBadgeText != null) joinRequestsBadgeText.text = count > 0 ? count.ToString() : string.Empty;
        }

        private ClanJoinRequestElement GetOrCreateJoinRequestElement(int index)
        {
            while (joinRequestElements.Count <= index)
            {
                if (joinRequestElementPrefab == null || joinRequestsContainer == null)
                {
                    Debug.LogWarning($"{GetType().Name}: chưa cấu hình joinRequestElementPrefab/joinRequestsContainer", this);
                    return null;
                }

                var element = Instantiate(joinRequestElementPrefab, joinRequestsContainer);
                element.name = $"ClanJoinRequestElement_{joinRequestElements.Count}";
                element.Clear();
                element.ApproveClicked += OnApproveJoinRequest;
                element.RejectClicked += OnRejectJoinRequest;
                joinRequestElements.Add(element);
            }

            return joinRequestElements[index];
        }

        private void CacheReusableJoinRequestElements()
        {
            if (joinRequestsContainer == null) return;
            foreach (var element in joinRequestsContainer.GetComponentsInChildren<ClanJoinRequestElement>(true))
            {
                if (element == null || element == joinRequestElementPrefab || joinRequestElements.Contains(element)) continue;
                element.Clear();
                element.SetActive(false);
                element.ApproveClicked += OnApproveJoinRequest;
                element.RejectClicked += OnRejectJoinRequest;
                joinRequestElements.Add(element);
            }
        }

        protected override void OnMemberElementClicked(ClanMemberElement element)
        {
            // Luôn cho xem chi tiết bất kỳ thành viên nào (kể cả bản thân); các nút
            // phong chức / đuổi bên trong popup tự ẩn nếu không hợp lệ hoặc không đủ quyền.
            if (element?.Member == null || memberActionPopup == null) return;
            memberActionPopup.Show(this, element);
        }

        protected virtual void OnShopChanged(IReadOnlyList<ClanShopItem> items)
        {
        }

        protected override void OnDestroy()
        {
            foreach (var element in joinRequestElements)
            {
                if (element == null) continue;
                element.ApproveClicked -= OnApproveJoinRequest;
                element.RejectClicked -= OnRejectJoinRequest;
            }

            PermissionsChanged = null;
            base.OnDestroy();
        }
    }
}
