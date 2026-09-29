using System;
using System.Collections.Generic;
using System.Linq;
using LongDH.LongDHModules;
using LongDHModules;
using System.Text;
using Network.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AllMaps.TongMonChien
{
    /// <summary>
    /// HUD tối thiểu của Tông môn chiến, dựng hoàn toàn bằng code để chạy được ngay không cần prefab.
    /// Có thể thay bằng UI chính thức sau; mọi thao tác đều gọi qua <see cref="TongMonChienController"/>.
    /// </summary>
    public class TongMonChienHud : MonoBehaviour
    {
        private TongMonChienController controller;
        private Font font;
        private Text hpText;
        private Text messageText;
        private float messageHideAt;
        private GameObject puppetPanel;
        private Transform puppetList;
        private Text puppetHeader;
        private Button createButton;
        private GameObject deployBar;
        private string puppetSignature = string.Empty;
        private Canvas canvas;
        private RectTransform infoRect;
        private RectTransform puppetRect;
        private RectTransform messageRect;
        private float nextLayoutAt;
        private readonly List<Graphic> uiManagerGraphics = new List<Graphic>();
        private readonly Vector3[] corners = new Vector3[4];
        private const float Margin = 20f;
        private HudDraggablePanel infoDrag;
        private HudDraggablePanel puppetDrag;

        public static TongMonChienHud Create(TongMonChienController controller)
        {
            var go = new GameObject("TongMonChienHud");
            var hud = go.AddComponent<TongMonChienHud>();
            hud.Build(controller);
            return hud;
        }

        private void Build(TongMonChienController owner)
        {
            controller = owner;
            // Font HUD: CLOUDLIBIAN-GB-NORMAL gán trên controller; thiếu thì dùng font mặc định của Unity.
            font = owner.HudFont != null ? owner.HudFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            gameObject.AddComponent<GraphicRaycaster>();
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            // Bảng thông tin bản thân (góc trên trái).
            var info = Panel("Info", transform, new Vector2(0, 1), new Vector2(20, -20), new Vector2(520, 210));
            infoRect = (RectTransform)info;
            infoDrag = MakeDraggable(info, "Info");
            Label("Title", info, "TÔNG MÔN CHIẾN", 30, new Vector2(15, -10), new Vector2(490, 40), FontStyle.Normal);
            hpText = Label("Hp", info, "", 24, new Vector2(15, -52), new Vector2(490, 70), FontStyle.Normal);
            ButtonAt(info, "Dùng đan", new Vector2(15, -130), new Vector2(155, 48), () => controller.OpenInventoryForSelf());
            ButtonAt(info, "Rối", new Vector2(182, -130), new Vector2(155, 48), TogglePuppets);
            ButtonAt(info, "Thoát", new Vector2(349, -130), new Vector2(155, 48), () => controller.Exit());

            // Dòng thông báo (giữa trên).
            var msg = new GameObject("Message", typeof(RectTransform));
            msg.transform.SetParent(transform, false);
            var msgRect = (RectTransform)msg.transform;
            messageRect = msgRect;
            msgRect.anchorMin = msgRect.anchorMax = new Vector2(0.5f, 1);
            msgRect.pivot = new Vector2(0.5f, 1);
            msgRect.anchoredPosition = new Vector2(0, -30);
            msgRect.sizeDelta = new Vector2(900, 50);
            messageText = msg.AddComponent<Text>();
            messageText.font = font;
            messageText.fontSize = 26;
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.color = new Color(1f, 0.92f, 0.6f);
            messageText.raycastTarget = false;
            msg.AddComponent<Outline>();

            // Thanh chọn node khi điều rối.
            deployBar = Panel("DeployBar", transform, new Vector2(0.5f, 0), new Vector2(-260, 30), new Vector2(520, 70)).gameObject;
            var deployRect = (RectTransform)deployBar.transform;
            deployRect.pivot = new Vector2(0, 0);
            Label("DeployText", deployBar.transform, "Chọn node đích cho rối", 22, new Vector2(15, -10), new Vector2(330, 50), FontStyle.Normal);
            ButtonAt(deployBar.transform, "Huỷ", new Vector2(365, -11), new Vector2(140, 48), () => controller.CancelDeploy());
            deployBar.SetActive(false);

            // Bảng rối (bên phải).
            var puppets = Panel("Puppets", transform, new Vector2(1, 1), new Vector2(-20, -20), new Vector2(760, 540));
            puppetRect = (RectTransform)puppets;
            puppetRect.pivot = new Vector2(1, 1);
            puppetDrag = MakeDraggable(puppets, "Puppets");
            puppetPanel = puppets.gameObject;
            puppetHeader = Label("Header", puppets, "Rối", 26, new Vector2(15, -10), new Vector2(520, 40), FontStyle.Normal);
            createButton = ButtonAt(puppets, "Tạo rối", new Vector2(560, -8), new Vector2(185, 44), () => controller.CreatePuppet());
            var list = new GameObject("List", typeof(RectTransform));
            list.transform.SetParent(puppets, false);
            var listRect = (RectTransform)list.transform;
            listRect.anchorMin = new Vector2(0, 1);
            listRect.anchorMax = new Vector2(0, 1);
            listRect.pivot = new Vector2(0, 1);
            listRect.anchoredPosition = new Vector2(10, -60);
            listRect.sizeDelta = new Vector2(740, 450);
            puppetList = list.transform;
            puppetPanel.SetActive(false);
        }

        private void TogglePuppets()
        {
            puppetPanel.SetActive(!puppetPanel.activeSelf);
            puppetSignature = string.Empty;
            Refresh();
        }

        public void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(message) || messageText == null) return;
            messageText.text = message;
            messageHideAt = Time.unscaledTime + 3.5f;
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
            AvoidUiManagerOverlap();
        }

        /// <summary>
        /// UIManager (DontDestroyOnLoad, canvas order 101) vẽ đè lên HUD và còn chứa popup/loading nên không thể
        /// nâng HUD lên trên nó. Thay vào đó đo các phần tử UIManager đang hiện ở dải trên màn hình (UIHeadPanel,
        /// nút danh sách icon...) và đẩy từng khối HUD xuống ngay dưới phần tử thấp nhất cùng cột.
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
                // Bỏ qua nền/khung toàn màn hình và phần tử không nằm ở dải trên.
                if (yMax - yMin > height * 0.5f || xMax - xMin > width * 0.6f) continue;
                if (yMin < height * 0.5f || xMax < 0 || xMin > width) continue;
                if (xMin < width * 0.45f) leftBottom = Mathf.Min(leftBottom, yMin);
                if (xMax > width * 0.55f) rightBottom = Mathf.Min(rightBottom, yMin);
                if (xMax > width * 0.3f && xMin < width * 0.7f) centerBottom = Mathf.Min(centerBottom, yMin);
            }

            var scale = canvas.scaleFactor > 0 ? canvas.scaleFactor : 1f;
            float Offset(float bottom) => Mathf.Max(Margin, (height - bottom) / scale + 10f);
            // Khối người chơi đã tự kéo thì giữ nguyên, chỉ đảm bảo không trôi ra ngoài màn hình.
            if (infoRect != null && (infoDrag == null || !infoDrag.IsPinned))
                infoRect.anchoredPosition = new Vector2(Margin, -Offset(leftBottom));
            else infoDrag?.ClampToScreen();
            if (puppetRect != null && (puppetDrag == null || !puppetDrag.IsPinned))
                puppetRect.anchoredPosition = new Vector2(-Margin, -Offset(rightBottom));
            else puppetDrag?.ClampToScreen();
            if (messageRect != null) messageRect.anchoredPosition = new Vector2(0, -Offset(centerBottom));
        }

        public void Refresh()
        {
            if (controller == null || hpText == null) return;
            var me = controller.Me;
            var now = controller.ServerNow;
            if (me == null)
            {
                hpText.text = "Đang tải dữ liệu chiến trường...";
            }
            else
            {
                var hp = me.GetHp(now);
                var sb = new StringBuilder();
                sb.Append($"HP chiến: {hp}/{me.HpMax}");
                if (hp < me.HpMax && me.RegenPerMinute > 0) sb.Append($"  (+{me.RegenPerMinute:0.#}/phút)");
                sb.Append('\n').Append(StateText(me.State));
                if (hp < 1) sb.Append(" - cần hồi tối thiểu 1 HP để tham chiến");
                hpText.text = sb.ToString();
            }

            deployBar.SetActive(controller.DeployingPuppetId > 0);
            if (puppetPanel.activeSelf) RefreshPuppets(now);
        }

        private static string StateText(WarUnitState state)
        {
            switch (state)
            {
                case WarUnitState.Moving: return "Đang di chuyển";
                case WarUnitState.Fighting: return "Đang giao tranh";
                case WarUnitState.Holding: return "Đang chờ phân thắng bại tại node";
                default: return "Sẵn sàng";
            }
        }

        private void RefreshPuppets(DateTime now)
        {
            var puppets = controller.MyPuppets.ToList();
            var leader = controller.IsLeader;
            puppetHeader.text = $"Rối ({puppets.Count}/{controller.MaxPuppetCount}) - level tối đa {controller.MaxPuppetLevel}";
            createButton.gameObject.SetActive(leader && puppets.Count < controller.MaxPuppetCount);
            var signature = string.Join("|", puppets.Select(p =>
                $"{p.Id},{p.Level},{p.GetHp(now)},{p.HpMax},{p.NodeId},{p.State}")) + leader;
            if (signature == puppetSignature) return;
            puppetSignature = signature;
            for (var i = puppetList.childCount - 1; i >= 0; i--) Destroy(puppetList.GetChild(i).gameObject);
            var y = 0f;
            foreach (var p in puppets)
            {
                var hp = p.GetHp(now);
                var row = Panel($"Puppet_{p.Id}", puppetList, new Vector2(0, 1), new Vector2(0, y), new Vector2(740, 80));
                Label("Text", row,
                    $"{p.Name}  Lv {p.Level}\nHP {hp}/{p.HpMax}  |  Node {p.NodeId}  |  {StateText(p.State)}",
                    20, new Vector2(10, -6), new Vector2(430, 70), FontStyle.Normal);
                if (leader)
                {
                    var id = p.Id;
                    ButtonAt(row, "Nâng", new Vector2(445, -16), new Vector2(90, 48), () => controller.UpgradePuppet(id));
                    var deploy = ButtonAt(row, "Điều", new Vector2(543, -16), new Vector2(90, 48), () => controller.BeginDeployPuppet(id));
                    deploy.interactable = hp >= p.HpMax && p.State == WarUnitState.Idle;
                    ButtonAt(row, "Đan", new Vector2(641, -16), new Vector2(90, 48), () => controller.OpenInventoryForPuppet(id))
                        .interactable = hp < p.HpMax && p.State != WarUnitState.Fighting;
                }

                y -= 88;
            }
        }

        // ------------------------------------------------------------------ helpers

        private HudDraggablePanel MakeDraggable(Transform panel, string key)
        {
            var drag = panel.gameObject.AddComponent<HudDraggablePanel>();
            drag.Init(key);
            // Nhấp đúp để trả về vị trí tự động: bố trí lại ngay.
            drag.PositionReset += () => nextLayoutAt = 0;
            // Gợi ý thao tác ở góc dưới phải của khối.
            var hint = Label("DragHint", panel, "Kéo nền để di chuyển - nhấp đúp để đặt lại", 14,
                Vector2.zero, new Vector2(320, 20), FontStyle.Italic);
            var hintRect = (RectTransform)hint.transform;
            hintRect.anchorMin = hintRect.anchorMax = new Vector2(1, 0);
            hintRect.pivot = new Vector2(1, 0);
            hintRect.anchoredPosition = new Vector2(-8, 4);
            hint.alignment = TextAnchor.LowerRight;
            hint.color = new Color(0.96f, 0.9f, 0.78f, 0.45f);
            return drag;
        }

        private Transform Panel(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            go.GetComponent<Image>().color = new Color(0.08f, 0.06f, 0.05f, 0.78f);
            return go.transform;
        }

        private Text Label(string name, Transform parent, string text, int fontSize, Vector2 position, Vector2 size,
            FontStyle style)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var label = go.AddComponent<Text>();
            label.font = font;
            label.fontSize = fontSize;
            label.fontStyle = style;
            label.text = text;
            label.color = new Color(0.96f, 0.9f, 0.78f);
            label.alignment = TextAnchor.UpperLeft;
            label.raycastTarget = false;
            return label;
        }

        private Button ButtonAt(Transform parent, string text, Vector2 position, Vector2 size, Action onClick)
        {
            var go = new GameObject($"Btn_{text}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            go.GetComponent<Image>().color = new Color(0.55f, 0.36f, 0.16f, 0.95f);
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() => onClick());
            var label = Label("Text", go.transform, text, 22, Vector2.zero, size, FontStyle.Normal);
            label.alignment = TextAnchor.MiddleCenter;
            return button;
        }
    }
}
