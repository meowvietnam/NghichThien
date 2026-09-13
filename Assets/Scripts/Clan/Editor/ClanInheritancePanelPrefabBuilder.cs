#if UNITY_EDITOR
using Network.Models.Clans;
using UnityEditor;
using UnityEngine;

namespace Clan.Editor
{
    public static class ClanInheritancePanelPrefabBuilder
    {
        private const string PrefabPath = "Assets/Scripts/Clan/Prefabs/ClanInheritancePanel.prefab";
        private const string BodyFontPath = "Assets/Font/CLOUDLIBIAN-GB-NORMAL.ttf";
        private const string TitleFontPath = "Assets/Font/CLOUDSHUIZHU-GB-NORMAL.ttf";

        private static readonly Color Ink = Hex("#302B29");
        private static readonly Color MutedInk = Hex("#675E58");
        private static readonly Color Jade = Hex("#2E6B62");
        private static readonly Color Gold = Hex("#C6A25A");
        private static readonly Color Purple = Hex("#765B8F");
        private static readonly Color Parchment = Hex("#EEE2D5");
        private static readonly Color Field = Hex("#F7F0E4");
        private static Font bodyFont;
        private static Font titleFont;

        [MenuItem("Tools/Clan/Build Clan Inheritance Panel Prefab")]
        public static void Build()
        {
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>(BodyFontPath);
            titleFont = AssetDatabase.LoadAssetAtPath<Font>(TitleFontPath);
            if (bodyFont == null || titleFont == null)
                throw new System.InvalidOperationException("Không tải được font cho ClanInheritancePanel.");

            var root = Rect("ClanInheritancePanel", null);
            Stretch(root);
            var panel = root.gameObject.AddComponent<ClanInheritancePanel>();
            SetObject(panel, "myTf", root);
            SetObject(panel, "selfObject", root.gameObject);

            var dimmer = Image("Dimmer", root, new Color(0.035f, 0.03f, 0.045f, 0.82f));
            Stretch(dimmer.rectTransform);

            var window = Image("Window", root, Parchment);
            SetRect(window.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1420, 1160));
            Outline(window.gameObject, Ink, 3);

            var innerBorder = Image("InnerBorder", window.transform, new Color(Gold.r, Gold.g, Gold.b, 0.08f));
            Stretch(innerBorder.rectTransform, 14, 14, 14, 14);
            Outline(innerBorder.gameObject, Gold, 2);
            innerBorder.raycastTarget = false;

            var title = Text("Title", window.transform, "TRUYỀN THỪA TÔNG MÔN", 38, Ink, TextAnchor.MiddleCenter, true);
            SetTop(title.rectTransform, 0, -34, 900, 58);
            var subtitle = Text("Subtitle", window.transform,
                "Đạo thống lưu truyền · Uy lực tăng theo cấp tông môn", 21, MutedInk, TextAnchor.MiddleCenter);
            SetTop(subtitle.rectTransform, 0, -91, 900, 34);

            var close = Button("CloseButton", window.transform, "X", Ink, new Vector2(54, 54), 26);
            SetRect(close.GetComponent<RectTransform>(), Vector2.one, Vector2.one, new Vector2(-36, -36), new Vector2(54, 54));
            close.image.color = Hex("#D8C9B7");
            SetObject(panel, "closeButton", close);

            var info = Image("ProgressInfo", window.transform, Field);
            SetTop(info.rectTransform, 0, -142, 1280, 112);
            Outline(info.gameObject, new Color(Gold.r, Gold.g, Gold.b, 0.65f), 1);

            var levelText = Text("LevelText", info.transform, "Cấp 1", 28, Purple, TextAnchor.MiddleLeft, true);
            SetLeft(levelText.rectTransform, 28, 23, 180, 38);
            var expText = Text("ExpText", info.transform, "EXP 0 / 10,000", 18, MutedInk, TextAnchor.MiddleLeft);
            SetLeft(expText.rectTransform, 28, -22, 290, 30);

            var expTrack = Image("ExpSlider", info.transform, Hex("#D6C9B7"));
            SetLeft(expTrack.rectTransform, 330, 23, 450, 22);
            var slider = expTrack.gameObject.AddComponent<UnityEngine.UI.Slider>();
            slider.interactable = false;
            slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
            var fillArea = Rect("FillArea", expTrack.transform);
            Stretch(fillArea, 3, 3, 3, 3);
            var fill = Image("Fill", fillArea, Purple);
            Stretch(fill.rectTransform);
            slider.fillRect = fill.rectTransform;
            slider.targetGraphic = fill;

            var slotRuleText = Text("SlotRuleText", info.transform, "Đã mở 1 ô mỗi cột", 19, Jade, TextAnchor.MiddleRight, true);
            SetRight(slotRuleText.rectTransform, 28, 23, 420, 34);
            var permissionText = Text("PermissionText", info.transform, "Chỉ Tộc Chủ có thể thay đổi", 17, MutedInk, TextAnchor.MiddleRight);
            SetRight(permissionText.rectTransform, 28, -20, 420, 28);

            var startCard = BuildColumn(window.transform, "StartTurnColumn", "BẮT ĐẦU HIỆP", "Khai trận lập thế", Jade,
                out var startContainer, out var slotTemplate);
            SetTop(startCard.rectTransform, -325, -280, 620, 720);

            var endCard = BuildColumn(window.transform, "EndTurnColumn", "KẾT THÚC HIỆP", "Thu thế lưu vận", Purple,
                out var endContainer, out var endSlotTemplate);
            SetTop(endCard.rectTransform, 325, -280, 620, 720);

            BuildPreviewSlots(startContainer, slotTemplate, ClanInheritanceTrigger.StartTurn);
            BuildPreviewSlots(endContainer, endSlotTemplate, ClanInheritanceTrigger.EndTurn);

            var costHint = Text("CostHintText", window.transform,
                "Điền ô trống miễn phí · Đổi hoặc xóa: 1,000 Tiên Ngọc", 19, MutedInk, TextAnchor.MiddleCenter);
            SetTop(costHint.rectTransform, 0, -1025, 1100, 42);

            var selection = BuildSelection(root, out var selectionTitle, out var selectionHint,
                out var optionContainer, out var optionTemplate, out var clearButton, out var cancelButton);

            SetObject(panel, "levelText", levelText);
            SetObject(panel, "expText", expText);
            SetObject(panel, "expSlider", slider);
            SetObject(panel, "slotRuleText", slotRuleText);
            SetObject(panel, "permissionText", permissionText);
            SetObject(panel, "costHintText", costHint);
            SetObject(panel, "startTurnContainer", startContainer);
            SetObject(panel, "endTurnContainer", endContainer);
            SetObject(panel, "slotTemplate", slotTemplate);
            SetObject(panel, "selectionRoot", selection);
            SetObject(panel, "selectionTitleText", selectionTitle);
            SetObject(panel, "selectionHintText", selectionHint);
            SetObject(panel, "optionContainer", optionContainer);
            SetObject(panel, "optionTemplate", optionTemplate);
            SetObject(panel, "clearButton", clearButton);
            SetObject(panel, "cancelSelectionButton", cancelButton);

            System.IO.Directory.CreateDirectory("Assets/Scripts/Clan/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
            Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
            Debug.Log($"Clan inheritance panel prefab created: {PrefabPath}");
        }

        public static void BuildAndIntegrate()
        {
            Build();
            ClanPanelPrefabBuilder.Build();
        }

        private static UnityEngine.UI.Image BuildColumn(
            Transform parent, string name, string heading, string caption, Color accent,
            out RectTransform content, out ClanInheritanceSlotElement template)
        {
            var card = Image(name, parent, Field);
            Outline(card.gameObject, new Color(accent.r, accent.g, accent.b, 0.75f), 2);
            var strip = Image("Accent", card.transform, accent);
            SetTopStretch(strip.rectTransform, 0, 7, 0, 0);
            var title = Text("Title", card.transform, heading, 28, accent, TextAnchor.MiddleCenter, true);
            SetTop(title.rectTransform, 0, -24, 540, 42);
            var subtitle = Text("Caption", card.transform, caption, 17, MutedInk, TextAnchor.MiddleCenter);
            SetTop(subtitle.rectTransform, 0, -66, 540, 28);

            var scrollRoot = Rect("ScrollView", card.transform);
            SetTop(scrollRoot, 0, -110, 574, 580);
            var scrollImage = scrollRoot.gameObject.AddComponent<UnityEngine.UI.Image>();
            scrollImage.color = new Color(1, 1, 1, 0.12f);
            var scroll = scrollRoot.gameObject.AddComponent<UnityEngine.UI.ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 26;

            var viewport = Rect("Viewport", scrollRoot);
            Stretch(viewport, 4, 4, 4, 4);
            var viewportImage = viewport.gameObject.AddComponent<UnityEngine.UI.Image>();
            viewportImage.color = new Color(1, 1, 1, 0.01f);
            var mask = viewport.gameObject.AddComponent<UnityEngine.UI.Mask>();
            mask.showMaskGraphic = false;

            content = Rect("Content", viewport);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.sizeDelta = Vector2.zero;
            var layout = content.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.spacing = 10;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var fitter = content.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport;
            scroll.content = content;

            template = BuildSlotTemplate(content, accent);
            template.gameObject.SetActive(false);
            return card;
        }

        private static ClanInheritanceSlotElement BuildSlotTemplate(Transform parent, Color accent)
        {
            var root = Image("SlotTemplate", parent, Hex("#E9DDCD"));
            var layout = root.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
            layout.minHeight = 92;
            layout.preferredHeight = 92;
            Outline(root.gameObject, new Color(accent.r, accent.g, accent.b, 0.45f), 1);
            var button = root.gameObject.AddComponent<UnityEngine.UI.Button>();
            var colors = button.colors;
            colors.highlightedColor = new Color(1f, 0.96f, 0.85f, 1f);
            colors.pressedColor = Hex("#D8C8B4");
            colors.disabledColor = Hex("#CFC7BC");
            button.colors = colors;
            var component = root.gameObject.AddComponent<ClanInheritanceSlotElement>();

            var slotText = Text("SlotText", root.transform, "Ô 1", 19, accent, TextAnchor.MiddleLeft, true);
            SetLeft(slotText.rectTransform, 18, 19, 110, 28);
            var effectText = Text("EffectText", root.transform, "Chưa chọn truyền thừa", 20, Ink, TextAnchor.MiddleLeft);
            SetLeft(effectText.rectTransform, 18, -19, 370, 30);
            var valueText = Text("ValueText", root.transform, "+—", 23, Gold, TextAnchor.MiddleRight, true);
            SetRight(valueText.rectTransform, 20, 0, 100, 42);

            var locked = Image("LockedRoot", root.transform, new Color(0.20f, 0.18f, 0.16f, 0.78f));
            Stretch(locked.rectTransform);
            var lockText = Text("LockedText", locked.transform, "Mở ở cấp 2", 20, new Color(0.95f, 0.88f, 0.72f), TextAnchor.MiddleCenter, true);
            Stretch(lockText.rectTransform, 10, 10, 8, 8);

            SetObject(component, "selectButton", button);
            SetObject(component, "slotText", slotText);
            SetObject(component, "effectText", effectText);
            SetObject(component, "valueText", valueText);
            SetObject(component, "lockedRoot", locked.gameObject);
            SetObject(component, "lockedText", lockText);
            return component;
        }

        private static void BuildPreviewSlots(
            Transform parent,
            ClanInheritanceSlotElement template,
            ClanInheritanceTrigger trigger)
        {
            for (var index = 0; index < 5; index++)
            {
                var slot = Object.Instantiate(template, parent);
                slot.name = $"InheritanceSlot_{index + 1}";
                slot.gameObject.SetActive(true);
                slot.Init(trigger, index, null, index == 0, index + 1);
            }
        }

        private static GameObject BuildSelection(
            Transform parent, out UnityEngine.UI.Text title, out UnityEngine.UI.Text hint,
            out RectTransform optionContent, out ClanInheritanceOptionElement optionTemplate,
            out UnityEngine.UI.Button clear, out UnityEngine.UI.Button cancel)
        {
            var root = Rect("SelectionPopup", parent);
            Stretch(root);
            var dimmer = Image("Dimmer", root, new Color(0.03f, 0.025f, 0.04f, 0.72f));
            Stretch(dimmer.rectTransform);
            var card = Image("Card", root, Parchment);
            SetRect(card.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760, 920));
            Outline(card.gameObject, Purple, 3);

            title = Text("Title", card.transform, "Bắt đầu hiệp · Ô 1", 30, Purple, TextAnchor.MiddleCenter, true);
            SetTop(title.rectTransform, 0, -34, 650, 48);
            hint = Text("Hint", card.transform, "Lần chọn đầu tiên cho ô này được miễn phí", 18, MutedInk, TextAnchor.MiddleCenter);
            SetTop(hint.rectTransform, 0, -86, 650, 34);

            var scrollRoot = Rect("ScrollView", card.transform);
            SetTop(scrollRoot, 0, -138, 680, 620);
            var scrollImage = scrollRoot.gameObject.AddComponent<UnityEngine.UI.Image>();
            scrollImage.color = Field;
            Outline(scrollRoot.gameObject, new Color(Gold.r, Gold.g, Gold.b, 0.55f), 1);
            var scroll = scrollRoot.gameObject.AddComponent<UnityEngine.UI.ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 26;
            var viewport = Rect("Viewport", scrollRoot);
            Stretch(viewport, 4, 4, 4, 4);
            var viewportImage = viewport.gameObject.AddComponent<UnityEngine.UI.Image>();
            viewportImage.color = new Color(1, 1, 1, 0.01f);
            var mask = viewport.gameObject.AddComponent<UnityEngine.UI.Mask>();
            mask.showMaskGraphic = false;
            optionContent = Rect("Content", viewport);
            optionContent.anchorMin = new Vector2(0, 1);
            optionContent.anchorMax = new Vector2(1, 1);
            optionContent.pivot = new Vector2(0.5f, 1);
            optionContent.sizeDelta = Vector2.zero;
            var layout = optionContent.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.spacing = 8;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var fitter = optionContent.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport;
            scroll.content = optionContent;
            optionTemplate = BuildOptionTemplate(optionContent);
            optionTemplate.gameObject.SetActive(false);

            clear = Button("ClearButton", card.transform, "Xóa Lựa Chọn", Hex("#8C3B33"), new Vector2(270, 64), 21);
            SetRect(clear.GetComponent<RectTransform>(), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-150, 48), new Vector2(270, 64));
            cancel = Button("CancelButton", card.transform, "Quay Lại", MutedInk, new Vector2(270, 64), 21);
            SetRect(cancel.GetComponent<RectTransform>(), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(150, 48), new Vector2(270, 64));
            root.gameObject.SetActive(false);
            return root.gameObject;
        }

        private static ClanInheritanceOptionElement BuildOptionTemplate(Transform parent)
        {
            var root = Image("OptionTemplate", parent, Hex("#E9DDCD"));
            var layout = root.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
            layout.minHeight = 70;
            layout.preferredHeight = 70;
            Outline(root.gameObject, new Color(Gold.r, Gold.g, Gold.b, 0.45f), 1);
            var button = root.gameObject.AddComponent<UnityEngine.UI.Button>();
            var component = root.gameObject.AddComponent<ClanInheritanceOptionElement>();
            var name = Text("NameText", root.transform, "Kiếm Khí", 22, Ink, TextAnchor.MiddleLeft, true);
            SetLeft(name.rectTransform, 22, 0, 430, 38);
            var value = Text("ValueText", root.transform, "+1", 23, Gold, TextAnchor.MiddleRight, true);
            SetRight(value.rectTransform, 24, 0, 100, 38);
            var selected = Image("SelectedRoot", root.transform, new Color(Purple.r, Purple.g, Purple.b, 0.12f));
            Stretch(selected.rectTransform);
            selected.raycastTarget = false;
            var selectedLabel = Text("Label", selected.transform, "ĐÃ CHỌN", 16, Purple, TextAnchor.MiddleRight, true);
            SetRight(selectedLabel.rectTransform, 130, 0, 120, 30);
            SetObject(component, "selectButton", button);
            SetObject(component, "nameText", name);
            SetObject(component, "valueText", value);
            SetObject(component, "selectedRoot", selected.gameObject);
            return component;
        }

        private static UnityEngine.UI.Button Button(string name, Transform parent, string label, Color color, Vector2 size, int fontSize)
        {
            var image = Image(name, parent, color);
            Outline(image.gameObject, new Color(0.22f, 0.16f, 0.12f, 0.95f), 2);
            var button = image.gameObject.AddComponent<UnityEngine.UI.Button>();
            var colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.16f);
            button.colors = colors;
            image.rectTransform.sizeDelta = size;
            var text = Text("Label", image.transform, label, fontSize, Hex("#F9E9C3"), TextAnchor.MiddleCenter, true);
            Stretch(text.rectTransform, 10, 10, 5, 5);
            return button;
        }

        private static UnityEngine.UI.Image Image(string name, Transform parent, Color color)
        {
            var rect = Rect(name, parent);
            var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>();
            image.color = color;
            return image;
        }

        private static UnityEngine.UI.Text Text(string name, Transform parent, string value, int size, Color color, TextAnchor alignment, bool title = false)
        {
            var rect = Rect(name, parent);
            var text = rect.gameObject.AddComponent<UnityEngine.UI.Text>();
            text.text = value;
            text.font = title ? titleFont : bodyFont;
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
            if (property == null)
                throw new System.InvalidOperationException($"Missing serialized property {propertyName} on {target.GetType().Name}");
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Outline(GameObject target, Color color, float distance)
        {
            var outline = target.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(distance, -distance);
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

        private static void SetLeft(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void SetRight(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(1, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchoredPosition = new Vector2(-x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void SetTop(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void SetTopStretch(RectTransform rect, float y, float height, float left, float right)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
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
