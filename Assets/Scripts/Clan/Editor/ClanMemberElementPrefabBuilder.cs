#if UNITY_EDITOR
using Auction;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Clan.Editor
{
    public static class ClanMemberElementPrefabBuilder
    {
        private const string MemberPrefabPath = "Assets/Scripts/Clan/Prefabs/ClanMemberElement_Member.prefab";
        private const string LeadershipPrefabPath = "Assets/Scripts/Clan/Prefabs/ClanMemberElement_Leadership.prefab";
        private const string BodyFontPath = "Assets/Font/CLOUDLIBIAN-GB-NORMAL.ttf";
        private const string TitleFontPath = "Assets/Font/CLOUDSHUIZHU-GB-NORMAL.ttf";
        private const string AvatarPrefabPath = "Assets/Scripts/Auction/Prefabs/Player.prefab";

        private static readonly Color Ink = Hex("#302B29");
        private static readonly Color MutedInk = Hex("#675E58");
        private static readonly Color Jade = Hex("#2E6B62");
        private static readonly Color DarkJade = Hex("#173E3A");
        private static readonly Color Gold = Hex("#C6A25A");
        private static readonly Color Parchment = Hex("#EEE2D5");
        private static readonly Color RowBg = Hex("#F3EAD9");
        private static readonly Color AvatarBg = Hex("#FFFFFF");
        private static Font bodyFont;
        private static Font titleFont;

        [MenuItem("Tools/Clan/Build Clan Member Element (Member - Ngang)")]
        public static void BuildMemberRow()
        {
            LoadFonts();

            // Hàng ngang cho danh sách Đệ Tử: kéo giãn theo chiều rộng, cao cố định.
            var root = Rect("ClanMemberElement_Member", null);
            root.anchorMin = new Vector2(0, 1);
            root.anchorMax = new Vector2(1, 1);
            root.pivot = new Vector2(0.5f, 1f);
            root.sizeDelta = new Vector2(0, 104);
            var rootLayout = root.gameObject.AddComponent<LayoutElement>();
            rootLayout.minHeight = 104;
            rootLayout.preferredHeight = 104;

            var element = root.gameObject.AddComponent<ClanMemberElement>();

            var bg = Image("Background", root, RowBg);
            Stretch(bg.rectTransform);
            bg.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.35f);

            var avatarSlot = Image("AvatarSlot", root, AvatarBg);
            SetLeft(avatarSlot.rectTransform, 16, 0, 72, 72);
            avatarSlot.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.7f);
            avatarSlot.gameObject.AddComponent<RectMask2D>();
            var avatarGlyph = Text("Glyph", avatarSlot.transform, "人", 28, MutedInk, TextAnchor.MiddleCenter, true);
            Stretch(avatarGlyph.rectTransform);
            var avatarGraphic = BuildAvatarDisplay(avatarSlot.rectTransform);
            if (avatarGraphic != null) avatarGlyph.gameObject.SetActive(false);

            var levelImage = Image("LevelBadge", root, Gold);
            SetLeft(levelImage.rectTransform, 70, -28, 28, 28);
            levelImage.gameObject.AddComponent<Outline>().effectColor = new Color(0.25f, 0.18f, 0.12f, 0.6f);

            var nameText = Text("NameText", root, "Tên nhân vật", 24, Ink, TextAnchor.MiddleLeft, true);
            SetLeft(nameText.rectTransform, 104, 14, 480, 28);

            var levelText = Text("LevelText", root, "Luyện Khí Sơ Kì", 18, MutedInk, TextAnchor.MiddleLeft);
            SetLeft(levelText.rectTransform, 104, -14, 480, 22);

            var roleText = Text("RoleText", root, "Đệ Tử", 20, MutedInk, TextAnchor.MiddleRight, true);
            SetRight(roleText.rectTransform, 16, 14, 220, 28);

            var contribText = Text("ContribText", root, "0 điểm", 18, Gold, TextAnchor.MiddleRight);
            SetRight(contribText.rectTransform, 16, -14, 220, 22);

            SetObject(element, "nameText", nameText);
            SetObject(element, "levelText", levelText);
            SetObject(element, "levelImage", levelImage);
            SetObject(element, "roleText", roleText);
            SetObject(element, "contribText", contribText);
            SetObject(element, "avatar", avatarGraphic);
            SetObject(element, "avatarFallback", avatarGlyph.gameObject);
            // Lưu ý: "spriteLevels" cố tình để trống — cần các sprite icon cấp bậc thực tế.

            Save(root.gameObject, MemberPrefabPath);
        }

        [MenuItem("Tools/Clan/Build Clan Member Element (Elder-ViceLeader-Leader - Doc)")]
        public static void BuildLeadershipCard()
        {
            LoadFonts();

            // Thẻ dọc dùng chung cho Trưởng Lão / Phó Tộc Chủ / Tộc Chủ.
            var root = Rect("ClanMemberElement_Leadership", null);
            SetRect(root, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(240, 320));
            var rootLayout = root.gameObject.AddComponent<LayoutElement>();
            rootLayout.minWidth = 240;
            rootLayout.preferredWidth = 240;
            rootLayout.minHeight = 320;
            rootLayout.preferredHeight = 320;

            var element = root.gameObject.AddComponent<ClanMemberElement>();

            var bg = Image("Background", root, Parchment);
            Stretch(bg.rectTransform);
            bg.gameObject.AddComponent<Outline>().effectColor = new Color(0.17f, 0.14f, 0.12f, 0.85f);

            var banner = Image("RoleBanner", root, DarkJade);
            SetTopStretch(banner.rectTransform, 0, 40);
            var roleText = Text("RoleText", banner.transform, "TÔNG CHỦ", 20, new Color(0.96f, 0.87f, 0.66f), TextAnchor.MiddleCenter, true);
            Stretch(roleText.rectTransform, 8, 8, 4, 4);

            var avatarSlot = Image("AvatarSlot", root, AvatarBg);
            SetTop(avatarSlot.rectTransform, 0, -56, 160, 160);
            avatarSlot.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.8f);
            avatarSlot.gameObject.AddComponent<RectMask2D>();
            var avatarGlyph = Text("Glyph", avatarSlot.transform, "人", 44, MutedInk, TextAnchor.MiddleCenter, true);
            Stretch(avatarGlyph.rectTransform);
            var avatarGraphic = BuildAvatarDisplay(avatarSlot.rectTransform);
            if (avatarGraphic != null) avatarGlyph.gameObject.SetActive(false);

            var levelImage = Image("LevelBadge", root, Gold);
            SetTop(levelImage.rectTransform, 66, -202, 32, 32);
            levelImage.gameObject.AddComponent<Outline>().effectColor = new Color(0.25f, 0.18f, 0.12f, 0.6f);

            var nameText = Text("NameText", root, "Tên nhân vật", 22, Ink, TextAnchor.MiddleCenter, true);
            SetTop(nameText.rectTransform, 0, -226, 200, 30);

            var levelText = Text("LevelText", root, "Luyện Khí Sơ Kì", 17, MutedInk, TextAnchor.MiddleCenter);
            SetTop(levelText.rectTransform, 0, -258, 200, 24);

            var contribText = Text("ContribText", root, "0 điểm", 17, Gold, TextAnchor.MiddleCenter);
            SetTop(contribText.rectTransform, 0, -286, 200, 24);

            SetObject(element, "nameText", nameText);
            SetObject(element, "levelText", levelText);
            SetObject(element, "levelImage", levelImage);
            SetObject(element, "roleText", roleText);
            SetObject(element, "contribText", contribText);
            SetObject(element, "avatar", avatarGraphic);
            SetObject(element, "avatarFallback", avatarGlyph.gameObject);
            // Lưu ý: "spriteLevels" cố tình để trống, tương tự bản Member.

            Save(root.gameObject, LeadershipPrefabPath);
        }

        // Dựng avatar thật bằng cách nhúng Auction/Prefabs/Player.prefab — component Auction.Player
        // kế thừa Auction.CharacterGraphic và đã có sẵn "graphic" (SkeletonGraphic Spine) lẫn
        // "skinResources" (asset SkinResources thật) wiring đầy đủ, không cần gán tay gì thêm.
        // Ẩn bớt phần UI riêng của đấu giá (tên/số tiền/trạng thái/highlight) vì element này đã
        // tự hiển thị các thông tin đó bằng Text riêng.
        private static CharacterGraphic BuildAvatarDisplay(RectTransform avatarSlot)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AvatarPrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Không tìm thấy {AvatarPrefabPath} để dựng avatar — bỏ qua, cần gán tay.");
                return null;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, avatarSlot);
            instance.name = "Avatar";
            var player = instance.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogWarning($"{AvatarPrefabPath} thiếu component Auction.Player — bỏ qua, cần gán tay.");
                return null;
            }

            var serialized = new SerializedObject(player);
            HideField(serialized, "nameText");
            HideField(serialized, "amountObject");
            HideField(serialized, "statusText");
            HideField(serialized, "statusImage");
            HideField(serialized, "highlight");

            var rect = instance.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            // Player.prefab dựng cho thẻ đấu giá 279x120 — thu nhỏ/phóng vừa khung avatar này.
            var scale = Mathf.Min(avatarSlot.rect.width / 279f, avatarSlot.rect.height / 120f);
            rect.localScale = new Vector3(scale, scale, rect.localScale.z);

            return player;
        }

        private static void HideField(SerializedObject serialized, string propertyName) => SetFieldActive(serialized, propertyName, false);

        private static void SetFieldActive(SerializedObject serialized, string propertyName, bool active)
        {
            var property = serialized.FindProperty(propertyName);
            var reference = property?.objectReferenceValue;
            var go = reference as GameObject;
            if (go == null && reference is Component component) go = component.gameObject;
            if (go != null) go.SetActive(active);
        }

        private static void LoadFonts()
        {
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>(BodyFontPath);
            titleFont = AssetDatabase.LoadAssetAtPath<Font>(TitleFontPath);
            if (bodyFont == null || titleFont == null)
                throw new System.InvalidOperationException("Không tải được font UI của ClanMemberElement.");
        }

        private static void Save(GameObject root, string path)
        {
            System.IO.Directory.CreateDirectory("Assets/Scripts/Clan/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            Debug.Log($"Clan member element prefab created: {path}");
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

        // Neo trái, tâm dọc theo chiều cao parent (dùng cho hàng ngang).
        private static void SetLeft(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        // Neo phải, tâm dọc theo chiều cao parent (dùng cho hàng ngang).
        private static void SetRight(RectTransform rect, float xFromRight, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(1, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-xFromRight, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        // Neo trên, tâm ngang theo chiều rộng parent (dùng cho thẻ dọc).
        private static void SetTop(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        // Neo trên, kéo giãn toàn bộ chiều rộng parent (dùng cho banner vai trò).
        private static void SetTopStretch(RectTransform rect, float y, float height)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0, y);
            rect.sizeDelta = new Vector2(0, height);
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString(value, out var color);
            return color;
        }
    }
}
#endif
