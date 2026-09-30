using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using Network.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AllMaps.TongMonChien
{
    /// <summary>
    /// HUD Tông môn chiến. Giao diện nằm trong prefab <c>TongMonChienHud.prefab</c> (chỉnh sửa tự do trong Unity);
    /// script chỉ giữ tham chiếu và đổ dữ liệu. Mọi thao tác gọi qua <see cref="TongMonChienController"/>.
    /// Tạo lại prefab mặc định: menu Tools → Tông môn chiến → Tạo prefab HUD.
    /// </summary>
    public class TongMonChienHud : MonoBehaviour
    {
        [Header("Chung")]
        [SerializeField] private Canvas canvas;
        [Tooltip("Chỉ dùng khi không có NotifyGroup (thông báo mặc định hiện qua NotifyGroup của game).")]
        [SerializeField] private Text messageText;
        [SerializeField] private float messageDuration = 3.5f;
        [Tooltip("Tự dời các khối xuống dưới UIManager (head, icon...) để không bị che.")]
        [SerializeField] private bool avoidUiManager = true;
        [SerializeField] private float margin = 20f;

        [Header("Bảng thông tin bản thân")]
        [SerializeField] private RectTransform infoPanel;
        [Tooltip("Trạng thái bản thân. HP chiến hiển thị trên UIHead.")]
        [SerializeField] private Text stateText;
        [SerializeField] private Button useDanButton;
        [SerializeField] private Button puppetsButton;
        [SerializeField] private Button exitButton;

        [Header("Thưởng hằng ngày")]
        [Tooltip("Số điểm (node thường) tông đã chiếm.")]
        [SerializeField] private Text nodeText;
        [SerializeField] private Text nodeMoneyText;
        [Tooltip("Số thành (Stronghold) tông đã chiếm.")]
        [SerializeField] private Text strongholdText;
        [SerializeField] private Text strongholdMoneyText;
        [SerializeField] private Text totalMoneyText;
        [Tooltip("Nhãn 'Thực nhận (x%)' theo chức vụ.")]
        [SerializeField] private Text myRewardLabel;
        [SerializeField] private Text myMoneyText;
        [Tooltip("Thời gian còn lại tới lúc phát thưởng (0h hằng ngày).")]
        [SerializeField] private Text settlementText;
        [Tooltip("Thời gian còn lại của mùa (2 tuần); hết mùa làm mới đất đã chiếm.")]
        [SerializeField] private Text seasonText;
        [Tooltip("Icon tiên ngọc; bỏ trống sprite thì tự lấy MoneyResources.MoneyVipIcon.")]
        [SerializeField] private Image[] moneyVipIcons = new Image[0];

        [Header("Bảng Rối")]
        [SerializeField] private RectTransform puppetPanel;
        [SerializeField] private Text puppetHeader;
        [SerializeField] private Button createPuppetButton;
        [Tooltip("Nút đóng bảng Rối (tuỳ chọn).")]
        [SerializeField] private Button closePuppetsButton;
        [SerializeField] private Transform puppetList;
        [SerializeField] private TongMonChienPuppetRow puppetRowPrefab;

        [Header("Thanh chọn node khi điều Rối")]
        [SerializeField] private GameObject deployBar;
        [SerializeField] private Button cancelDeployButton;

        private TongMonChienController controller;
        private float messageHideAt;
        private float nextLayoutAt;
        private HudDraggablePanel infoDrag;
        private HudDraggablePanel puppetDrag;
        private readonly List<TongMonChienPuppetRow> rows = new List<TongMonChienPuppetRow>();
        private readonly List<Graphic> uiManagerGraphics = new List<Graphic>();
        private readonly Vector3[] corners = new Vector3[4];

        public void Bind(TongMonChienController owner)
        {
            controller = owner;
            AddClick(useDanButton, controller.OpenInventoryForSelf);
            AddClick(puppetsButton, TogglePuppets);
            AddClick(exitButton, controller.Exit);
            AddClick(createPuppetButton, controller.CreatePuppet);
            AddClick(closePuppetsButton, TogglePuppets);
            AddClick(cancelDeployButton, controller.CancelDeploy);
            infoDrag = infoPanel != null ? infoPanel.GetComponent<HudDraggablePanel>() : null;
            puppetDrag = puppetPanel != null ? puppetPanel.GetComponent<HudDraggablePanel>() : null;
            if (infoDrag != null) infoDrag.PositionReset += RequestLayout;
            if (puppetDrag != null) puppetDrag.PositionReset += RequestLayout;
            if (puppetPanel != null) puppetPanel.gameObject.SetActive(false);
            if (deployBar != null) deployBar.SetActive(false);
            if (messageText != null) messageText.text = string.Empty;
            ApplyMoneyVipIcon();
            EnsureEventSystem();
            Refresh();
        }

        private void ApplyMoneyVipIcon()
        {
            var icon = MoneyResources.Instance != null ? MoneyResources.Instance.MoneyVipIcon : null;
            if (icon == null) return;
            foreach (var image in moneyVipIcons)
            {
                if (image != null && image.sprite == null) image.sprite = icon;
            }
        }

        private static void AddClick(Button button, Action action)
        {
            if (button != null) button.onClick.AddListener(() => action());
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        private void RequestLayout() => nextLayoutAt = 0;

        private void TogglePuppets()
        {
            if (puppetPanel == null) return;
            puppetPanel.gameObject.SetActive(!puppetPanel.gameObject.activeSelf);
            Refresh();
        }

        /// <summary>Thông báo qua NotifyGroup của game (dễ thấy, cùng kiểu với các thông báo khác).</summary>
        public void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            var notify = NotifyGroup.Instance;
            if (notify != null)
            {
                notify.Show(message);
                return;
            }

            if (messageText == null) return;
            messageText.text = message;
            messageHideAt = Time.unscaledTime + messageDuration;
        }

        private void Update()
        {
            if (messageText != null && messageText.text.Length > 0 && Time.unscaledTime > messageHideAt)
                messageText.text = string.Empty;
        }

        private void LateUpdate()
        {
            if (Time.unscaledTime < nextLayoutAt) return;
            nextLayoutAt = Time.unscaledTime + 0.5f;
            if (avoidUiManager) AvoidUiManagerOverlap();
        }

        // =====================================================================
        // Đổ dữ liệu
        // =====================================================================

        public void Refresh()
        {
            if (controller == null) return;
            var me = controller.Me;
            var now = controller.ServerNow;
            if (me == null)
            {
                SetText(stateText, "Đang tải dữ liệu chiến trường...");
            }
            else
            {
                var state = StateText(me.State);
                if (me.GetHp(now) < 1) state += " - cần hồi tối thiểu 1 HP để tham chiến";
                SetText(stateText, state);
            }

            RefreshDaily(now);

            if (deployBar != null) deployBar.SetActive(controller.DeployingPuppetId > 0);
            if (puppetPanel != null && puppetPanel.gameObject.activeSelf) RefreshPuppets(now);
        }

        /// <summary>
        /// Thưởng hằng ngày của tông: số điểm/thành đã chiếm, tiên ngọc/ngày tương ứng, tổng của tông và số thực nhận
        /// của bản thân theo % chức vụ (Tộc chủ 50%, Phó tộc chủ/Trưởng lão 20%, Đệ tử 10%).
        /// </summary>
        private void RefreshDaily(DateTime now)
        {
            var clan = controller.MyClan;
            var strongholds = clan?.StrongholdCount ?? 0;
            var normalNodes = clan?.NodeCount ?? 0;
            long strongholdMoney = (long)strongholds * controller.StrongholdMoneyVip;
            long nodeMoney = (long)normalNodes * controller.NodeMoneyVip;
            var total = strongholdMoney + nodeMoney;
            var percent = controller.MyRewardPercent;
            SetText(nodeText, $"Số điểm đã chiếm: {normalNodes}");
            SetText(nodeMoneyText, $"{nodeMoney:N0} /ngày");
            SetText(strongholdText, $"Số thành đã chiếm: {strongholds}");
            SetText(strongholdMoneyText, $"{strongholdMoney:N0} /ngày");
            SetText(totalMoneyText, $"{total:N0} /ngày");
            SetText(myRewardLabel, $"Thực nhận ({percent}%)");
            SetText(myMoneyText, $"{total * percent / 100:N0} /ngày");
            var settleAt = controller.NextSettlementAt;
            SetText(settlementText, settleAt == default ? string.Empty : $"Phát thưởng qua thư sau {DurationText(settleAt - now)}");
            var seasonEnd = controller.SeasonEndAt;
            SetText(seasonText, seasonEnd == default ? string.Empty : $"Mùa kết thúc sau {DurationText(seasonEnd - now)} (làm mới đất)");
        }

        private static string DurationText(TimeSpan span)
        {
            if (span < TimeSpan.Zero) span = TimeSpan.Zero;
            if (span.TotalDays >= 1) return $"{(int)span.TotalDays} ngày {span.Hours} giờ";
            if (span.TotalHours >= 1) return $"{(int)span.TotalHours} giờ {span.Minutes} phút";
            return $"{Math.Max(1, (int)Math.Ceiling(span.TotalMinutes))} phút";
        }

        private void RefreshPuppets(DateTime now)
        {
            var puppets = controller.MyPuppets.ToList();
            var leader = controller.IsLeader;
            SetText(puppetHeader,
                $"Rối ({puppets.Count}/{controller.MaxPuppetCount}) - tối đa {LevelText(controller.MaxPuppetLevel)}");
            if (createPuppetButton != null)
                createPuppetButton.gameObject.SetActive(leader && puppets.Count < controller.MaxPuppetCount);
            if (puppetList == null || puppetRowPrefab == null) return;

            // Tái sử dụng các hàng đã có, chỉ tạo thêm khi thiếu.
            while (rows.Count < puppets.Count) rows.Add(Instantiate(puppetRowPrefab, puppetList));
            for (var i = 0; i < rows.Count; i++)
            {
                var active = i < puppets.Count;
                rows[i].gameObject.SetActive(active);
                if (active) rows[i].Bind(puppets[i], now, leader, controller);
            }
        }

        /// <summary>Cảnh giới theo level (GameManager.levelWithText), ví dụ "Luyện Khí Sơ Kì".</summary>
        public static string LevelText(int level) =>
            GameManager.levelWithText.TryGetValue(level, out var text) ? text : $"Lv {level}";

        public static string StateText(WarUnitState state)
        {
            switch (state)
            {
                case WarUnitState.Moving: return "Đang di chuyển";
                case WarUnitState.Fighting: return "Đang giao tranh";
                case WarUnitState.Holding: return "Đang chờ phân thắng bại tại node";
                default: return "Sẵn sàng";
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null) text.text = value;
        }

        // =====================================================================
        // Tránh bị UIManager che
        // =====================================================================

        /// <summary>
        /// UIManager (DontDestroyOnLoad, canvas order 101) vẽ đè lên HUD và còn chứa popup/loading nên không thể
        /// nâng HUD lên trên nó. Đo các phần tử UIManager đang hiện ở dải trên màn hình và đẩy từng khối HUD
        /// (chưa bị người chơi kéo đi chỗ khác) xuống ngay dưới phần tử thấp nhất cùng cột.
        /// </summary>
        private void AvoidUiManagerOverlap()
        {
            var uiManager = Pool.Get<UIManager>();
            if (uiManager == null || canvas == null) return;
            uiManager.GetComponentsInChildren(false, uiManagerGraphics);
            float width = Screen.width, height = Screen.height;
            float leftBottom = height, centerBottom = height, rightBottom = height;
            foreach (var graphic in uiManagerGraphics)
            {
                if (graphic == null || !graphic.enabled || graphic.color.a <= 0.01f) continue;
                if (graphic.canvasRenderer.GetInheritedAlpha() <= 0.01f) continue;
                graphic.rectTransform.GetWorldCorners(corners);
                float xMin = corners[0].x, yMin = corners[0].y, xMax = corners[2].x, yMax = corners[2].y;
                if (yMax - yMin > height * 0.5f || xMax - xMin > width * 0.6f) continue;
                if (yMin < height * 0.5f || xMax < 0 || xMin > width) continue;
                if (xMin < width * 0.45f) leftBottom = Mathf.Min(leftBottom, yMin);
                if (xMax > width * 0.55f) rightBottom = Mathf.Min(rightBottom, yMin);
                if (xMax > width * 0.3f && xMin < width * 0.7f) centerBottom = Mathf.Min(centerBottom, yMin);
            }

            var scale = canvas.scaleFactor > 0 ? canvas.scaleFactor : 1f;
            float Offset(float bottom) => Mathf.Max(margin, (height - bottom) / scale + 10f);
            PlaceBelow(infoPanel, infoDrag, Offset(leftBottom));
            PlaceBelow(puppetPanel, puppetDrag, Offset(rightBottom));
            if (messageText != null)
            {
                var rect = messageText.rectTransform;
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -Offset(centerBottom));
            }
        }

        private static void PlaceBelow(RectTransform panel, HudDraggablePanel drag, float offset)
        {
            if (panel == null) return;
            if (drag != null && drag.IsPinned)
            {
                drag.ClampToScreen();
                return;
            }

            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, -offset);
        }
    }
}
