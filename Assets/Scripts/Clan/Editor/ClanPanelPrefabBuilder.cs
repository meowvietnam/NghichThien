#if UNITY_EDITOR
using Auction;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Clan.Editor
{
    public static class ClanPanelPrefabBuilder
    {
        private const string PrefabPath = "Assets/Scripts/Clan/Prefabs/ClanPanel.prefab";
        private const string MemberRowPrefabPath = "Assets/Scripts/Clan/Prefabs/ClanMemberElement_Member.prefab";
        private const string LeadershipCardPrefabPath = "Assets/Scripts/Clan/Prefabs/ClanMemberElement_Leadership.prefab";
        private const string InheritancePanelPrefabPath = "Assets/Scripts/Clan/Prefabs/ClanInheritancePanel.prefab";
        private const string AvatarPrefabPath = "Assets/Scripts/Auction/Prefabs/Player.prefab";
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

        [MenuItem("Tools/Clan/Build Clan Panel Prefab")]
        public static void Build()
        {
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>(BodyFontPath);
            titleFont = AssetDatabase.LoadAssetAtPath<Font>(TitleFontPath);
            if (bodyFont == null || titleFont == null)
                throw new System.InvalidOperationException("Không tải được font UI của ClanPanel.");

            var memberRowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MemberRowPrefabPath);
            var leadershipCardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LeadershipCardPrefabPath);
            var inheritancePanelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(InheritancePanelPrefabPath);
            if (memberRowPrefab == null || leadershipCardPrefab == null || inheritancePanelPrefab == null)
            {
                throw new System.InvalidOperationException(
                    "Thiếu prefab thành viên hoặc ClanInheritancePanel. Hãy build các prefab phụ trước ClanPanel.");
            }

            var root = Rect("ClanPanel", null);
            Stretch(root);
            var panel = root.gameObject.AddComponent<ClanPanel>();
            SetObject(panel, "myTf", root);
            SetObject(panel, "selfObject", root.gameObject);

            var dimmer = Image("Dimmer", root, new Color(0.05f, 0.045f, 0.04f, 0.78f));
            Stretch(dimmer.rectTransform);

            var window = Image("Window", root, Parchment);
            SetRect(window.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1400, 1190));
            window.gameObject.AddComponent<Outline>().effectColor = new Color(0.17f, 0.14f, 0.12f, 0.9f);

            var title = Text("Title", window.transform, "TÔNG MÔN", 36, Ink, TextAnchor.MiddleCenter, true);
            SetRect(title.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -66), new Vector2(760, 60));

            var rule = Image("TitleRule", window.transform, new Color(Gold.r, Gold.g, Gold.b, 0.65f));
            SetTopStretch(rule.rectTransform, -108, 2, 70, 70);

            var close = Button("CloseButton", window.transform, "X", Ink, new Vector2(54, 54), 28);
            SetRect(close.GetComponent<RectTransform>(), Vector2.one, Vector2.one, new Vector2(-34, -34), new Vector2(54, 54));
            close.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);
            SetObject(panel, "closeButton", close);

            // --- Header: ảnh tông môn + số lượng thành viên + cấp độ + quỹ tông môn ---
            var headerRow = Rect("HeaderRow", window.transform);
            SetTopStretch(headerRow, -140, 170);
            var clanImage = Image("ClanImage", headerRow, new Color(1f, 1f, 1f, 1f));
            SetLeft(clanImage.rectTransform, 0, 0, 130, 130);
            clanImage.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.7f);
            var memberCountText = Text("MemberCountText", headerRow, "Thành viên: --/--", 26, MutedInk, TextAnchor.MiddleLeft, true);
            SetLeft(memberCountText.rectTransform, 148, 48, 400, 44);
            var levelText = Text("LevelText", headerRow, "Cấp --", 26, Jade, TextAnchor.MiddleLeft, true);
            SetLeft(levelText.rectTransform, 148, 0, 400, 44);
            var fundsText = Text("FundsText", headerRow, "Quỹ: --", 26, Gold, TextAnchor.MiddleLeft, true);
            SetLeft(fundsText.rectTransform, 148, -48, 400, 44);

            // --- Hàng lãnh đạo: Tông Chủ / Phó Tông Chủ / Trưởng Lão ---
            var leadersRow = Rect("LeadersRow", window.transform);
            SetTop(leadersRow, 0, -330, 768, 320);
            var leadersLayout = leadersRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            leadersLayout.childAlignment = TextAnchor.MiddleCenter;
            leadersLayout.spacing = 24;
            leadersLayout.childForceExpandWidth = false;
            leadersLayout.childForceExpandHeight = false;
            leadersLayout.childControlWidth = true;
            leadersLayout.childControlHeight = true;

            // Thứ tự khớp với Base.BaseClanInfo.Init (OrderByDescending theo Role): Tông Chủ, Phó Tông Chủ, Trưởng Lão.
            var leaderElement0 = InstantiateMemberElement(leadershipCardPrefab, leadersRow, "LeaderSlot_TongChu");
            var leaderElement1 = InstantiateMemberElement(leadershipCardPrefab, leadersRow, "LeaderSlot_PhoTongChu");
            var leaderElement2 = InstantiateMemberElement(leadershipCardPrefab, leadersRow, "LeaderSlot_TruongLao");

            // --- Hàng thao tác: đóng góp (1 lần / 10 lần) + kho tông môn + rời tông môn ---
            // Năm nút thao tác chính, vừa chiều rộng nội dung trên màn hình ngang.
            var actionsRow = Rect("ActionsRow", window.transform);
            SetTop(actionsRow, 0, -670, 1060, 80);
            var actionsLayout = actionsRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            actionsLayout.childAlignment = TextAnchor.MiddleCenter;
            actionsLayout.spacing = 15;
            actionsLayout.childForceExpandWidth = false;
            actionsLayout.childForceExpandHeight = false;
            actionsLayout.childControlWidth = true;
            actionsLayout.childControlHeight = true;

            var contributeOnceButton = Button("ContributeOnceButton", actionsRow, "Đóng Góp x1", Jade, new Vector2(200, 74), 21);
            AddLayoutElement(contributeOnceButton.gameObject, 200, 74);

            var freeContributionBadge = Image("FreeContributionBadge", contributeOnceButton.transform, Gold);
            SetRect(freeContributionBadge.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-6, -6), new Vector2(96, 30));
            freeContributionBadge.raycastTarget = false;
            freeContributionBadge.gameObject.AddComponent<Outline>().effectColor = new Color(0.25f, 0.18f, 0.12f, 0.9f);
            var freeContributionLabel = Text("Label", freeContributionBadge.transform, "Miễn Phí", 16, Ink, TextAnchor.MiddleCenter);
            Stretch(freeContributionLabel.rectTransform);
            freeContributionBadge.gameObject.SetActive(false);

            var contributeTenButton = Button("ContributeTenButton", actionsRow, "Đóng Góp x10", Jade, new Vector2(200, 74), 21);
            AddLayoutElement(contributeTenButton.gameObject, 200, 74);

            var openShopButton = Button("OpenShopButton", actionsRow, "Kho Tông Môn", Gold, new Vector2(200, 74), 21);
            AddLayoutElement(openShopButton.gameObject, 200, 74);

            var openInheritanceButton = Button("OpenInheritanceButton", actionsRow, "Truyền Thừa", Hex("#765B8F"), new Vector2(200, 74), 21);
            AddLayoutElement(openInheritanceButton.gameObject, 200, 74);

            var leaveClanButton = Button("LeaveClanButton", actionsRow, "Rời Tông Môn", Hex("#8C3B33"), new Vector2(200, 74), 21);
            AddLayoutElement(leaveClanButton.gameObject, 200, 74);

            // --- Danh sách Đệ Tử (ScrollRect) ---
            var scrollRoot = Rect("MemberScrollView", window.transform);
            SetTopStretch(scrollRoot, -770, 380);
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
            contentLayout.spacing = 6;
            contentLayout.padding = new RectOffset(6, 6, 6, 6);
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

            var memberElementTemplate = InstantiateMemberElement(memberRowPrefab, content, "ClanMemberElement_Template");

            // --- Popup thao tác thành viên (phong chức / đuổi) khi ấn vào một ClanMemberElement ---
            var memberActionPopup = BuildMemberActionPopup(root);

            // --- Nút "Xin Gia Nhập" — hiện đúng vị trí ActionsRow khi đang xem tông môn của người
            // khác (ActionsRow/clanFeaturesRoot lúc đó đang ẩn nên không đè lên nhau). ---
            var requestJoinButton = Button("RequestJoinButton", window.transform, "Xin Gia Nhập Tông Môn", Jade, new Vector2(320, 74), 26);
            SetTop(requestJoinButton.GetComponent<RectTransform>(), 0, -670, 320, 74);
            requestJoinButton.gameObject.SetActive(false);

            // --- Nút mở popup "Đơn Xin Gia Nhập" — chỉ hiện khi có quyền ClanPermission.AddMember
            // và đang xem tông môn của mình (ClanPanel tự bật/tắt trong RefreshPermissions). ---
            var joinRequestsButton = Button("JoinRequestsButton", headerRow, "Đơn Xin Gia Nhập", Jade, new Vector2(260, 70), 22);
            SetRight(joinRequestsButton.GetComponent<RectTransform>(), 40, 0, 260, 70);
            joinRequestsButton.gameObject.SetActive(false);

            var joinRequestsBadge = Image("Badge", joinRequestsButton.transform, Hex("#8C3B33"));
            SetRect(joinRequestsBadge.rectTransform, Vector2.one, Vector2.one, new Vector2(-6, -6), new Vector2(34, 34));
            joinRequestsBadge.gameObject.AddComponent<Outline>().effectColor = new Color(0.25f, 0.18f, 0.12f, 0.9f);
            var joinRequestsBadgeText = Text("Label", joinRequestsBadge.transform, "0", 18, Color.white, TextAnchor.MiddleCenter, true);
            Stretch(joinRequestsBadgeText.rectTransform);
            joinRequestsBadge.gameObject.SetActive(false);

            // --- Popup danh sách đơn xin gia nhập (duyệt / từ chối) ---
            var joinRequestsPopup = BuildJoinRequestsPopup(root);

            var inheritanceObject = (GameObject)PrefabUtility.InstantiatePrefab(inheritancePanelPrefab, root);
            inheritanceObject.name = "ClanInheritancePanel";
            var inheritancePanel = inheritanceObject.GetComponent<ClanInheritancePanel>();
            inheritanceObject.SetActive(false);

            SetObject(panel, "clanImage", clanImage);
            SetObject(panel, "memberCountText", memberCountText);
            SetObject(panel, "levelText", levelText);
            SetObject(panel, "fundsText", fundsText);
            SetObjectArray(panel, "leaderElements", new Object[] { leaderElement0, leaderElement1, leaderElement2 });
            SetObject(panel, "memberElementPrefab", memberElementTemplate);
            SetObject(panel, "memberContainer", content);
            SetObject(panel, "contributeOnceButton", contributeOnceButton);
            SetObject(panel, "contributeTenButton", contributeTenButton);
            SetObject(panel, "freeContributionBadge", freeContributionBadge.gameObject);
            SetObject(panel, "openShopButton", openShopButton);
            SetObject(panel, "openInheritanceButton", openInheritanceButton);
            SetObject(panel, "inheritancePanel", inheritancePanel);
            SetObject(panel, "leaveClanButton", leaveClanButton);
            SetObject(panel, "memberActionPopup", memberActionPopup);
            // Lưu ý: "clanShop" cố tình để trống — đây là tham chiếu tới instance ClanShop
            // đã đặt sẵn trong scene (giống tiền lệ ClanListPanel.clanPanel), builder đứng
            // độc lập không thể resolve được. Cần gán tay trong Inspector sau khi build.
            // ActionsRow (đóng góp + rời tông môn) là khối tính năng chỉ dành cho thành viên —
            // ClanPanel tự ẩn cả khối này khi IsViewingExternalClan = true (xem tông môn người khác).
            SetObject(panel, "clanFeaturesRoot", actionsRow.gameObject);
            SetObject(panel, "requestJoinButton", requestJoinButton);
            SetObject(panel, "joinRequestsButton", joinRequestsButton);
            SetObject(panel, "joinRequestsBadge", joinRequestsBadge.gameObject);
            SetObject(panel, "joinRequestsBadgeText", joinRequestsBadgeText);
            SetObject(panel, "joinRequestsRoot", joinRequestsPopup.Root);
            SetObject(panel, "joinRequestsCloseButton", joinRequestsPopup.CloseButton);
            SetObject(panel, "joinRequestElementPrefab", joinRequestsPopup.Template);
            SetObject(panel, "joinRequestsContainer", joinRequestsPopup.Container);
            SetObject(panel, "joinRequestsEmptyState", joinRequestsPopup.EmptyState);
            // Lưu ý: "defaultClanImage" cố tình để trống — tự lấy từ sprite gán sẵn trên
            // ClanImage (nếu có) lúc Awake.

            System.IO.Directory.CreateDirectory("Assets/Scripts/Clan/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
            Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
            Debug.Log($"Clan panel prefab created: {PrefabPath}");
        }

        private static ClanMemberElement InstantiateMemberElement(GameObject prefab, Transform parent, string name)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = name;
            var rect = instance.GetComponent<RectTransform>();
            if (rect != null) rect.localScale = Vector3.one;
            return instance.GetComponent<ClanMemberElement>();
        }

        // Popup nhỏ hiện khi ấn vào một ClanMemberElement: phong chức / đuổi thành viên.
        private static ClanMemberActionPopup BuildMemberActionPopup(Transform parentRoot)
        {
            var popupRoot = Rect("MemberActionPopup", parentRoot);
            Stretch(popupRoot);
            var popup = popupRoot.gameObject.AddComponent<ClanMemberActionPopup>();
            SetObject(popup, "selfObject", popupRoot.gameObject);

            var dimmer = Image("Dimmer", popupRoot, new Color(0.05f, 0.045f, 0.04f, 0.6f));
            Stretch(dimmer.rectTransform);

            var card = Image("Card", popupRoot, Parchment);
            SetRect(card.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460, 620));
            card.gameObject.AddComponent<Outline>().effectColor = new Color(0.17f, 0.14f, 0.12f, 0.9f);

            var close = Button("CloseButton", card.transform, "X", Ink, new Vector2(46, 46), 24);
            SetRect(close.GetComponent<RectTransform>(), Vector2.one, Vector2.one, new Vector2(-30, -30), new Vector2(46, 46));
            close.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);
            SetObject(popup, "closeButton", close);

            var avatarSlot = Image("AvatarSlot", card.transform, FieldBg);
            SetTop(avatarSlot.rectTransform, 0, -30, 128, 128);
            avatarSlot.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.6f);
            var avatarGraphic = BuildAvatarDisplay(avatarSlot.rectTransform);
            SetObject(popup, "avatar", avatarGraphic);

            var nameText = Text("NameText", card.transform, "—", 26, Ink, TextAnchor.MiddleCenter, true);
            SetTop(nameText.rectTransform, 0, -172, 400, 40);
            SetObject(popup, "nameText", nameText);

            var roleText = Text("RoleText", card.transform, "—", 20, MutedInk, TextAnchor.MiddleCenter);
            SetTop(roleText.rectTransform, 0, -214, 400, 30);
            SetObject(popup, "roleText", roleText);

            var levelText = Text("LevelText", card.transform, "—", 18, MutedInk, TextAnchor.MiddleCenter);
            SetTop(levelText.rectTransform, 0, -246, 400, 28);
            SetObject(popup, "levelText", levelText);

            var contribText = Text("ContribText", card.transform, "Đóng góp: --", 18, MutedInk, TextAnchor.MiddleCenter);
            SetTop(contribText.rectTransform, 0, -276, 400, 28);
            SetObject(popup, "contribText", contribText);

            var joinAtText = Text("JoinAtText", card.transform, "Gia nhập: --", 18, MutedInk, TextAnchor.MiddleCenter);
            SetTop(joinAtText.rectTransform, 0, -306, 400, 28);
            SetObject(popup, "joinAtText", joinAtText);

            var roleButtonContainer = Rect("RoleButtonContainer", card.transform);
            SetTop(roleButtonContainer, 0, -344, 380, 190);
            var roleLayout = roleButtonContainer.gameObject.AddComponent<VerticalLayoutGroup>();
            roleLayout.spacing = 10;
            roleLayout.childAlignment = TextAnchor.UpperCenter;
            roleLayout.childForceExpandWidth = true;
            roleLayout.childForceExpandHeight = false;
            roleLayout.childControlWidth = true;
            roleLayout.childControlHeight = true;

            // Template ẩn để clone lúc runtime cho từng chức vụ có thể phong (ClanPanel.GetAssignableRoles).
            var roleButtonTemplate = Button("RoleButtonTemplate", roleButtonContainer, "Phong ...", Jade, new Vector2(380, 58), 22);
            AddLayoutElement(roleButtonTemplate.gameObject, 380, 58);
            SetObject(popup, "roleButtonContainer", roleButtonContainer);
            SetObject(popup, "roleButtonTemplate", roleButtonTemplate);

            var kickButton = Button("KickButton", card.transform, "Đuổi Khỏi Tông Môn", Hex("#8C3B33"), new Vector2(380, 60), 22);
            SetTop(kickButton.GetComponent<RectTransform>(), 0, -544, 380, 60);
            SetObject(popup, "kickButton", kickButton);

            popupRoot.gameObject.SetActive(false);
            return popup;
        }

        private readonly struct JoinRequestsPopupRefs
        {
            public readonly GameObject Root;
            public readonly Button CloseButton;
            public readonly RectTransform Container;
            public readonly GameObject EmptyState;
            public readonly ClanJoinRequestElement Template;

            public JoinRequestsPopupRefs(GameObject root, Button closeButton, RectTransform container, GameObject emptyState, ClanJoinRequestElement template)
            {
                Root = root;
                CloseButton = closeButton;
                Container = container;
                EmptyState = emptyState;
                Template = template;
            }
        }

        // Popup danh sách đơn xin gia nhập — thành viên có quyền ClanPermission.AddMember mở bằng
        // JoinRequestsButton (ở HeaderRow) để duyệt/từ chối từng đơn.
        private static JoinRequestsPopupRefs BuildJoinRequestsPopup(Transform parentRoot)
        {
            var popupRoot = Rect("JoinRequestsPopup", parentRoot);
            Stretch(popupRoot);

            var dimmer = Image("Dimmer", popupRoot, new Color(0.05f, 0.045f, 0.04f, 0.6f));
            Stretch(dimmer.rectTransform);

            var card = Image("Card", popupRoot, Parchment);
            SetRect(card.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620, 760));
            card.gameObject.AddComponent<Outline>().effectColor = new Color(0.17f, 0.14f, 0.12f, 0.9f);

            var title = Text("Title", card.transform, "ĐƠN XIN GIA NHẬP", 28, Ink, TextAnchor.MiddleCenter, true);
            SetTop(title.rectTransform, 0, -30, 500, 44);

            var close = Button("CloseButton", card.transform, "X", Ink, new Vector2(46, 46), 24);
            SetRect(close.GetComponent<RectTransform>(), Vector2.one, Vector2.one, new Vector2(-30, -30), new Vector2(46, 46));
            close.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);

            var emptyState = Text("EmptyState", card.transform, "Chưa có đơn xin gia nhập nào.", 20, MutedInk, TextAnchor.MiddleCenter);
            SetTop(emptyState.rectTransform, 0, -140, 480, 60);

            var scrollRoot = Rect("ScrollView", card.transform);
            SetTop(scrollRoot, 0, -96, 560, 600);
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

            var template = BuildJoinRequestElementTemplate(content);

            popupRoot.gameObject.SetActive(false);
            return new JoinRequestsPopupRefs(popupRoot.gameObject, close, content, emptyState.gameObject, template);
        }

        // Một dòng đơn xin gia nhập trong popup: avatar + tên + cảnh giới + nút Duyệt/Từ Chối.
        // Là template được Instantiate lúc runtime (ClanPanel.GetOrCreateJoinRequestElement),
        // không phải prefab asset riêng — giống cách RoleButtonTemplate được dùng trong popup thành viên.
        private static ClanJoinRequestElement BuildJoinRequestElementTemplate(Transform parent)
        {
            var root = Rect("ClanJoinRequestElement_Template", parent);
            root.anchorMin = new Vector2(0, 1);
            root.anchorMax = new Vector2(1, 1);
            root.pivot = new Vector2(0.5f, 1f);
            root.sizeDelta = new Vector2(0, 104);
            var rootLayout = root.gameObject.AddComponent<LayoutElement>();
            rootLayout.minHeight = 104;
            rootLayout.preferredHeight = 104;

            var element = root.gameObject.AddComponent<ClanJoinRequestElement>();

            var bg = Image("Background", root, FieldBg);
            Stretch(bg.rectTransform);
            bg.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.35f);

            var avatarSlot = Image("AvatarSlot", root, new Color(1f, 1f, 1f, 1f));
            SetLeft(avatarSlot.rectTransform, 12, 0, 72, 72);
            avatarSlot.gameObject.AddComponent<Outline>().effectColor = new Color(Gold.r, Gold.g, Gold.b, 0.7f);
            var avatarGlyph = Text("Glyph", avatarSlot.transform, "人", 28, MutedInk, TextAnchor.MiddleCenter, true);
            Stretch(avatarGlyph.rectTransform);
            var avatarGraphic = BuildAvatarDisplay(avatarSlot.rectTransform);
            if (avatarGraphic != null) avatarGlyph.gameObject.SetActive(false);

            var nameText = Text("NameText", root, "Tên nhân vật", 22, Ink, TextAnchor.MiddleLeft, true);
            SetLeft(nameText.rectTransform, 100, 18, 236, 30);

            var levelText = Text("LevelText", root, "Luyện Khí Sơ Kì", 16, MutedInk, TextAnchor.MiddleLeft);
            SetLeft(levelText.rectTransform, 100, -18, 236, 24);

            var approveButton = Button("ApproveButton", root, "Duyệt", Jade, new Vector2(110, 56), 18);
            SetRight(approveButton.GetComponent<RectTransform>(), 132, 0, 110, 56);

            var rejectButton = Button("RejectButton", root, "Từ Chối", Hex("#8C3B33"), new Vector2(110, 56), 18);
            SetRight(rejectButton.GetComponent<RectTransform>(), 16, 0, 110, 56);

            SetObject(element, "nameText", nameText);
            SetObject(element, "levelText", levelText);
            SetObject(element, "avatar", avatarGraphic);
            SetObject(element, "approveButton", approveButton);
            SetObject(element, "rejectButton", rejectButton);

            return element;
        }

        // Dựng avatar thật bằng cách nhúng Auction/Prefabs/Player.prefab — component Auction.Player
        // kế thừa Auction.CharacterGraphic và đã có sẵn "graphic" (SkeletonGraphic Spine) lẫn
        // "skinResources" (asset SkinResources thật) wiring đầy đủ, không cần gán tay gì thêm.
        // Ẩn bớt phần UI riêng của đấu giá (tên/số tiền/trạng thái/highlight) vì popup thành viên
        // đã tự hiển thị các thông tin đó bằng Text riêng.
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

        private static void AddLayoutElement(GameObject go, float width, float height)
        {
            var element = go.GetComponent<LayoutElement>();
            if (element == null) element = go.AddComponent<LayoutElement>();
            element.minWidth = width;
            element.minHeight = height;
            element.preferredWidth = width;
            element.preferredHeight = height;
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

        private static void SetObjectArray(Object target, string propertyName, Object[] values)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(propertyName);
            if (property == null) throw new System.InvalidOperationException($"Missing serialized property {propertyName} on {target.GetType().Name}");
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
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
