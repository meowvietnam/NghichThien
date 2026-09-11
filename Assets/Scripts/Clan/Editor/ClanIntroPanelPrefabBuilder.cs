#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Clan.Editor
{
    public static class ClanIntroPanelPrefabBuilder
    {
        private const string PrefabPath = "Assets/Scripts/Clan/Prefabs/ClanIntroPanel.prefab";
        private const string ArtPath = "Assets/Resources02/ui icon/clan/clan_intro_mountain_sect.png";
        private const string CreateButtonPath = "Assets/Resources02/ui icon/clan/buttons/clan_button_create.png";
        private const string ListButtonPath = "Assets/Resources02/ui icon/clan/buttons/clan_button_list.png";
        private const string BackgroundPath = "Assets/Resources02/ui icon/duihuan.png";
        private const string BodyFontPath = "Assets/Font/CLOUDLIBIAN-GB-NORMAL.ttf";
        private const string TitleFontPath = "Assets/Font/CLOUDSHUIZHU-GB-NORMAL.ttf";

        private static readonly Color Ink = Hex("#302B29");
        private static readonly Color MutedInk = Hex("#675E58");
        private static readonly Color Jade = Hex("#2E6B62");
        private static readonly Color DarkJade = Hex("#173E3A");
        private static readonly Color Gold = Hex("#C6A25A");
        private static readonly Color Parchment = Hex("#EEE2D5");
        private static readonly Color Red = Hex("#87483D");
        private static Font bodyFont;
        private static Font titleFont;
        private static Sprite createButtonSprite;
        private static Sprite listButtonSprite;

        [MenuItem("Tools/Clan/Build Clan Intro Panel Prefab")]
        public static void Build()
        {
            PrepareArtwork();
            PrepareButtonTexture(CreateButtonPath);
            PrepareButtonTexture(ListButtonPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>(BodyFontPath);
            titleFont = AssetDatabase.LoadAssetAtPath<Font>(TitleFontPath);
            createButtonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(CreateButtonPath);
            listButtonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ListButtonPath);
            if (bodyFont == null || titleFont == null)
                throw new System.InvalidOperationException("Không tải được font UI của ClanIntroPanel.");
            if (createButtonSprite == null || listButtonSprite == null)
                throw new System.InvalidOperationException("Không tải được sprite button của ClanIntroPanel.");

            var root = Rect("ClanIntroPanel", null);
            Stretch(root);

            var intro = CreateIntroPanel(root.transform);
            var create = CreateLinkedPanel<CreateClanPanel>(root.transform, "CreateClanPanel", "KHỞI TẠO TÔNG MÔN");
            var list = CreateLinkedPanel<ClanListPanel>(root.transform, "ClanListPanel", "DANH SÁCH TÔNG MÔN");
            var clan = CreateLinkedPanel<ClanPanel>(root.transform, "ClanPanel", "TÔNG MÔN");

            SetObject(create, "clanPanel", clan);
            SetObject(list, "clanPanel", clan);
            SetObject(intro, "createClanPanel", create);
            SetObject(intro, "clanListPanel", list);
            SetObject(intro, "clanPanel", clan);

            create.gameObject.SetActive(false);
            list.gameObject.SetActive(false);
            clan.gameObject.SetActive(false);

            System.IO.Directory.CreateDirectory("Assets/Scripts/Clan/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
            Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
            Debug.Log($"Clan intro prefab created: {PrefabPath}");
        }

        public static void BuildAndRenderPreview()
        {
            Build();
            var cameraObject = new GameObject("ClanIntroPreviewCamera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.09f, 0.08f, 0.075f, 1f);

            var canvasObject = new GameObject("ClanIntroPreviewCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1f;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvasObject.transform);
            Stretch(instance.GetComponent<RectTransform>());

            var target = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            var screenshot = new Texture2D(1920, 1080, TextureFormat.RGBA32, false);
            screenshot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
            screenshot.Apply();
            System.IO.File.WriteAllBytes("/tmp/ClanIntroPanelPreview.png", screenshot.EncodeToPNG());

            RenderTexture.active = null;
            camera.targetTexture = null;
            Object.DestroyImmediate(screenshot);
            Object.DestroyImmediate(target);
            Object.DestroyImmediate(canvasObject);
            Object.DestroyImmediate(cameraObject);
            Debug.Log("Clan intro preview rendered: /tmp/ClanIntroPanelPreview.png");
        }

        private static ClanIntroPanel CreateIntroPanel(Transform parent)
        {
            var panel = Rect("Intro", parent);
            Stretch(panel);
            var popup = panel.gameObject.AddComponent<ClanIntroPanel>();
            SetPopupBase(popup, panel);

            var dimmer = Image("Dimmer", panel.transform, new Color(0.05f, 0.045f, 0.04f, 0.78f));
            Stretch(dimmer.rectTransform);

            var window = Image("Window", panel.transform, Parchment);
            SetRect(window.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1510, 840));
            var bg = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
            if (bg != null) window.sprite = bg;
            window.type = UnityEngine.UI.Image.Type.Sliced;
            window.gameObject.AddComponent<Outline>().effectColor = new Color(0.17f, 0.14f, 0.12f, 0.9f);

            AddDecorativeFrame(window.transform);
            AddTitle(window.transform);
            AddArtwork(window.transform);
            AddBenefits(window.transform);

            var close = Button("CloseButton", window.transform, "X", Ink, new Vector2(54, 54), 28);
            SetRect(close.GetComponent<RectTransform>(), Vector2.one, Vector2.one, new Vector2(-34, -34), new Vector2(54, 54));
            close.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);
            SetObject(popup, "closeButton", close);

            var listButton = Button("ClanListButton", window.transform, "DANH SÁCH TÔNG MÔN", Jade, new Vector2(350, 82), 24, listButtonSprite, new Color(0.94f, 0.88f, 0.80f));
            SetRect(listButton.GetComponent<RectTransform>(), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-405, 78), new Vector2(350, 82));
            var createButton = Button("CreateClanButton", window.transform, "KHỞI TẠO TÔNG MÔN", Red, new Vector2(350, 82), 24, createButtonSprite, Ink);
            SetRect(createButton.GetComponent<RectTransform>(), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-405, 176), new Vector2(350, 82));
            SetObject(popup, "clanListButton", listButton);
            SetObject(popup, "createClanButton", createButton);

            return popup;
        }

        private static void AddTitle(Transform parent)
        {
            var ornament = Image("TitleOrnament", parent, DarkJade);
            SetRect(ornament.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -43), new Vector2(560, 66));
            ornament.gameObject.AddComponent<Outline>().effectColor = Gold;
            ornament.gameObject.GetComponent<Outline>().effectDistance = new Vector2(2, -2);
            var title = Text("Title", ornament.transform, "TÔNG MÔN", 36, new Color(0.96f, 0.87f, 0.66f), TextAnchor.MiddleCenter, true);
            Stretch(title.rectTransform, 15, 15, 5, 5);

            var subtitle = Text("Subtitle", parent, "KHAI TÔNG  •  LẬP PHÁI", 16, MutedInk, TextAnchor.MiddleCenter);
            SetRect(subtitle.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-405, -102), new Vector2(430, 34));
        }

        private static void AddArtwork(Transform parent)
        {
            var art = Image("MountainSectArtwork", parent, Color.white);
            art.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath);
            art.preserveAspect = true;
            art.raycastTarget = false;
            SetRect(art.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(390, -12), new Vector2(800, 690));

            var seal = Image("ClanSeal", parent, new Color(0.25f, 0.12f, 0.10f, 0.92f));
            SetRect(seal.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(96, 92), new Vector2(86, 86));
            seal.gameObject.AddComponent<Outline>().effectColor = Gold;
            var sealText = Text("SealText", seal.transform, "TÔNG", 18, new Color(0.93f, 0.80f, 0.55f), TextAnchor.MiddleCenter, true);
            Stretch(sealText.rectTransform, 5, 5, 5, 5);
        }

        private static void AddBenefits(Transform parent)
        {
            var content = Rect("Benefits", parent);
            SetRect(content, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-405, -340), new Vector2(430, 390));

            var heading = Text("Heading", content, "LỢI ÍCH TÔNG MÔN", 25, Ink, TextAnchor.MiddleLeft, true);
            SetRect(heading.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -22), new Vector2(0, 44));
            AddRule(content, -52);

            AddBenefit(content, 0, "ĐỒNG HÀNH TU LUYỆN", "Kết nối đạo hữu, cùng phát triển thế lực.", "Đ");
            AddBenefit(content, 1, "KHO TÔNG MÔN", "Dùng điểm cống hiến đổi vật phẩm do thành viên chia sẻ.", "K");
            AddBenefit(content, 2, "TRUYỀN THỪA", "Nhận kỹ năng hỗ trợ khi bắt đầu trận.", "T");
            AddBenefit(content, 3, "THÀNH TRÌ RIÊNG", "Xây tông môn tại một linh địa trên bản đồ.", "C");
        }

        private static void AddBenefit(RectTransform parent, int index, string title, string description, string glyph)
        {
            float y = -92 - index * 76;
            var icon = Image($"Benefit{index + 1}Icon", parent, index % 2 == 0 ? Jade : Red);
            SetRect(icon.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, y), new Vector2(54, 54));
            icon.gameObject.AddComponent<Outline>().effectColor = Gold;
            var glyphText = Text("Glyph", icon.transform, glyph, 25, new Color(0.96f, 0.88f, 0.69f), TextAnchor.MiddleCenter, true);
            Stretch(glyphText.rectTransform, 2, 2, 2, 2);

            var titleText = Text($"Benefit{index + 1}Title", parent, title, 20, Ink, TextAnchor.MiddleLeft);
            SetRect(titleText.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(88, y + 10), new Vector2(-88, 28));
            var descText = Text($"Benefit{index + 1}Description", parent, description, 17, MutedInk, TextAnchor.UpperLeft);
            SetRect(descText.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(88, y - 18), new Vector2(-88, 42));
        }

        private static void AddDecorativeFrame(Transform parent)
        {
            var top = Image("GoldTop", parent, Gold);
            SetRect(top.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -7), new Vector2(-22, 4));
            var bottom = Image("GoldBottom", parent, Gold);
            SetRect(bottom.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 7), new Vector2(-22, 4));
            var divider = Image("Divider", parent, new Color(Gold.r, Gold.g, Gold.b, 0.7f));
            SetRect(divider.rectTransform, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-760, -12), new Vector2(2, 580));
        }

        private static void AddRule(RectTransform parent, float y)
        {
            var rule = Image("Rule", parent, new Color(Gold.r, Gold.g, Gold.b, 0.65f));
            SetRect(rule.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, y), new Vector2(0, 2));
        }

        private static T CreateLinkedPanel<T>(Transform parent, string name, string title) where T : ClanPopup
        {
            var root = Rect(name, parent);
            Stretch(root);
            var popup = root.gameObject.AddComponent<T>();
            SetPopupBase(popup, root);
            var dimmer = Image("Dimmer", root, new Color(0.05f, 0.045f, 0.04f, 0.78f));
            Stretch(dimmer.rectTransform);
            var window = Image("Window", root, Parchment);
            SetRect(window.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1100, 700));
            window.gameObject.AddComponent<Outline>().effectColor = Ink;
            var label = Text("Title", window.transform, title, 36, Ink, TextAnchor.MiddleCenter, true);
            SetRect(label.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -70), new Vector2(700, 60));
            var close = Button("CloseButton", window.transform, "X", Ink, new Vector2(54, 54), 28);
            SetRect(close.GetComponent<RectTransform>(), Vector2.one, Vector2.one, new Vector2(-34, -34), new Vector2(54, 54));
            close.image.color = new Color(0.86f, 0.80f, 0.72f, 0.95f);
            SetObject(popup, "closeButton", close);
            return popup;
        }

        private static Button Button(string name, Transform parent, string label, Color color, Vector2 size, int fontSize, Sprite background = null, Color? labelColor = null)
        {
            var image = Image(name, parent, background == null ? color : Color.white);
            if (background != null)
            {
                image.sprite = background;
                image.type = UnityEngine.UI.Image.Type.Simple;
            }
            else
            {
                image.gameObject.AddComponent<Outline>().effectColor = new Color(0.25f, 0.18f, 0.12f, 0.95f);
            }
            var button = image.gameObject.AddComponent<Button>();
            var colors = button.colors;
            var baseColor = background == null ? color : Color.white;
            colors.normalColor = baseColor;
            colors.highlightedColor = Color.Lerp(baseColor, new Color(1f, 0.94f, 0.76f), 0.12f);
            colors.pressedColor = Color.Lerp(baseColor, Color.black, 0.16f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            image.rectTransform.sizeDelta = size;
            var text = Text("Label", image.transform, label, fontSize, labelColor ?? new Color(0.98f, 0.91f, 0.75f), TextAnchor.MiddleCenter);
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

        private static void SetPopupBase(ClanPopup popup, RectTransform self)
        {
            SetObject(popup, "myTf", self);
            SetObject(popup, "selfObject", self.gameObject);
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

        private static void PrepareArtwork()
        {
            AssetDatabase.ImportAsset(ArtPath, ImportAssetOptions.ForceSynchronousImport);
            var importer = AssetImporter.GetAtPath(ArtPath) as TextureImporter;
            if (importer == null) return;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.maxTextureSize = 1024;
            importer.SaveAndReimport();
        }

        private static void PrepareButtonTexture(string path)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spriteBorder = Vector4.zero;
            importer.alphaIsTransparency = false;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.maxTextureSize = 1024;
            importer.SaveAndReimport();
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString(value, out var color);
            return color;
        }
    }
}
#endif
