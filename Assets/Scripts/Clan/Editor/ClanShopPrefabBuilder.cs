#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Clan.Editor
{
    // Sinh prefab "Kho Tông Môn" (ClanShop) + popup con "Thêm Vào Kho" (ClanBag) từ ClanShop.cs /
    // ClanBag.cs / ClanPopup.cs hiện có — không sửa logic 3 script đó, chỉ dựng UI uGUI và gán
    // đúng serialized field bằng SerializedObject/SerializedProperty.
    public static class ClanShopPrefabBuilder
    {
        private const string PrefabPath = "Assets/Scripts/Clan/Prefabs/ClanShop.prefab";

        // Bag1ItemPrefabInventory (global namespace, Assets/Scripts/AllMaps/Bag/Bag1ItemPrefabInventory.cs)
        // chỉ cần đúng 1 field cấu trúc: "prefabItemBag" (kiểu ItemInventory) — một item mẫu nằm
        // bên trong Content của một ScrollRect bất kỳ ở tổ tiên của nó. Không tự viết lại
        // implementation này — dùng đúng prefab item có sẵn của hệ thống Bag làm template.
        private const string ItemTemplatePrefabPath = "Assets/Scripts/AllMaps/Bag/ItemInventory.prefab";

        // duihuan.png ("兑换" = đổi/trao đổi) — sprite nền dialog có sẵn trong project, đã được
        // ClanIntroPanelPrefabBuilder dùng làm nền cửa sổ theo đúng phong cách này; ở đây dùng làm
        // lớp "Frame" trang trí phủ lên trên nền giấy kem, rất hợp chủ đề "đổi vật phẩm" của Kho Tông Môn.
        private const string FrameSpritePath = "Assets/Resources02/ui icon/duihuan.png";

        private const string BodyFontPath = "Assets/Font/CLOUDLIBIAN-GB-NORMAL.ttf";
        private const string TitleFontPath = "Assets/Font/CLOUDSHUIZHU-GB-NORMAL.ttf";

        private static readonly Color Ink = Hex("#302B29");
        private static readonly Color MutedInk = Hex("#675E58");
        private static readonly Color Jade = Hex("#2E6B62");
        private static readonly Color DarkJade = Hex("#173E3A");
        private static readonly Color Gold = Hex("#C6A25A");
        private static readonly Color Parchment = Hex("#EEE2D5");
        private static readonly Color FieldBg = Hex("#F7F0E4");
        private static readonly Color DoSon = Hex("#8C3B33"); // đỏ son — dùng chung với các nút phá hủy/gỡ khác trong Clan UI.

        private static Font bodyFont;
        private static Font titleFont;

        [MenuItem("Tools/MTS/Clan/Build Clan Shop Prefab")]
        public static void Build()
        {
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>(BodyFontPath);
            titleFont = AssetDatabase.LoadAssetAtPath<Font>(TitleFontPath);
            if (bodyFont == null)
                throw new System.InvalidOperationException($"Không tìm thấy font nội dung tại {BodyFontPath}");
            if (titleFont == null)
                throw new System.InvalidOperationException($"Không tìm thấy font tiêu đề tại {TitleFontPath}");

            var itemTemplatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ItemTemplatePrefabPath);
            if (itemTemplatePrefab == null)
            {
                throw new System.InvalidOperationException(
                    $"Không tìm thấy prefab item mẫu tại {ItemTemplatePrefabPath} — đây là prefab " +
                    "ItemInventory dùng chung của hệ thống túi đồ (Bag1ItemPrefabInventory), bắt buộc phải " +
                    "có sẵn trong project trước khi build ClanShop.");
            }
            if (itemTemplatePrefab.GetComponent<ItemInventory>() == null)
            {
                throw new System.InvalidOperationException(
                    $"{ItemTemplatePrefabPath} thiếu component ItemInventory — không thể dùng làm " +
                    "prefabItemBag cho Bag1ItemPrefabInventory.");
            }

            var frameSprite = AssetDatabase.LoadAssetAtPath<Sprite>(FrameSpritePath);
            if (frameSprite == null)
                Debug.LogWarning($"Không tìm thấy sprite trang trí tại {FrameSpritePath} — dùng màu nền phẳng thay thế.");

            // --- Root: ClanShop ---
            var root = Rect("ClanShop", null);
            Stretch(root);
            var shop = root.gameObject.AddComponent<global::Clan.ClanShop>();
            SetObject(shop, "selfObject", root.gameObject);

            var dimBackground = Image("DimBackground", root, new Color(0.05f, 0.045f, 0.04f, 0.78f));
            Stretch(dimBackground.rectTransform);

            // --- Window (1540x860, giữa màn hình) ---
            var window = Image("Window", root, Parchment);
            SetRect(window.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1540, 860));
            window.gameObject.AddComponent<UnityEngine.UI.Outline>().effectColor = Gold; // viền vàng đồng mảnh
            SetObject(shop, "myTf", window.rectTransform);

            var frame = Image("Frame", window.transform, Parchment);
            Stretch(frame.rectTransform, 6, 6, 6, 6);
            frame.raycastTarget = false;
            if (frameSprite != null)
            {
                frame.sprite = frameSprite;
                frame.type = UnityEngine.UI.Image.Type.Sliced;
                frame.color = new Color(1f, 1f, 1f, 0.9f);
            }

            BuildHeader(window.transform, out var contributionPointsText, out var closeButton, out var refreshButton);
            SetObject(shop, "closeButton", closeButton);
            SetObject(shop, "refreshButton", refreshButton);
            SetObject(shop, "contributionPointsText", contributionPointsText);

            var bodyRow = Rect("Body", window.transform);
            SetTopStretch(bodyRow, -140, 670, 30, 30);
            var bodyLayout = bodyRow.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            bodyLayout.spacing = 40;
            bodyLayout.childAlignment = TextAnchor.UpperLeft;
            bodyLayout.childForceExpandWidth = false;
            bodyLayout.childForceExpandHeight = true;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;

            var shopListPanel = BuildShopListPanel(bodyRow, itemTemplatePrefab,
                out var shopItemList, out var shopEmptyState, out var shopLoadingState);
            SetLayoutWidth(shopListPanel.gameObject, 880);
            SetObject(shop, "itemList", shopItemList);
            SetObject(shop, "emptyState", shopEmptyState);
            SetObject(shop, "loadingState", shopLoadingState);

            var itemDetailPanel = BuildItemDetailPanel(bodyRow,
                out var selectedItemText, out var priceText, out var totalPriceText, out var quantityInput,
                out var buyButton, out var removeButton, out var openBagButton);
            SetLayoutWidth(itemDetailPanel.gameObject, 560);
            SetObject(shop, "selectedItemText", selectedItemText);
            SetObject(shop, "priceText", priceText);
            SetObject(shop, "totalPriceText", totalPriceText);
            SetObject(shop, "quantityInput", quantityInput);
            SetObject(shop, "buyButton", buyButton);
            SetObject(shop, "removeButton", removeButton);
            SetObject(shop, "openBagButton", openBagButton);

            var footerDecoration = Image("FooterDecoration", window.transform, new Color(Gold.r, Gold.g, Gold.b, 0.6f));
            SetTopStretch(footerDecoration.rectTransform, -828, 3, 60, 60);
            footerDecoration.raycastTarget = false;

            // --- ClanBagPopup: popup con "Thêm Vào Kho", mặc định inactive ---
            var clanBag = BuildClanBagPopup(root, itemTemplatePrefab);
            SetObject(shop, "clanBag", clanBag);

            System.IO.Directory.CreateDirectory("Assets/Scripts/Clan/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
            Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
            Debug.Log($"Clan shop prefab created: {PrefabPath}");
        }

        // ------------------------------------------------------------------
        // Header: trang trí trái + tiêu đề + điểm cống hiến + làm mới + đóng.
        // ------------------------------------------------------------------
        private static void BuildHeader(Transform windowTransform,
            out UnityEngine.UI.Text contributionPointsText, out UnityEngine.UI.Button closeButton,
            out UnityEngine.UI.Button refreshButton)
        {
            var header = Rect("Header", windowTransform);
            SetTopStretch(header, -20, 110, 30, 30);

            var decorationLeft = Image("DecorationLeft", header, Gold);
            SetLeft(decorationLeft.rectTransform, 10, 0, 34, 34);
            decorationLeft.raycastTarget = false;
            decorationLeft.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

            var titleText = Text("TitleText", header, "KHO TÔNG MÔN", 34, Ink, TextAnchor.MiddleCenter, true);
            SetRect(titleText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560, 60));

            closeButton = Button("CloseButton", header, "X", Ink, new Vector2(58, 58), 26);
            SetRight(closeButton.GetComponent<RectTransform>(), 0, 0, 58, 58);
            closeButton.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);

            refreshButton = Button("RefreshButton", header, "Làm Mới", Jade, new Vector2(150, 58), 20);
            SetRight(refreshButton.GetComponent<RectTransform>(), 76, 0, 150, 58);

            var contributionBox = Rect("ContributionBox", header);
            SetRight(contributionBox, 244, 0, 300, 92);

            var contributionIcon = Image("ContributionIcon", contributionBox, Gold);
            SetLeft(contributionIcon.rectTransform, 0, 0, 60, 60);
            contributionIcon.raycastTarget = false;

            var contributionLabel = Text("ContributionLabel", contributionBox, "Điểm cống hiến", 18, MutedInk, TextAnchor.MiddleLeft);
            SetLeft(contributionLabel.rectTransform, 74, 22, 226, 28);

            contributionPointsText = Text("ContributionPointsText", contributionBox, "0", 32, Gold, TextAnchor.MiddleLeft, true);
            SetLeft(contributionPointsText.rectTransform, 74, -18, 226, 40);
        }

        // ------------------------------------------------------------------
        // ShopListPanel: danh sách vật phẩm trong kho (Bag1ItemPrefabInventory #1).
        // ------------------------------------------------------------------
        private static RectTransform BuildShopListPanel(Transform parent, GameObject itemTemplatePrefab,
            out global::Bag1ItemPrefabInventory itemList, out GameObject emptyState, out GameObject loadingState)
        {
            var panel = Rect("ShopListPanel", parent);
            var panelBg = panel.gameObject.AddComponent<UnityEngine.UI.Image>();
            panelBg.color = FieldBg;
            panel.gameObject.AddComponent<UnityEngine.UI.Outline>().effectColor = new Color(DarkJade.r, DarkJade.g, DarkJade.b, 0.6f);

            var sectionTitle = Text("SectionTitle", panel, "Vật phẩm trong kho", 24, Jade, TextAnchor.MiddleLeft, true);
            SetTopStretch(sectionTitle.rectTransform, -10, 40, 20, 20);

            var itemListComponent = BuildItemListScrollView("ItemList", panel, itemTemplatePrefab);
            var itemListRect = itemListComponent.GetComponent<RectTransform>();
            Stretch(itemListRect, 16, 16, 58, 12);

            // EmptyState/LoadingState phủ đúng vùng ItemList (không che SectionTitle phía trên).
            var emptyStateGo = Rect("EmptyState", panel);
            Stretch(emptyStateGo, 16, 16, 58, 12);
            var emptyIcon = Image("EmptyIcon", emptyStateGo, new Color(MutedInk.r, MutedInk.g, MutedInk.b, 0.5f));
            SetRect(emptyIcon.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 26), new Vector2(56, 56));
            emptyIcon.raycastTarget = false;
            var emptyText = Text("EmptyText", emptyStateGo, "Kho tông môn chưa có vật phẩm", 20, MutedInk, TextAnchor.MiddleCenter);
            SetRect(emptyText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -36), new Vector2(500, 40));
            emptyStateGo.gameObject.SetActive(false);
            emptyState = emptyStateGo.gameObject;

            var loadingStateGo = Rect("LoadingState", panel);
            Stretch(loadingStateGo, 16, 16, 58, 12);
            var loadingBg = loadingStateGo.gameObject.AddComponent<UnityEngine.UI.Image>();
            loadingBg.color = new Color(Parchment.r, Parchment.g, Parchment.b, 0.7f);
            loadingBg.raycastTarget = false;
            var loadingText = Text("LoadingText", loadingStateGo, "Đang tải kho...", 20, MutedInk, TextAnchor.MiddleCenter);
            Stretch(loadingText.rectTransform);
            loadingStateGo.gameObject.SetActive(false);
            loadingState = loadingStateGo.gameObject;

            itemList = itemListComponent;
            return panel;
        }

        // ------------------------------------------------------------------
        // ItemDetailPanel: thông tin vật phẩm đang chọn + giá + số lượng + thao tác.
        // ------------------------------------------------------------------
        private static RectTransform BuildItemDetailPanel(Transform parent,
            out UnityEngine.UI.Text selectedItemText, out UnityEngine.UI.Text priceText,
            out UnityEngine.UI.Text totalPriceText, out UnityEngine.UI.InputField quantityInput,
            out UnityEngine.UI.Button buyButton, out UnityEngine.UI.Button removeButton,
            out UnityEngine.UI.Button openBagButton)
        {
            var panel = Rect("ItemDetailPanel", parent);
            var panelBg = panel.gameObject.AddComponent<UnityEngine.UI.Image>();
            panelBg.color = FieldBg;
            panel.gameObject.AddComponent<UnityEngine.UI.Outline>().effectColor = new Color(DarkJade.r, DarkJade.g, DarkJade.b, 0.6f);

            var content = Rect("Content", panel);
            Stretch(content, 20, 20, 16, 16);
            var layout = content.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layout.spacing = 14;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;

            var sectionTitle = Text("SectionTitle", content, "Thông tin vật phẩm", 24, Jade, TextAnchor.MiddleLeft, true);
            SetLayoutHeight(sectionTitle.gameObject, 40);

            var selectedItemFrame = Rect("SelectedItemFrame", content);
            SetLayoutHeight(selectedItemFrame.gameObject, 70);
            var selectedBg = selectedItemFrame.gameObject.AddComponent<UnityEngine.UI.Image>();
            selectedBg.color = Parchment;
            selectedItemFrame.gameObject.AddComponent<UnityEngine.UI.Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.7f);
            selectedItemText = Text("SelectedItemText", selectedItemFrame, "Chưa chọn vật phẩm", 22, Ink, TextAnchor.MiddleCenter, true);
            Stretch(selectedItemText.rectTransform, 12, 12, 6, 6);

            priceText = BuildInfoRow(content, "PriceRow", "Đơn giá", "0", out _);
            var quantityRow = Rect("QuantityRow", content);
            SetLayoutHeight(quantityRow.gameObject, 60);
            var quantityLabel = Text("Label", quantityRow, "Số lượng", 20, MutedInk, TextAnchor.MiddleLeft);
            SetLeft(quantityLabel.rectTransform, 0, 0, 200, 44);
            quantityInput = CreateInputField("QuantityInput", quantityRow, "1", 20,
                UnityEngine.UI.InputField.ContentType.IntegerNumber, "1");
            SetRight(quantityInput.GetComponent<RectTransform>(), 0, 0, 220, 52);

            var divider = Image("Divider", content, new Color(Gold.r, Gold.g, Gold.b, 0.6f));
            SetLayoutHeight(divider.gameObject, 3);
            divider.raycastTarget = false;

            totalPriceText = BuildInfoRow(content, "TotalPriceRow", "Tổng cộng", "0", out _);

            buyButton = Button("BuyButton", content, "ĐỔI VẬT PHẨM", Jade, new Vector2(0, 66), 22);
            SetLayoutHeight(buyButton.gameObject, 66);
            SetDisabledTint(buyButton, Jade);

            removeButton = Button("RemoveButton", content, "GỠ KHỎI KHO", DoSon, new Vector2(0, 66), 22);
            SetLayoutHeight(removeButton.gameObject, 66);
            SetDisabledTint(removeButton, DoSon);

            openBagButton = Button("OpenBagButton", content, "THÊM VẬT PHẨM", Gold, new Vector2(0, 66), 22);
            SetLayoutHeight(openBagButton.gameObject, 66);
            SetDisabledTint(openBagButton, Gold);

            return panel;
        }

        // Dòng "Label ... giá trị ... (icon điểm)" dùng chung cho PriceRow/TotalPriceRow.
        private static UnityEngine.UI.Text BuildInfoRow(Transform parent, string rowName, string label, string defaultValue,
            out UnityEngine.UI.Image pointIcon)
        {
            var row = Rect(rowName, parent);
            SetLayoutHeight(row.gameObject, 50);

            var labelText = Text("Label", row, label, 20, MutedInk, TextAnchor.MiddleLeft);
            SetLeft(labelText.rectTransform, 0, 0, 200, 40);

            var valueText = Text(rowName == "PriceRow" ? "PriceText" : "TotalPriceText", row, defaultValue, 22, Ink, TextAnchor.MiddleRight, true);
            SetRight(valueText.rectTransform, 36, 0, 180, 40);

            pointIcon = Image("PointIcon", row, Gold);
            SetRight(pointIcon.rectTransform, 0, 0, 26, 26);
            pointIcon.raycastTarget = false;

            return valueText;
        }

        // ------------------------------------------------------------------
        // ItemList dùng chung cho ClanShop (kho) và ClanBag (túi đồ) — mỗi lần gọi tạo MỘT
        // instance Bag1ItemPrefabInventory độc lập, template item lấy từ ItemInventory.prefab có sẵn.
        // ------------------------------------------------------------------
        private static global::Bag1ItemPrefabInventory BuildItemListScrollView(string name, Transform parent, GameObject itemTemplatePrefab)
        {
            var scrollRoot = Rect(name, parent);
            var scrollBg = scrollRoot.gameObject.AddComponent<UnityEngine.UI.Image>();
            scrollBg.color = FieldBg;
            scrollRoot.gameObject.AddComponent<UnityEngine.UI.Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.4f);
            var scrollRect = scrollRoot.gameObject.AddComponent<UnityEngine.UI.ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 24;

            var viewport = Rect("Viewport", scrollRoot);
            Stretch(viewport, 4, 4, 4, 4);
            viewport.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
            var viewportImage = viewport.gameObject.AddComponent<UnityEngine.UI.Image>();
            viewportImage.color = new Color(1f, 1f, 1f, 0.01f);

            var content = Rect("Content", viewport);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1f);
            content.sizeDelta = new Vector2(0, 0);
            var grid = content.gameObject.AddComponent<UnityEngine.UI.GridLayoutGroup>();
            grid.padding = new RectOffset(15, 15, 20, 20);
            grid.cellSize = new Vector2(100, 100);
            grid.spacing = new Vector2(20, 20);
            grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.Flexible;
            var contentFitter = content.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            contentFitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewport;
            scrollRect.content = content;

            var itemInstance = (GameObject)PrefabUtility.InstantiatePrefab(itemTemplatePrefab, content);
            itemInstance.name = "ItemTemplate";
            var itemInstanceRect = itemInstance.GetComponent<RectTransform>();
            if (itemInstanceRect != null) itemInstanceRect.localScale = Vector3.one;
            var itemInventory = itemInstance.GetComponent<global::ItemInventory>();

            var bagInventory = scrollRoot.gameObject.AddComponent<global::Bag1ItemPrefabInventory>();
            SetObject(bagInventory, "prefabItemBag", itemInventory);

            return bagInventory;
        }

        // ------------------------------------------------------------------
        // ClanBagPopup: popup "Thêm Vào Kho" — chọn vật phẩm trong túi đồ để đăng bán.
        // ------------------------------------------------------------------
        private static global::Clan.ClanBag BuildClanBagPopup(Transform parentRoot, GameObject itemTemplatePrefab)
        {
            var popupRoot = Rect("ClanBagPopup", parentRoot);
            Stretch(popupRoot);
            var clanBag = popupRoot.gameObject.AddComponent<global::Clan.ClanBag>();
            SetObject(clanBag, "selfObject", popupRoot.gameObject);

            var dimBackground = Image("DimBackground", popupRoot, new Color(0.05f, 0.045f, 0.04f, 0.7f));
            Stretch(dimBackground.rectTransform);

            var window = Image("Window", popupRoot, Parchment);
            SetRect(window.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900, 760));
            window.gameObject.AddComponent<UnityEngine.UI.Outline>().effectColor = Gold;
            SetObject(clanBag, "myTf", window.rectTransform);

            var header = Rect("Header", window.transform);
            SetTopStretch(header, -16, 74, 24, 24);
            var titleText = Text("TitleText", header, "THÊM VÀO KHO", 28, Ink, TextAnchor.MiddleCenter, true);
            Stretch(titleText.rectTransform, 70, 70, 0, 0);
            var closeButton = Button("CloseButton", header, "X", Ink, new Vector2(52, 52), 24);
            SetRight(closeButton.GetComponent<RectTransform>(), 0, 0, 52, 52);
            closeButton.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);
            SetObject(clanBag, "closeButton", closeButton);

            var itemListComponent = BuildItemListScrollView("ItemList", window.transform, itemTemplatePrefab);
            var itemListRect = itemListComponent.GetComponent<RectTransform>();
            SetTop(itemListRect, 0, -100, 852, 380);
            SetObject(clanBag, "itemList", itemListComponent);

            var emptyStateGo = Rect("EmptyState", window.transform);
            SetTop(emptyStateGo, 0, -100, 852, 380);
            var emptyText = Text("Text", emptyStateGo, "Túi đồ không có vật phẩm phù hợp", 20, MutedInk, TextAnchor.MiddleCenter);
            Stretch(emptyText.rectTransform);
            emptyStateGo.gameObject.SetActive(false);
            SetObject(clanBag, "emptyState", emptyStateGo.gameObject);

            var selectedItemText = Text("SelectedItemText", window.transform, "Chưa chọn vật phẩm", 22, Ink, TextAnchor.MiddleCenter, true);
            SetTop(selectedItemText.rectTransform, 0, -496, 800, 36);
            SetObject(clanBag, "selectedItemText", selectedItemText);

            var dailyLimitText = Text("DailyLimitText", window.transform, "Slot treo: 0/0", 18, MutedInk, TextAnchor.MiddleCenter);
            SetTop(dailyLimitText.rectTransform, 0, -534, 800, 28);
            SetObject(clanBag, "dailyLimitText", dailyLimitText);

            var quantityRow = Rect("QuantityRow", window.transform);
            SetTop(quantityRow, 0, -578, 800, 56);
            var quantityLabel = Text("Label", quantityRow, "Số lượng", 20, MutedInk, TextAnchor.MiddleLeft);
            SetLeft(quantityLabel.rectTransform, 0, 0, 220, 48);
            var quantityInput = CreateInputField("QuantityInput", quantityRow, "1", 20,
                UnityEngine.UI.InputField.ContentType.IntegerNumber, "1");
            SetRight(quantityInput.GetComponent<RectTransform>(), 0, 0, 300, 48);
            SetObject(clanBag, "quantityInput", quantityInput);

            var addButton = Button("AddButton", window.transform, "ĐƯA VÀO KHO", Jade, new Vector2(400, 70), 24);
            SetTop(addButton.GetComponent<RectTransform>(), 0, -650, 400, 70);
            SetDisabledTint(addButton, Jade);
            SetObject(clanBag, "addButton", addButton);

            popupRoot.gameObject.SetActive(false);
            return clanBag;
        }

        // ------------------------------------------------------------------
        // Helpers dùng chung (bản sao riêng cho file này, theo đúng quy ước các builder khác
        // trong project — dùng kiểu UnityEngine.UI.* đầy đủ để tránh xung đột namespace).
        // ------------------------------------------------------------------
        private static UnityEngine.UI.InputField CreateInputField(string name, Transform parent, string placeholder,
            int fontSize, UnityEngine.UI.InputField.ContentType contentType, string defaultText)
        {
            var rect = Rect(name, parent);
            var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>();
            image.color = FieldBg;
            var inputField = rect.gameObject.AddComponent<UnityEngine.UI.InputField>();

            var placeholderText = Text("Placeholder", rect, placeholder, fontSize, new Color(0.45f, 0.4f, 0.36f, 0.75f), TextAnchor.MiddleLeft);
            Stretch(placeholderText.rectTransform, 16, 16, 6, 6);
            placeholderText.fontStyle = FontStyle.Italic;

            var text = Text("Text", rect, defaultText, fontSize, Ink, TextAnchor.MiddleLeft);
            Stretch(text.rectTransform, 16, 16, 6, 6);

            inputField.textComponent = text;
            inputField.placeholder = placeholderText;
            inputField.targetGraphic = image;
            inputField.contentType = contentType;
            inputField.text = defaultText;

            return inputField;
        }

        private static UnityEngine.UI.Button Button(string name, Transform parent, string label, Color color, Vector2 size, int fontSize)
        {
            var image = Image(name, parent, color);
            image.gameObject.AddComponent<UnityEngine.UI.Outline>().effectColor = new Color(0.25f, 0.18f, 0.12f, 0.95f);
            var button = image.gameObject.AddComponent<UnityEngine.UI.Button>();
            var colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = Color.Lerp(color, new Color(1f, 0.94f, 0.76f), 0.12f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.16f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = Color.Lerp(color, Color.gray, 0.65f) * new Color(1f, 1f, 1f, 0.55f);
            button.colors = colors;
            image.rectTransform.sizeDelta = size;
            var text = Text("Label", image.transform, label, fontSize, new Color(0.98f, 0.91f, 0.75f), TextAnchor.MiddleCenter);
            Stretch(text.rectTransform, 10, 10, 6, 6);
            return button;
        }

        // Làm rõ thêm hiệu ứng mờ khi disabled (ColorTint đã set ở Button(), gọi thêm cho các nút
        // thao tác chính để đảm bảo tương phản rõ ràng theo đúng yêu cầu).
        private static void SetDisabledTint(UnityEngine.UI.Button button, Color baseColor)
        {
            var colors = button.colors;
            colors.disabledColor = Color.Lerp(baseColor, Color.gray, 0.7f) * new Color(1f, 1f, 1f, 0.5f);
            colors.fadeDuration = 0.1f;
            button.colors = colors;
        }

        private static UnityEngine.UI.Image Image(string name, Transform parent, Color color)
        {
            var rect = Rect(name, parent);
            var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>();
            image.color = color;
            return image;
        }

        private static UnityEngine.UI.Text Text(string name, Transform parent, string value, int size, Color color, TextAnchor alignment, bool useTitleFont = false)
        {
            var rect = Rect(name, parent);
            var text = rect.gameObject.AddComponent<UnityEngine.UI.Text>();
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

        // Neo trên, tâm ngang theo chiều rộng parent.
        private static void SetTop(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        // Neo trên, kéo giãn toàn bộ chiều rộng parent (trừ inset trái/phải).
        private static void SetTopStretch(RectTransform rect, float y, float height, float left = 30, float right = 30)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0, y);
            rect.sizeDelta = new Vector2(-(left + right), height);
        }

        private static void SetLayoutWidth(GameObject go, float width)
        {
            var element = go.GetComponent<UnityEngine.UI.LayoutElement>();
            if (element == null) element = go.AddComponent<UnityEngine.UI.LayoutElement>();
            element.minWidth = width;
            element.preferredWidth = width;
        }

        private static void SetLayoutHeight(GameObject go, float height)
        {
            var element = go.GetComponent<UnityEngine.UI.LayoutElement>();
            if (element == null) element = go.AddComponent<UnityEngine.UI.LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString(value, out var color);
            return color;
        }
    }
}
#endif
