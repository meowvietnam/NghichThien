#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Clan.Editor
{
    public static class ClanListPanelPrefabBuilder
    {
        private const string PrefabPath = "Assets/Scripts/Clan/Prefabs/ClanListPanel.prefab";
        private const string ElementPrefabPath = "Assets/Scripts/Clan/Prefabs/ClanElement.prefab";
        private const string BodyFontPath = "Assets/Font/CLOUDLIBIAN-GB-NORMAL.ttf";
        private const string TitleFontPath = "Assets/Font/CLOUDSHUIZHU-GB-NORMAL.ttf";

        private static readonly Color Ink = Hex("#302B29");
        private static readonly Color MutedInk = Hex("#675E58");
        private static readonly Color Jade = Hex("#2E6B62");
        private static readonly Color Gold = Hex("#C6A25A");
        private static readonly Color Parchment = Hex("#EEE2D5");
        private static readonly Color FieldBg = Hex("#F7F0E4");
        private static Font bodyFont;
        private static Font titleFont;

        [MenuItem("Tools/Clan/Build Clan List Panel Prefab")]
        public static void Build()
        {
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>(BodyFontPath);
            titleFont = AssetDatabase.LoadAssetAtPath<Font>(TitleFontPath);
            if (bodyFont == null || titleFont == null)
                throw new System.InvalidOperationException("Không tải được font UI của ClanListPanel.");

            var elementPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ElementPrefabPath);
            if (elementPrefab == null)
            {
                throw new System.InvalidOperationException(
                    "Chưa có ClanElement.prefab. Hãy chạy Tools/Clan/Build Clan Element (Danh sách tông môn) " +
                    "trước khi build ClanListPanel.");
            }

            var root = Rect("ClanListPanel", null);
            Stretch(root);
            var panel = root.gameObject.AddComponent<ClanListPanel>();
            SetObject(panel, "myTf", root);
            SetObject(panel, "selfObject", root.gameObject);

            var dimmer = Image("Dimmer", root, new Color(0.05f, 0.045f, 0.04f, 0.78f));
            Stretch(dimmer.rectTransform);

            var window = Image("Window", root, Parchment);
            SetRect(window.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900, 1200));
            window.gameObject.AddComponent<Outline>().effectColor = new Color(0.17f, 0.14f, 0.12f, 0.9f);

            var title = Text("Title", window.transform, "DANH SÁCH TÔNG MÔN", 32, Ink, TextAnchor.MiddleCenter, true);
            SetRect(title.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -56), new Vector2(600, 56));

            var rule = Image("TitleRule", window.transform, new Color(Gold.r, Gold.g, Gold.b, 0.65f));
            SetTopStretch(rule.rectTransform, -92, 2, 60, 60);

            var close = Button("CloseButton", window.transform, "X", Ink, new Vector2(54, 54), 28);
            SetRect(close.GetComponent<RectTransform>(), Vector2.one, Vector2.one, new Vector2(-34, -34), new Vector2(54, 54));
            close.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);
            SetObject(panel, "closeButton", close);

            // --- Ô tìm kiếm + nút Tìm Kiếm ---
            var searchRow = Rect("SearchRow", window.transform);
            SetTopStretch(searchRow, -132, 70, 40, 40);

            var searchInput = CreateInputField("SearchInput", searchRow, "Nhập tên tông môn...", 22);
            Stretch(searchInput.GetComponent<RectTransform>(), 0, 166, 0, 0);

            var searchButton = Button("SearchButton", searchRow, "Tìm Kiếm", Jade, new Vector2(150, 70), 22);
            SetRight(searchButton.GetComponent<RectTransform>(), 0, 0, 150, 70);

            // --- Số lượng kết quả ---
            var resultCountText = Text("ResultCountText", window.transform, string.Empty, 18, MutedInk, TextAnchor.MiddleLeft);
            resultCountText.rectTransform.anchorMin = new Vector2(0, 1);
            resultCountText.rectTransform.anchorMax = new Vector2(0, 1);
            resultCountText.rectTransform.pivot = new Vector2(0f, 1f);
            resultCountText.rectTransform.anchoredPosition = new Vector2(40, -212);
            resultCountText.rectTransform.sizeDelta = new Vector2(780, 28);

            // --- Danh sách kết quả (ScrollRect), tái dùng ClanElement.prefab làm template ---
            var scrollRoot = Rect("ClanScrollView", window.transform);
            SetTopStretch(scrollRoot, -252, 888, 40, 40);
            var scrollBg = scrollRoot.gameObject.AddComponent<Image>();
            scrollBg.color = FieldBg;
            scrollRoot.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.4f);
            var scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 24;

            var viewport = Rect("Viewport", scrollRoot);
            Stretch(viewport, 4, 4, 4, 4);
            viewport.gameObject.AddComponent<RectMask2D>();

            var content = Rect("Content", viewport);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1f);
            content.sizeDelta = new Vector2(0, 0);
            var contentLayout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 8;
            contentLayout.padding = new RectOffset(8, 8, 8, 8);
            contentLayout.childAlignment = TextAnchor.UpperCenter;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            var contentFitter = content.gameObject.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewport;
            scrollRect.content = content;

            var elementTemplateInstance = (GameObject)PrefabUtility.InstantiatePrefab(elementPrefab, content);
            elementTemplateInstance.name = "ClanElement_Template";
            var elementTemplateRect = elementTemplateInstance.GetComponent<RectTransform>();
            if (elementTemplateRect != null) elementTemplateRect.localScale = Vector3.one;
            var elementTemplate = elementTemplateInstance.GetComponent<ClanElement>();

            // --- Trạng thái rỗng / đang tải, phủ lên vùng danh sách ---
            var emptyState = Text("EmptyState", scrollRoot, "Không tìm thấy tông môn nào phù hợp.", 20, MutedInk, TextAnchor.MiddleCenter);
            Stretch(emptyState.rectTransform, 40, 40, 40, 40);
            emptyState.gameObject.SetActive(false);

            var loadingState = Image("LoadingState", scrollRoot, new Color(0.08f, 0.07f, 0.06f, 0.5f));
            Stretch(loadingState.rectTransform);
            var loadingLabel = Text("Label", loadingState.transform, "Đang tìm kiếm...", 22, new Color(0.97f, 0.92f, 0.8f), TextAnchor.MiddleCenter, true);
            Stretch(loadingLabel.rectTransform);
            loadingState.gameObject.SetActive(false);

            SetObject(panel, "searchInput", searchInput);
            SetObject(panel, "searchButton", searchButton);
            SetObject(panel, "elementPrefab", elementTemplate);
            SetObject(panel, "listContainer", content);
            SetObject(panel, "resultCountText", resultCountText);
            SetObject(panel, "emptyState", emptyState.gameObject);
            SetObject(panel, "loadingState", loadingState.gameObject);
            // Lưu ý cần gán tay: "clanPanel" — ClanPanel hiển thị tông môn của chính người chơi,
            // dùng chung để (1) xem chi tiết mọi tông môn trong danh sách này và (2) tự chuyển
            // sang hiển thị khi vừa gia nhập/tạo tông môn. Đây là tham chiếu chéo tới prefab/instance
            // ClanPanel đã đặt trong scene của bạn, builder này không tự suy ra được.

            System.IO.Directory.CreateDirectory("Assets/Scripts/Clan/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
            Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
            Debug.Log($"Clan list panel prefab created: {PrefabPath}");
        }

        // Legacy UnityEngine.UI.InputField: cần Image nền (targetGraphic), một Text hiển thị giá trị
        // gõ vào (textComponent) và một Text placeholder — cả hai là con trực tiếp, không cần Viewport.
        private static InputField CreateInputField(string name, Transform parent, string placeholder, int fontSize)
        {
            var rect = Rect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = FieldBg;
            var inputField = rect.gameObject.AddComponent<InputField>();

            var placeholderText = Text("Placeholder", rect, placeholder, fontSize, new Color(0.45f, 0.4f, 0.36f, 0.75f), TextAnchor.MiddleLeft);
            Stretch(placeholderText.rectTransform, 16, 16, 6, 6);
            placeholderText.fontStyle = FontStyle.Italic;

            var text = Text("Text", rect, string.Empty, fontSize, Ink, TextAnchor.MiddleLeft);
            Stretch(text.rectTransform, 16, 16, 6, 6);

            inputField.textComponent = text;
            inputField.placeholder = placeholderText;
            inputField.targetGraphic = image;

            return inputField;
        }

        private static Button Button(string name, Transform parent, string label, Color color, Vector2 size, int fontSize)
        {
            var image = Image(name, parent, color);
            image.gameObject.AddComponent<Outline>().effectColor = new Color(0.25f, 0.18f, 0.12f, 0.95f);
            var button = image.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = Color.Lerp(color, new Color(1f, 0.94f, 0.76f), 0.12f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.16f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            image.rectTransform.sizeDelta = size;
            var text = Text("Label", image.transform, label, fontSize, new Color(0.98f, 0.91f, 0.75f), TextAnchor.MiddleCenter);
            Stretch(text.rectTransform, 12, 12, 6, 6);
            return button;
        }

        private static Image Image(string name, Transform parent, Color color)
        {
            var rect = Rect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text Text(string name, Transform parent, string value, int size, Color color, TextAnchor alignment, bool useTitleFont = false)
        {
            var rect = Rect(name, parent);
            var text = rect.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = useTitleFont ? titleFont : bodyFont;
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            if (parent != null) rect.SetParent(parent, false);
            rect.localScale = Vector3.one;
            return rect;
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(propertyName);
            if (property == null) throw new System.InvalidOperationException($"Missing serialized property {propertyName} on {target.GetType().Name}");
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Stretch(RectTransform rect, float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        // Neo phải, tâm dọc theo chiều cao parent.
        private static void SetRight(RectTransform rect, float xFromRight, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(1, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-xFromRight, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        // Neo trên, kéo giãn toàn bộ chiều rộng parent (trừ inset trái/phải).
        private static void SetTopStretch(RectTransform rect, float y, float height, float left = 60, float right = 60)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0, y);
            rect.sizeDelta = new Vector2(-(left + right), height);
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString(value, out var color);
            return color;
        }
    }
}
#endif
