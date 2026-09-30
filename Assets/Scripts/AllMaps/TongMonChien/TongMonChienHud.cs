using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        [SerializeField] private Text hpText;
        [SerializeField] private Text stateText;
        [Tooltip("Tuỳ chọn: thanh HP chiến.")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private Button useDanButton;
        [SerializeField] private Button puppetsButton;
        [SerializeField] private Button exitButton;

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
            EnsureEventSystem();
            Refresh();
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
                SetText(hpText, "Đang tải dữ liệu chiến trường...");
                SetText(stateText, string.Empty);
                if (hpBar != null) hpBar.value = 0;
            }
            else
            {
                var hp = me.GetHp(now);
                var sb = new StringBuilder($"HP chiến: {hp}/{me.HpMax}");
                if (hp < me.HpMax && me.RegenPerMinute > 0) sb.Append($"  (+{me.RegenPerMinute:0.#}/phút)");
                SetText(hpText, sb.ToString());
                var state = StateText(me.State);
                if (hp < 1) state += " - cần hồi tối thiểu 1 HP để tham chiến";
                SetText(stateText, state);
                if (hpBar != null) hpBar.value = me.HpMax > 0 ? (float)hp / me.HpMax : 0;
            }

            if (deployBar != null) deployBar.SetActive(controller.DeployingPuppetId > 0);
            if (puppetPanel != null && puppetPanel.gameObject.activeSelf) RefreshPuppets(now);
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
