#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Clan.Editor
{
    public static class ClanElementPrefabBuilder
    {
        private const string PrefabPath = "Assets/Scripts/Clan/Prefabs/ClanElement.prefab";
        private const string BodyFontPath = "Assets/Font/CLOUDLIBIAN-GB-NORMAL.ttf";
        private const string TitleFontPath = "Assets/Font/CLOUDSHUIZHU-GB-NORMAL.ttf";

        private static readonly Color Ink = Hex("#302B29");
        private static readonly Color MutedInk = Hex("#675E58");
        private static readonly Color Jade = Hex("#2E6B62");
        private static readonly Color Gold = Hex("#C6A25A");
        private static readonly Color RowBg = Hex("#F3EAD9");
        private static readonly Color AvatarBg = Hex("#FFFFFF");
        private static Font bodyFont;
        private static Font titleFont;

        [MenuItem("Tools/Clan/Build Clan Element (Danh sách tông môn)")]
        public static void Build()
        {
            LoadFonts();

            // Hàng danh sách tông môn: kéo giãn theo chiều rộng, cao cố định — dùng cho
            // ClanListPanel (kết quả tìm kiếm) và bất kỳ nơi nào khác cần liệt kê tông môn.
            var root = Rect("ClanElement", null);
            root.anchorMin = new Vector2(0, 1);
            root.anchorMax = new Vector2(1, 1);
            root.pivot = new Vector2(0.5f, 1f);
            root.sizeDelta = new Vector2(0, 132);
            var rootLayout = root.gameObject.AddComponent<LayoutElement>();
            rootLayout.minHeight = 132;
            rootLayout.preferredHeight = 132;

            var element = root.gameObject.AddComponent<ClanElement>();

            var bg = Image("Background", root, RowBg);
            Stretch(bg.rectTransform);
            bg.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.35f);

            // Toàn bộ hàng là một nút bấm để mở chi tiết tông môn (ClanElement.OnClick -> ShowClanInfo).
            var mainButton = bg.gameObject.AddComponent<Button>();
            var buttonColors = mainButton.colors;
            buttonColors.normalColor = Color.white;
            buttonColors.highlightedColor = new Color(1f, 0.97f, 0.88f, 1f);
            buttonColors.pressedColor = new Color(0.9f, 0.86f, 0.78f, 1f);
            buttonColors.selectedColor = buttonColors.highlightedColor;
            mainButton.colors = buttonColors;

            var clanImage = Image("ClanImage", root, AvatarBg);
            SetLeft(clanImage.rectTransform, 18, 0, 96, 96);
            clanImage.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.7f);

            var clanText = Text("ClanText", root, "Tên tông môn", 26, Ink, TextAnchor.MiddleLeft, true);
            SetLeft(clanText.rectTransform, 134, 24, 460, 34);

            var levelText = Text("LevelText", root, "Cấp 1", 19, Jade, TextAnchor.MiddleLeft);
            SetLeft(levelText.rectTransform, 134, -18, 300, 26);

            var memberCountText = Text("MemberCountText", root, "0/0", 20, MutedInk, TextAnchor.MiddleRight, true);
            SetRight(memberCountText.rectTransform, 24, 0, 160, 32);

            // Lớp phủ "đang tải" khi bấm vào hàng để xem chi tiết — chặn double-click và báo trạng thái.
            var loadingState = Image("LoadingState", root, new Color(0.08f, 0.07f, 0.06f, 0.55f));
            Stretch(loadingState.rectTransform);
            var loadingText = Text("Label", loadingState.transform, "Đang tải...", 20, new Color(0.97f, 0.92f, 0.8f), TextAnchor.MiddleCenter, true);
            Stretch(loadingText.rectTransform);
            loadingState.gameObject.SetActive(false);

            SetObject(element, "mainButton", mainButton);
            SetObject(element, "clanText", clanText);
            SetObject(element, "levelText", levelText);
            SetObject(element, "clanImage", clanImage);
            SetObject(element, "memberCountText", memberCountText);
            SetObject(element, "loadingState", loadingState.gameObject);
            // Lưu ý cần gán tay sau khi kéo prefab này vào scene:
            // - "detailPanel" (BaseClanInfo): panel sẽ hiện chi tiết khi bấm vào hàng — tuỳ nơi
            //   sử dụng (vd. ClanPanel dùng chung), không thể gán sẵn trong prefab dùng lại này.
            // - "defaultClanImage" cố tình để trống — tự lấy từ sprite gán sẵn trên ClanImage
            //   (nếu có) lúc Awake.

            System.IO.Directory.CreateDirectory("Assets/Scripts/Clan/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
            Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
            Debug.Log($"Clan element prefab created: {PrefabPath}");
        }

        private static void LoadFonts()
        {
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>(BodyFontPath);
            titleFont = AssetDatabase.LoadAssetAtPath<Font>(TitleFontPath);
            if (bodyFont == null || titleFont == null)
                throw new System.InvalidOperationException("Không tải được font UI của ClanElement.");
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

        // Neo trái, tâm dọc theo chiều cao parent.
        private static void SetLeft(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
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

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString(value, out var color);
            return color;
        }
    }
}
#endif
