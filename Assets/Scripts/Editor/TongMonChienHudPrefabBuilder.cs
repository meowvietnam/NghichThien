using System.IO;
using System.Linq;
using AllMaps.TongMonChien;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dựng prefab HUD Tông môn chiến mặc định (TongMonChienHud + TongMonChienPuppetRow) với toàn bộ tham chiếu
/// đã gán sẵn. Sau đó chỉnh sửa trực tiếp trên prefab (sprite, vị trí, font...) — không cần sửa code.
/// </summary>
public static class TongMonChienHudPrefabBuilder
{
    private const string Folder = "Assets/Scripts/AllMaps/TongMonChien/Prefabs";
    private const string RowPath = Folder + "/TongMonChienPuppetRow.prefab";
    private const string FontPath = "Assets/Font/CLOUDLIBIAN-GB-NORMAL.ttf";

    private static readonly Color PanelColor = new Color(0.08f, 0.06f, 0.05f, 0.78f);
    private static readonly Color ButtonColor = new Color(0.55f, 0.36f, 0.16f, 0.95f);
    private static readonly Color TextColor = new Color(0.96f, 0.9f, 0.78f);
    private static readonly Color HpColor = new Color(0.8f, 0.2f, 0.18f);

    private static Font font;

    // Sprite có sẵn của game (cùng bộ với PopupConfirmAction / UIHeadPanel).
    private const string PanelSpritePath = "Assets/Resources02/newui/lianqi/MCS_CJ_ty_bg1.png";
    private const string ButtonSheetPath = "Assets/Resources02/newui/kefang/KeFang.png";
    private const string TabSheetPath = "Assets/Resources02/newui/tab/tabset.png";
    private const string HpBgPath = "Assets/Resources02/newui/fight/Head_HP_BG.png";
    private const string HpFillPath = "Assets/Resources02/newui/fight/Head_HP.png";

    private static Sprite panelSprite, rowSprite, buttonTeal, buttonOrange, hpBgSprite, hpFillSprite;
    private static readonly Color ButtonTextColor = new Color(1f, 0.98f, 0.92f);
    private static readonly Color TitleColor = new Color(0.98f, 0.84f, 0.5f);

    private static Sprite LoadSprite(string path, string name = null) =>
        AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>()
            .FirstOrDefault(x => name == null || x.name == name);

    private static void LoadSprites()
    {
        panelSprite = LoadSprite(PanelSpritePath);
        rowSprite = LoadSprite(TabSheetPath, "MCS_TAB_renwu_bg1.png");
        buttonTeal = LoadSprite(ButtonSheetPath, "MCS_CJ_ty_bt_GN.png");
        buttonOrange = LoadSprite(ButtonSheetPath, "MCS_CJ_ty_bt_RD.png");
        hpBgSprite = LoadSprite(HpBgPath);
        hpFillSprite = LoadSprite(HpFillPath);
    }

    /// <summary>Gán sprite; thiếu sprite thì giữ màu phẳng mặc định.</summary>
    private static void Skin(Image image, Sprite sprite, bool sliced, Color tint)
    {
        if (sprite == null) return;
        image.sprite = sprite;
        image.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
        image.color = tint;
    }

    [MenuItem("Tools/Tông môn chiến/Tạo prefab HUD")]
    public static void BuildMenu()
    {
        if ((File.Exists(TongMonChienController.HudPrefabPath) || File.Exists(RowPath)) &&
            !EditorUtility.DisplayDialog("Tạo prefab HUD",
                "Prefab HUD đã tồn tại. Tạo lại sẽ ghi đè mọi chỉnh sửa trên prefab. Tiếp tục?", "Ghi đè", "Huỷ"))
            return;
        var hud = Build();
        var controller = Object.FindFirstObjectByType<TongMonChienController>(FindObjectsInactive.Include);
        if (controller != null)
        {
            var so = new SerializedObject(controller);
            so.FindProperty("hudPrefab").objectReferenceValue = hud;
            so.ApplyModifiedProperties();
        }

        Selection.activeObject = hud;
        EditorGUIUtility.PingObject(hud);
    }

    /// <summary>Tạo/ghi đè hai prefab và trả về prefab HUD.</summary>
    public static TongMonChienHud Build()
    {
        Directory.CreateDirectory(Folder);
        font = AssetDatabase.LoadAssetAtPath<Font>(FontPath) ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        LoadSprites();
        var row = SavePrefab(BuildRow(), RowPath).GetComponent<TongMonChienPuppetRow>();
        var hud = SavePrefab(BuildHud(row), TongMonChienController.HudPrefabPath).GetComponent<TongMonChienHud>();
        AssetDatabase.SaveAssets();
        Debug.Log($"Đã tạo prefab HUD Tông môn chiến tại {TongMonChienController.HudPrefabPath}");
        return hud;
    }

    private static GameObject SavePrefab(GameObject root, string path)
    {
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    // ------------------------------------------------------------------ HUD

    private static GameObject BuildHud(TongMonChienPuppetRow rowPrefab)
    {
        var root = new GameObject("TongMonChienHud", typeof(RectTransform));
        var canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // Dưới UIManager (101) để popup/loading của game luôn nằm trên HUD.
        canvas.sortingOrder = 100;
        var scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        root.AddComponent<GraphicRaycaster>();
        var hud = root.AddComponent<TongMonChienHud>();

        // Bảng thông tin bản thân (trên trái). HP chiến hiển thị trên UIHead.
        var info = Panel("Info", root.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20), new Vector2(520, 410));
        Draggable(info, "Info");
        Label("Title", info, "TÔNG MÔN CHIẾN", 30, new Vector2(15, -10), new Vector2(490, 40)).color = TitleColor;
        var stateText = Label("State", info, "", 20, new Vector2(15, -54), new Vector2(490, 48));

        // Thưởng hằng ngày: điểm/thành đã chiếm, tổng của tông và số thực nhận theo chức vụ.
        var daily = new GameObject("Daily", typeof(RectTransform));
        daily.transform.SetParent(info, false);
        SetRect((RectTransform)daily.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -104), new Vector2(520, 210));
        Label("Header", daily.transform, "Thưởng tiên ngọc hằng ngày", 22, new Vector2(15, 0), new Vector2(490, 28))
            .color = TitleColor;
        var nodeText = Label("Diem", daily.transform, "Số điểm đã chiếm: 0", 20, new Vector2(15, -32), new Vector2(280, 28));
        var nodeIcon = MoneyIcon("DiemIcon", daily.transform, new Vector2(300, -32));
        var nodeMoney = Label("DiemMoney", daily.transform, "0 /ngày", 20, new Vector2(334, -32), new Vector2(170, 28));
        var strongholdText = Label("Thanh", daily.transform, "Số thành đã chiếm: 0", 20, new Vector2(15, -62), new Vector2(280, 28));
        var strongholdIcon = MoneyIcon("ThanhIcon", daily.transform, new Vector2(300, -62));
        var strongholdMoney = Label("ThanhMoney", daily.transform, "0 /ngày", 20, new Vector2(334, -62), new Vector2(170, 28));
        Label("TongLabel", daily.transform, "Tổng tiên ngọc", 20, new Vector2(15, -92), new Vector2(280, 28));
        var totalIcon = MoneyIcon("TongIcon", daily.transform, new Vector2(300, -92));
        var totalMoney = Label("TongMoney", daily.transform, "0 /ngày", 20, new Vector2(334, -92), new Vector2(170, 28));
        var myLabel = Label("ThucNhanLabel", daily.transform, "Thực nhận (0%)", 20, new Vector2(15, -122), new Vector2(280, 28));
        myLabel.color = TitleColor;
        var myIcon = MoneyIcon("ThucNhanIcon", daily.transform, new Vector2(300, -122));
        var myMoney = Label("ThucNhanMoney", daily.transform, "0 /ngày", 20, new Vector2(334, -122), new Vector2(170, 28));
        myMoney.color = TitleColor;
        var mutedColor = new Color(TextColor.r, TextColor.g, TextColor.b, 0.75f);
        var settlement = Label("PhatThuong", daily.transform, "", 18, new Vector2(15, -154), new Vector2(490, 24));
        settlement.color = mutedColor;
        var season = Label("Mua", daily.transform, "", 18, new Vector2(15, -180), new Vector2(490, 24));
        season.color = mutedColor;

        var useDan = MakeButton("Btn_DungDan", info, "Dùng đan", new Vector2(15, -322), new Vector2(116, 48), true);
        var puppets = MakeButton("Btn_Roi", info, "Rối", new Vector2(139, -322), new Vector2(116, 48));
        var achievement = MakeButton("Btn_ThanhTuu", info, "Thành tựu", new Vector2(263, -322), new Vector2(116, 48));
        var exit = MakeButton("Btn_Thoat", info, "Thoát", new Vector2(387, -322), new Vector2(116, 48));
        DragHint(info);

        // Dòng thông báo (giữa trên).
        var message = Label("Message", root.transform, "", 26, new Vector2(0, -30), new Vector2(900, 50));
        var messageRect = message.rectTransform;
        messageRect.anchorMin = messageRect.anchorMax = new Vector2(0.5f, 1);
        messageRect.pivot = new Vector2(0.5f, 1);
        messageRect.anchoredPosition = new Vector2(0, -30);
        message.alignment = TextAnchor.MiddleCenter;
        message.color = new Color(1f, 0.92f, 0.6f);
        message.gameObject.AddComponent<Outline>();

        // Thanh chọn node khi điều Rối (giữa dưới).
        var deploy = Panel("DeployBar", root.transform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(520, 70));
        Label("Text", deploy, "Chọn node đích cho Rối", 22, new Vector2(15, -10), new Vector2(330, 50))
            .alignment = TextAnchor.MiddleLeft;
        var cancelDeploy = MakeButton("Btn_Huy", deploy, "Huỷ", new Vector2(365, -11), new Vector2(140, 48));

        // Bảng Rối (trên phải).
        var puppetPanel = Panel("Puppets", root.transform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20), new Vector2(760, 560));
        Draggable(puppetPanel, "Puppets");
        var header = Label("Header", puppetPanel, "Rối", 26, new Vector2(15, -10), new Vector2(420, 40));
        header.color = TitleColor;
        var create = MakeButton("Btn_TaoRoi", puppetPanel, "Tạo rối", new Vector2(440, -8), new Vector2(170, 44), true);
        var close = MakeButton("Btn_Dong", puppetPanel, "Đóng", new Vector2(620, -8), new Vector2(125, 44));
        var list = new GameObject("List", typeof(RectTransform), typeof(VerticalLayoutGroup));
        list.transform.SetParent(puppetPanel, false);
        var listRect = (RectTransform)list.transform;
        SetRect(listRect, new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -60), new Vector2(740, 470));
        var layout = list.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 8;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        DragHint(puppetPanel);

        var so = new SerializedObject(hud);
        so.FindProperty("canvas").objectReferenceValue = canvas;
        so.FindProperty("messageText").objectReferenceValue = message;
        so.FindProperty("infoPanel").objectReferenceValue = info;
        so.FindProperty("stateText").objectReferenceValue = stateText;
        so.FindProperty("nodeText").objectReferenceValue = nodeText;
        so.FindProperty("nodeMoneyText").objectReferenceValue = nodeMoney;
        so.FindProperty("strongholdText").objectReferenceValue = strongholdText;
        so.FindProperty("strongholdMoneyText").objectReferenceValue = strongholdMoney;
        so.FindProperty("totalMoneyText").objectReferenceValue = totalMoney;
        so.FindProperty("myRewardLabel").objectReferenceValue = myLabel;
        so.FindProperty("myMoneyText").objectReferenceValue = myMoney;
        so.FindProperty("settlementText").objectReferenceValue = settlement;
        so.FindProperty("seasonText").objectReferenceValue = season;
        var icons = so.FindProperty("moneyVipIcons");
        icons.arraySize = 4;
        icons.GetArrayElementAtIndex(0).objectReferenceValue = nodeIcon;
        icons.GetArrayElementAtIndex(1).objectReferenceValue = strongholdIcon;
        icons.GetArrayElementAtIndex(2).objectReferenceValue = totalIcon;
        icons.GetArrayElementAtIndex(3).objectReferenceValue = myIcon;
        so.FindProperty("useDanButton").objectReferenceValue = useDan;
        so.FindProperty("puppetsButton").objectReferenceValue = puppets;
        so.FindProperty("exitButton").objectReferenceValue = exit;
        so.FindProperty("achievementButton").objectReferenceValue = achievement;
        so.FindProperty("achievementPanelPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<DP.DongPhuAchievementPanel>(DP.AchievementButton.PanelPrefabPath);
        so.FindProperty("puppetPanel").objectReferenceValue = puppetPanel;
        so.FindProperty("puppetHeader").objectReferenceValue = header;
        so.FindProperty("createPuppetButton").objectReferenceValue = create;
        so.FindProperty("closePuppetsButton").objectReferenceValue = close;
        so.FindProperty("puppetList").objectReferenceValue = list.transform;
        so.FindProperty("puppetRowPrefab").objectReferenceValue = rowPrefab;
        so.FindProperty("deployBar").objectReferenceValue = deploy.gameObject;
        so.FindProperty("cancelDeployButton").objectReferenceValue = cancelDeploy;
        so.ApplyModifiedPropertiesWithoutUndo();
        return root;
    }

    // ------------------------------------------------------------------ Hàng Rối

    private static GameObject BuildRow()
    {
        var root = new GameObject("TongMonChienPuppetRow", typeof(RectTransform), typeof(Image));
        var rect = (RectTransform)root.transform;
        rect.sizeDelta = new Vector2(740, 90);
        root.GetComponent<Image>().color = new Color(0.14f, 0.1f, 0.08f, 0.85f);
        Skin(root.GetComponent<Image>(), rowSprite, false, Color.white);
        var row = root.AddComponent<TongMonChienPuppetRow>();
        var name = Label("Name", root.transform, "Rối #1  Luyện Khí Sơ Kì", 22, new Vector2(10, -6), new Vector2(420, 30));
        var bar = Bar("HpBar", root.transform, new Vector2(10, -38), new Vector2(420, 16));
        var info = Label("Info", root.transform, "HP 100/100  |  Độ bền 5/5  |  Node 0  |  Sẵn sàng", 16, new Vector2(10, -58), new Vector2(420, 28));
        var actions = new GameObject("LeaderActions", typeof(RectTransform));
        actions.transform.SetParent(root.transform, false);
        SetRect((RectTransform)actions.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(440, 0), new Vector2(300, 90));
        var upgrade = MakeButton("Btn_Nang", actions.transform, "Nâng", new Vector2(5, -21), new Vector2(90, 48));
        var deploy = MakeButton("Btn_Dieu", actions.transform, "Điều", new Vector2(103, -21), new Vector2(90, 48));
        var pill = MakeButton("Btn_Dan", actions.transform, "Đan", new Vector2(201, -21), new Vector2(90, 48), true);

        var so = new SerializedObject(row);
        so.FindProperty("nameText").objectReferenceValue = name;
        so.FindProperty("infoText").objectReferenceValue = info;
        so.FindProperty("hpBar").objectReferenceValue = bar;
        so.FindProperty("leaderActions").objectReferenceValue = actions;
        so.FindProperty("upgradeButton").objectReferenceValue = upgrade;
        so.FindProperty("deployButton").objectReferenceValue = deploy;
        so.FindProperty("pillButton").objectReferenceValue = pill;
        so.ApplyModifiedPropertiesWithoutUndo();
        return root;
    }

    // ------------------------------------------------------------------ helpers

    private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static RectTransform Panel(string name, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position,
        Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform;
        SetRect(rect, anchor, pivot, position, size);
        var image = go.GetComponent<Image>();
        image.color = PanelColor;
        Skin(image, panelSprite, true, new Color(1, 1, 1, 0.96f));
        return rect;
    }

    private static Text Label(string name, Transform parent, string text, int fontSize, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        SetRect((RectTransform)go.transform, new Vector2(0, 1), new Vector2(0, 1), position, size);
        var label = go.AddComponent<Text>();
        label.font = font;
        label.fontSize = fontSize;
        label.fontStyle = FontStyle.Normal;
        label.text = text;
        label.color = TextColor;
        label.alignment = TextAnchor.UpperLeft;
        label.raycastTarget = false;
        return label;
    }

    private static Button MakeButton(string name, Transform parent, string text, Vector2 position, Vector2 size,
        bool primary = false)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        SetRect((RectTransform)go.transform, new Vector2(0, 1), new Vector2(0, 1), position, size);
        var image = go.GetComponent<Image>();
        image.color = ButtonColor;
        // Cam = hành động chính, xanh ngọc = hành động phụ (giống nút OK / Quay lại của popup).
        Skin(image, primary ? buttonOrange : buttonTeal, false, Color.white);
        var label = Label("Text", go.transform, text, 22, Vector2.zero, size);
        label.alignment = TextAnchor.MiddleCenter;
        label.color = ButtonTextColor;
        return go.GetComponent<Button>();
    }

    /// <summary>Thanh tiến trình chỉ để hiển thị (Slider không tương tác, không có nút kéo).</summary>
    private static Slider Bar(string name, Transform parent, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Slider));
        go.transform.SetParent(parent, false);
        SetRect((RectTransform)go.transform, new Vector2(0, 1), new Vector2(0, 1), position, size);
        var bg = go.GetComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.6f);
        Skin(bg, hpBgSprite, false, Color.white);
        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(go.transform, false);
        var fillAreaRect = (RectTransform)fillArea.transform;
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(2, 2);
        fillAreaRect.offsetMax = new Vector2(-2, -2);
        var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        var fillRect = (RectTransform)fill.transform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;
        var fillImage = fill.GetComponent<Image>();
        fillImage.color = HpColor;
        Skin(fillImage, hpFillSprite, true, Color.white);
        var slider = go.GetComponent<Slider>();
        slider.fillRect = fillRect;
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;
        return slider;
    }

    /// <summary>Icon tiên ngọc lấy từ MoneyResources (Resources/MoneyResources.asset).</summary>
    private static Image MoneyIcon(string name, Transform parent, Vector2 position)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        SetRect((RectTransform)go.transform, new Vector2(0, 1), new Vector2(0, 1), position, new Vector2(28, 28));
        var image = go.GetComponent<Image>();
        image.sprite = MoneyResources.Instance != null ? MoneyResources.Instance.MoneyVipIcon : null;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return image;
    }

    private static void Draggable(RectTransform panel, string key)
    {
        var drag = panel.gameObject.AddComponent<HudDraggablePanel>();
        var so = new SerializedObject(drag);
        so.FindProperty("key").stringValue = key;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void DragHint(RectTransform panel)
    {
        var hint = Label("DragHint", panel, "Kéo nền để di chuyển - nhấp đúp để đặt lại", 14, Vector2.zero,
            new Vector2(320, 20));
        var rect = hint.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(1, 0);
        rect.anchoredPosition = new Vector2(-8, 4);
        hint.alignment = TextAnchor.LowerRight;
        hint.fontStyle = FontStyle.Italic;
        hint.color = new Color(TextColor.r, TextColor.g, TextColor.b, 0.45f);
    }
}
