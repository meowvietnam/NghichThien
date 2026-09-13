#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Clan.Editor
{
    public static class CreateClanPanelPrefabBuilder
    {
        private const string PrefabPath = "Assets/Scripts/Clan/Prefabs/CreateClanPanel.prefab";
        private const string BodyFontPath = "Assets/Font/CLOUDLIBIAN-GB-NORMAL.ttf";
        private const string TitleFontPath = "Assets/Font/CLOUDSHUIZHU-GB-NORMAL.ttf";

        private static readonly Color Ink = Hex("#302B29");
        private static readonly Color MutedInk = Hex("#675E58");
        private static readonly Color Jade = Hex("#2E6B62");
        private static readonly Color Gold = Hex("#C6A25A");
        private static readonly Color Parchment = Hex("#EEE2D5");
        private static readonly Color Red = Hex("#87483D");
        private static readonly Color FieldBg = Hex("#F7F0E4");
        private static Font bodyFont;
        private static Font titleFont;

        [MenuItem("Tools/Clan/Build Create Clan Panel Prefab")]
        public static void Build()
        {
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>(BodyFontPath);
            titleFont = AssetDatabase.LoadAssetAtPath<Font>(TitleFontPath);
            if (bodyFont == null || titleFont == null)
                throw new System.InvalidOperationException("Không tải được font UI của CreateClanPanel.");

            var root = Rect("CreateClanPanel", null);
            Stretch(root);
            var popup = root.gameObject.AddComponent<CreateClanPanel>();
            SetObject(popup, "myTf", root);
            SetObject(popup, "selfObject", root.gameObject);

            var dimmer = Image("Dimmer", root, new Color(0.05f, 0.045f, 0.04f, 0.78f));
            Stretch(dimmer.rectTransform);

            var window = Image("Window", root, Parchment);
            SetRect(window.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1200, 1000));
            window.gameObject.AddComponent<Outline>().effectColor = new Color(0.17f, 0.14f, 0.12f, 0.9f);

            var title = Text("Title", window.transform, "KHỞI TẠO TÔNG MÔN", 36, Ink, TextAnchor.MiddleCenter, true);
            SetRect(title.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -66), new Vector2(760, 60));

            var rule = Image("TitleRule", window.transform, new Color(Gold.r, Gold.g, Gold.b, 0.65f));
            SetRectTopStretch(rule.rectTransform, -108, 2, 70, 70);

            var close = Button("CloseButton", window.transform, "X", Ink, new Vector2(54, 54), 28);
            SetRect(close.GetComponent<RectTransform>(), Vector2.one, Vector2.one, new Vector2(-34, -34), new Vector2(54, 54));
            close.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);
            SetObject(popup, "closeButton", close);

            // --- Form fields ---
            var nameLabel = Text("NameLabel", window.transform, "Tên tông môn", 20, Ink, TextAnchor.LowerLeft, true);
            SetRectTopStretch(nameLabel.rectTransform, -140, 28);
            var nameInput = InputFieldControl("NameInput", window.transform, "Nhập tên tông môn...", -172, 58);

            var descLabel = Text("DescriptionLabel", window.transform, "Giới thiệu tông môn", 20, Ink, TextAnchor.LowerLeft, true);
            SetRectTopStretch(descLabel.rectTransform, -250, 28);
            var descInput = InputFieldControl("DescriptionInput", window.transform, "Giới thiệu về tông môn của bạn...", -282, 130, true);

            var announceLabel = Text("AnnouncementLabel", window.transform, "Thông báo tông môn", 20, Ink, TextAnchor.LowerLeft, true);
            SetRectTopStretch(announceLabel.rectTransform, -428, 28);
            var announceInput = InputFieldControl("AnnouncementInput", window.transform, "Thông báo gửi tới thành viên...", -460, 110, true);

            // Image selection row
            var imageRow = Rect("ImageRow", window.transform);
            SetRectTopStretch(imageRow, -586, 90);
            var imageRowLabel = Text("ImageRowLabel", imageRow, "Ảnh tông môn", 20, Ink, TextAnchor.MiddleLeft, true);
            SetRect(imageRowLabel.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0), new Vector2(160, 44));
            imageRowLabel.rectTransform.pivot = new Vector2(0f, 0.5f);
            var previewImage = Image("PreviewImage", imageRow, new Color(1f, 1f, 1f, 1f));
            SetRect(previewImage.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(176, 0), new Vector2(74, 74));
            previewImage.rectTransform.pivot = new Vector2(0f, 0.5f);
            previewImage.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.7f);
            var pickImageButton = Button("PickImageButton", imageRow, "CHỌN ẢNH", Jade, new Vector2(220, 58), 20);
            SetRect(pickImageButton.GetComponent<RectTransform>(), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(266, 0), new Vector2(220, 58));
            pickImageButton.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
            var imageStatusText = Text("ImageStatusText", imageRow, "Chưa chọn ảnh", 20, MutedInk, TextAnchor.MiddleLeft);
            SetRect(imageStatusText.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(502, 0), new Vector2(560, 44));
            imageStatusText.rectTransform.pivot = new Vector2(0f, 0.5f);

            // Node selection row
            var nodeRow = Rect("NodeRow", window.transform);
            SetRectTopStretch(nodeRow, -692, 58);
            var nodeRowLabel = Text("NodeRowLabel", nodeRow, "Vị trí tông môn", 20, Ink, TextAnchor.MiddleLeft, true);
            SetRect(nodeRowLabel.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0), new Vector2(200, 44));
            nodeRowLabel.rectTransform.pivot = new Vector2(0f, 0.5f);
            var selectedNodeText = Text("SelectedNodeText", nodeRow, "Chưa chọn vị trí", 22, MutedInk, TextAnchor.MiddleLeft);
            SetRect(selectedNodeText.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(220, 0), new Vector2(420, 44));
            selectedNodeText.rectTransform.pivot = new Vector2(0f, 0.5f);
            var selectNodeButton = Button("SelectNodeButton", nodeRow, "CHỌN VỊ TRÍ TRÊN BẢN ĐỒ", Jade, new Vector2(320, 58), 20);
            SetRect(selectNodeButton.GetComponent<RectTransform>(), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-4, 0), new Vector2(320, 58));

            // Bottom action buttons
            var createByMoneyVipButton = Button("CreateByMoneyVipButton", window.transform, "TẠO BẰNG TIÊN NGỌC", Jade, new Vector2(420, 82), 22);
            SetRect(createByMoneyVipButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-220, 60), new Vector2(420, 82));
            var createByVndButton = Button("CreateByVndButton", window.transform, "TẠO BẰNG VND", Red, new Vector2(420, 82), 22);
            SetRect(createByVndButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(220, 60), new Vector2(420, 82));

            // Giá tạo tông môn (điền từ ClanConfig lúc runtime)
            var moneyVipPriceText = Text("MoneyVipPriceText", window.transform, "--", 20, Gold, TextAnchor.MiddleCenter, true);
            SetRect(moneyVipPriceText.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-220, 150), new Vector2(420, 32));
            var vndPriceText = Text("VndPriceText", window.transform, "--", 20, Gold, TextAnchor.MiddleCenter, true);
            SetRect(vndPriceText.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(220, 150), new Vector2(420, 32));
            var vndBonusMoneyVipText = Text("VndBonusMoneyVipText", window.transform, "--", 18, Gold, TextAnchor.MiddleCenter, true);
            SetRect(vndBonusMoneyVipText.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(220, 184), new Vector2(420, 30));

            SetObject(popup, "nameInput", nameInput);
            SetObject(popup, "descriptionInput", descInput);
            SetObject(popup, "announcementInput", announceInput);
            SetObject(popup, "previewImage", previewImage);
            SetObject(popup, "pickImageButton", pickImageButton);
            SetObject(popup, "imageStatusText", imageStatusText);
            SetObject(popup, "selectedNodeText", selectedNodeText);
            SetObject(popup, "selectNodeButton", selectNodeButton);
            SetObject(popup, "createByMoneyVipButton", createByMoneyVipButton);
            SetObject(popup, "createByVndButton", createByVndButton);
            SetObject(popup, "moneyVipPriceText", moneyVipPriceText);
            SetObject(popup, "vndPriceText", vndPriceText);
            SetObject(popup, "vndBonusMoneyVipText", vndBonusMoneyVipText);
            // Lưu ý: "clanPanel" cố tình để trống — hãy gán tham chiếu tới ClanPanel đang dùng
            // trong scene/prefab cha (ví dụ trong ClanIntroPanelPrefabBuilder) sau khi build xong.

            System.IO.Directory.CreateDirectory("Assets/Scripts/Clan/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
            Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
            Debug.Log($"Create clan panel prefab created: {PrefabPath}");
        }

        private static InputField InputFieldControl(string name, Transform parent, string placeholderText, float y, float height, bool multiline = false)
        {
            var root = Rect(name, parent);
            SetRectTopStretch(root, y, height);

            var bg = root.gameObject.AddComponent<Image>();
            bg.color = FieldBg;
            bg.type = UnityEngine.UI.Image.Type.Sliced;
            root.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.55f);

            var inputField = root.gameObject.AddComponent<InputField>();
            inputField.targetGraphic = bg;

            var placeholderAnchor = multiline ? TextAnchor.UpperLeft : TextAnchor.MiddleLeft;
            var placeholder = Text("Placeholder", root, placeholderText, 20, new Color(MutedInk.r, MutedInk.g, MutedInk.b, 0.7f), placeholderAnchor);
            placeholder.fontStyle = FontStyle.Italic;
            Stretch(placeholder.rectTransform, 16, 16, 10, 10);

            var text = Text("Text", root, string.Empty, 20, Ink, placeholderAnchor);
            Stretch(text.rectTransform, 16, 16, 10, 10);

            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.lineType = multiline ? InputField.LineType.MultiLineNewline : InputField.LineType.SingleLine;
            if (multiline)
            {
                text.verticalOverflow = VerticalWrapMode.Overflow;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                placeholder.verticalOverflow = VerticalWrapMode.Overflow;
                placeholder.horizontalOverflow = HorizontalWrapMode.Wrap;
            }

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

        private static void SetRectTopStretch(RectTransform rect, float y, float height, float left = 60, float right = 60)
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
