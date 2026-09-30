using System.IO;
using System.Linq;
using DP;
using Events;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dựng prefab bảng Thành tựu Động phủ theo bộ sprite "Nhiệm vụ" (Resources02/newui/task/new): nền cuốn sách,
/// dải lụa theo trạng thái cho từng hàng, băng tiêu đề đỏ, nút "Trở Về". Hàng thành tựu sao chép
/// ElementMilestone.prefab để dùng lại nút và prefab hiển thị phần thưởng sự kiện, rồi đổi giao diện.
/// Sau khi dựng có thể chỉnh trực tiếp trên prefab.
/// </summary>
public static class DongPhuAchievementPrefabBuilder
{
    private const string SourceElementPath = "Assets/Scripts/Events/Prefabs/ElementMilestone.prefab";
    private const string ElementPath = "Assets/Scripts/DP/Prefabs/DongPhuAchievementElement.prefab";
    private const string SpriteFolder = "Assets/Resources02/newui/task/new/";

    private static readonly Vector2 PanelSize = new Vector2(1680, 836);
    private static readonly Vector2 RowSize = new Vector2(1240, 140);
    private static readonly Color Gold = new Color(1f, 0.87f, 0.55f);
    private static readonly Color Ink = new Color(0.33f, 0.2f, 0.11f);

    private static Font font;

    private static Sprite LoadSprite(string name) =>
        AssetDatabase.LoadAllAssetsAtPath(SpriteFolder + name + ".png").OfType<Sprite>().FirstOrDefault();

    [MenuItem("Tools/Động phủ/Tạo prefab Thành tựu")]
    public static void BuildMenu()
    {
        if ((File.Exists(AchievementButton.PanelPrefabPath) || File.Exists(ElementPath)) &&
            !EditorUtility.DisplayDialog("Tạo prefab Thành tựu",
                "Prefab Thành tựu đã tồn tại. Tạo lại sẽ ghi đè mọi chỉnh sửa trên prefab. Tiếp tục?", "Ghi đè", "Huỷ"))
            return;
        var element = BuildElement();
        var panel = BuildPanel(element);
        AssetDatabase.SaveAssets();

        // Gán vào nút Thành tựu nếu đang mở scene Động phủ.
        var button = Object.FindFirstObjectByType<AchievementButton>(FindObjectsInactive.Include);
        if (button != null)
        {
            var so = new SerializedObject(button);
            so.FindProperty("panelPrefab").objectReferenceValue = panel;
            so.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(button.gameObject.scene);
        }

        Selection.activeObject = panel;
        EditorGUIUtility.PingObject(panel);
        Debug.Log($"Đã tạo {AchievementButton.PanelPrefabPath} và {ElementPath}");
    }

    // ------------------------------------------------------------------ helpers

    private static Object Ref(SerializedObject so, string name) => so.FindProperty(name).objectReferenceValue;

    private static void Set(SerializedObject so, string name, Object value) =>
        so.FindProperty(name).objectReferenceValue = value;

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static RectTransform Node(string name, Transform parent, params System.Type[] components)
    {
        var go = new GameObject(name, new[] { typeof(RectTransform) }.Concat(components).ToArray());
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    private static Image ImageNode(string name, Transform parent, Sprite sprite, bool preserveAspect = false)
    {
        var image = Node(name, parent, typeof(Image)).GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = preserveAspect;
        image.raycastTarget = false;
        if (sprite == null) image.color = new Color(0, 0, 0, 0.35f);
        return image;
    }

    private static Text TextNode(string name, Transform parent, string text, int size, Color color,
        TextAnchor alignment)
    {
        var label = Node(name, parent, typeof(Text)).GetComponent<Text>();
        label.font = font;
        label.fontSize = size;
        label.color = color;
        label.alignment = alignment;
        label.supportRichText = true;
        label.raycastTarget = false;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.text = text;
        return label;
    }

    // ------------------------------------------------------------------ hàng thành tựu

    private static DongPhuAchievementElement BuildElement()
    {
        var root = PrefabUtility.LoadPrefabContents(SourceElementPath);
        try
        {
            root.name = "DongPhuAchievementElement";
            var old = root.GetComponent<ElementMilestone>();
            var src = new SerializedObject(old);
            var moneyVip = Ref(src, "moneyVipPrefab");
            var skillItem = Ref(src, "skillItemPrefab");
            var itemUI = Ref(src, "itemUIPrefab");
            var milestoneContent = (RectTransform)Ref(src, "milestoneContent");
            var content = (RectTransform)Ref(src, "content");
            var button = (Button)Ref(src, "button");
            var buttonText = (Text)Ref(src, "buttonText");
            Object.DestroyImmediate(old);
            font = buttonText != null ? buttonText.font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var inProgress = LoadSprite("mcs_RW_xuan_bl_bg");
            var claimable = LoadSprite("mcs_RW_xuan_or_bg");
            var completed = LoadSprite("mcs_RW_zui_bg");

            // Nền: dải lụa đổi màu theo trạng thái (xanh: đang làm, cam: chờ nhận, lục: hoàn thành).
            var rootRect = (RectTransform)root.transform;
            rootRect.sizeDelta = RowSize;
            var background = root.GetComponent<Image>();
            background.sprite = inProgress;
            background.type = Image.Type.Simple;
            background.preserveAspect = false;
            background.color = Color.white;

            // Khu mục tiêu (vốn là icon vật phẩm sự kiện) → tên + mô tả + tiến độ ở bên trái.
            for (var i = milestoneContent.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(milestoneContent.GetChild(i).gameObject);
            Object.DestroyImmediate(milestoneContent.GetComponent<HorizontalLayoutGroup>());
            SetRect(milestoneContent, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(70, 0), new Vector2(600, 124));
            var title = TextNode("Title", milestoneContent, "<b>Tưới Nước Hộ</b>  <size=22>(mốc 1/5)</size>\n" +
                                                            "<size=22>Tưới nước hộ động phủ đạo hữu 10 lần</size>  0/10",
                28, Color.white, TextAnchor.MiddleLeft);
            SetRect(title.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var outline = title.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.05f, 0.1f, 0.12f, 0.75f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            // Phần thưởng ở giữa-phải, thu nhỏ để vừa dải lụa.
            SetRect(content, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(690, 0), new Vector2(330, 120));
            content.localScale = Vector3.one * 0.8f;
            var rewardLayout = content.GetComponent<HorizontalLayoutGroup>();
            if (rewardLayout != null)
            {
                rewardLayout.childAlignment = TextAnchor.MiddleLeft;
                rewardLayout.spacing = 16;
            }

            // Nút nhận ở bên phải.
            var buttonRect = (RectTransform)button.transform;
            SetRect(buttonRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f),
                new Vector2(-60, 0), new Vector2(210, 76));
            buttonRect.SetAsLastSibling();

            // Dấu "mới" khi có mốc nhận được.
            var badge = ImageNode("NewBadge", root.transform, LoadSprite("mcs_RW_xuan_xin"), true);
            SetRect(badge.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f),
                new Vector2(40, -22), new Vector2(46, 48));
            badge.gameObject.SetActive(false);

            // Dấu hoàn thành thay cho nút khi đã nhận hết mốc.
            var done = Node("CompletedMark", root.transform);
            SetRect(done, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f),
                new Vector2(-60, 0), new Vector2(210, 76));
            var doneIcon = ImageNode("Icon", done, LoadSprite("mcs_RW_zui"), true);
            SetRect(doneIcon.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                Vector2.zero, new Vector2(58, 56));
            var doneText = TextNode("Text", done, "Hoàn thành", 28, new Color(0.8f, 1f, 0.75f), TextAnchor.MiddleLeft);
            SetRect(doneText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(34, 0),
                new Vector2(-68, 0));
            doneText.gameObject.AddComponent<Outline>().effectColor = new Color(0, 0.15f, 0.05f, 0.8f);
            done.gameObject.SetActive(false);

            var element = root.AddComponent<DongPhuAchievementElement>();
            var so = new SerializedObject(element);
            Set(so, "moneyVipPrefab", moneyVip);
            Set(so, "skillItemPrefab", skillItem);
            Set(so, "itemUIPrefab", itemUI);
            Set(so, "titleText", title);
            Set(so, "content", content);
            Set(so, "button", button);
            Set(so, "buttonText", buttonText);
            Set(so, "rowBackground", background);
            Set(so, "inProgressSprite", inProgress);
            Set(so, "claimableSprite", claimable);
            Set(so, "completedSprite", completed);
            Set(so, "claimableBadge", badge.gameObject);
            Set(so, "completedMark", done.gameObject);
            so.ApplyModifiedPropertiesWithoutUndo();
            Directory.CreateDirectory(Path.GetDirectoryName(ElementPath)!);
            return PrefabUtility.SaveAsPrefabAsset(root, ElementPath).GetComponent<DongPhuAchievementElement>();
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    // ------------------------------------------------------------------ bảng

    private static DongPhuAchievementPanel BuildPanel(DongPhuAchievementElement elementPrefab)
    {
        // Nền: cuốn sách "Kỷ sự lục" của bộ Nhiệm vụ; lớp phủ tối phía sau chặn bấm xuyên.
        var root = new GameObject("DongPhuAchievementPanel", typeof(RectTransform));
        try
        {
            var rootRect = (RectTransform)root.transform;
            SetRect(rootRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, PanelSize);

            var dim = ImageNode("Dim", root.transform, null);
            dim.color = new Color(0, 0, 0, 0.55f);
            dim.raycastTarget = true;
            SetRect(dim.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(4000, 3000));
            var book = ImageNode("Book", root.transform, LoadSprite("mcs_RW_shubg"), true);
            SetRect(book.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            book.raycastTarget = true;

            // Băng tiêu đề đỏ.
            var banner = ImageNode("TitleBanner", root.transform, LoadSprite("mcs_RW_biaoti_bg1"));
            SetRect(banner.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f),
                new Vector2(60, -56), new Vector2(460, 78));
            var title = TextNode("Title", banner.transform, "Thành Tựu Động Phủ", 40, Gold, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            title.gameObject.AddComponent<Outline>().effectColor = new Color(0.3f, 0.05f, 0.02f, 0.85f);

            var detail = TextNode("Detail", root.transform, "Đạt mốc để nhận thưởng", 24, Ink, TextAnchor.MiddleCenter);
            SetRect(detail.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f),
                new Vector2(60, -122), new Vector2(1200, 36));
            var line = ImageNode("Line", root.transform, LoadSprite("mcs_RW_biaoti_line"));
            SetRect(line.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f),
                new Vector2(60, -146), new Vector2(1100, 4));

            // Danh sách cuộn trên hai trang sách.
            var scroll = Node("Scroll View", root.transform, typeof(ScrollRect));
            SetRect(scroll, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(60, -160),
                new Vector2(1280, 590));
            var viewport = Node("Viewport", scroll, typeof(Image), typeof(RectMask2D));
            SetRect(viewport, Vector2.zero, Vector2.one, new Vector2(0, 1), Vector2.zero, Vector2.zero);
            var viewportImage = viewport.GetComponent<Image>();
            viewportImage.color = new Color(1, 1, 1, 0);
            var content = Node("Content", viewport, typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            SetRect(content, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, Vector2.zero);
            var layout = content.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(0, 0, 6, 6);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var scrollRect = scroll.GetComponent<ScrollRect>();
            scrollRect.viewport = viewport;
            scrollRect.content = content;
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 30;

            // Nút "Trở Về" (dải xanh + chữ có sẵn).
            var close = Node("CloseButton", root.transform, typeof(Image), typeof(Button));
            SetRect(close, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-40, -24),
                new Vector2(250, 83));
            var closeImage = close.GetComponent<Image>();
            closeImage.sprite = LoadSprite("mcs_RW_fanhui_bg");
            close.GetComponent<Button>().targetGraphic = closeImage;
            var closeLabel = ImageNode("Label", close, LoadSprite("mcs_RW_fanhui3"), true);
            SetRect(closeLabel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(10, 4), new Vector2(150, 50));

            var panel = root.AddComponent<DongPhuAchievementPanel>();
            var so = new SerializedObject(panel);
            Set(so, "closeButton", close.GetComponent<Button>());
            Set(so, "titleText", title);
            Set(so, "detailText", detail);
            Set(so, "content", content);
            Set(so, "elementPrefab", elementPrefab);
            so.ApplyModifiedPropertiesWithoutUndo();
            return PrefabUtility.SaveAsPrefabAsset(root, AchievementButton.PanelPrefabPath)
                .GetComponent<DongPhuAchievementPanel>();
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }
}
