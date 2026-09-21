#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

namespace Boss.Editor
{
    public static class BossInfoPopupBuilder
    {
        public const string PrefabPath = "Assets/Resources/BossInfoPopup.prefab";
        private const string SourcePath = "Assets/Scripts/AllMaps/PrepareCombat/PrepareCombatPopup.prefab";
        private static Font font;
        private static readonly Color Ink = new Color32(239, 222, 185, 255);

        [MenuItem("Tools/Boss/Build Boss Info Popup")]
        public static void Build()
        {
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(SourcePath);
            if (source == null) throw new InvalidOperationException("PrepareCombatPopup missing.");
            var sourceFields = new SerializedObject(source.GetComponent<PrepareCombatPanel>());
            var sourceText = (UnityEngine.UI.Text)sourceFields.FindProperty("txtTarget").objectReferenceValue;
            font = sourceText.font;

            var root = Rect("BossInfoPopup", null, Vector2.zero, new Vector2(1920, 1080));
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.sizeDelta = Vector2.zero;
            var popup = root.gameObject.AddComponent<BossInfoPopup>();
            var mask = Image("Dimmer", root, Vector2.zero, new Vector2(1920, 1080), new Color(0, 0, 0, 0.65f));
            mask.rectTransform.anchorMin = Vector2.zero;
            mask.rectTransform.anchorMax = Vector2.one;
            mask.rectTransform.sizeDelta = Vector2.zero;
            mask.raycastTarget = true;
            var window = Image("Window", root, Vector2.zero, new Vector2(1140, 680), Color.white);
            var background = source.GetComponentsInChildren<UnityEngine.UI.Image>(true)
                .First(x => x.name == "BG");
            window.sprite = background.sprite;
            window.type = UnityEngine.UI.Image.Type.Sliced;
            window.raycastTarget = true;

            Text("Heading", window.transform, "BOSS XUẤT HIỆN", new Vector2(0, 260), new Vector2(850, 60), 34, TextAnchor.MiddleCenter);
            var graphicSource = (CharacterVisualControl)sourceFields.FindProperty("graphicTarget").objectReferenceValue;
            var graphic = UnityEngine.Object.Instantiate(graphicSource, window.transform);
            graphic.name = "BossVisual";
            var graphicRect = graphic.GetComponent<RectTransform>();
            Place(graphicRect, new Vector2(-300, -5), new Vector2(340, 400));
            graphic.transform.localScale = Vector3.one;
            foreach (var g in graphic.GetComponentsInChildren<UnityEngine.UI.Graphic>(true)) g.raycastTarget = false;

            var title = Text("BossName", window.transform, "Huyền Thiên Cự Thú", new Vector2(190, 155), new Vector2(520, 70), 35);
            var level = Text("Level", window.transform, "Cảnh giới: Nguyên Anh", new Vector2(190, 100), new Vector2(520, 50), 26);
            var hpBack = Image("HpBackground", window.transform, new Vector2(180, 38), new Vector2(500, 30), new Color32(63, 47, 43, 255));
            var hp = Image("HpFill", hpBack.transform, Vector2.zero, new Vector2(494, 24), new Color32(152, 52, 43, 255));
            hp.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            hp.type = UnityEngine.UI.Image.Type.Filled;
            hp.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            hp.fillAmount = 0.72f;
            var hpText = Text("HpText", window.transform, "720.000 / 1.000.000", new Vector2(180, 38), new Vector2(500, 36), 21, TextAnchor.MiddleCenter);
            hpText.color = Color.white;
            var players = Text("PlayerCount", window.transform, "Đang chiến đấu: 12 người", new Vector2(190, -32), new Vector2(520, 50), 27);
            var attempts = Text("Attempts", window.transform, "Lượt còn lại: 3/3", new Vector2(190, -82), new Vector2(520, 50), 27);
            var message = Text("Message", window.transform, "", new Vector2(0, -165), new Vector2(950, 60), 23, TextAnchor.MiddleCenter);
            message.color = new Color32(255, 180, 148, 255);

            var confirmPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/PopupConfirmAction.prefab");
            var confirmFields = new SerializedObject(confirmPrefab.GetComponent<PopupConfirmAction>());
            var sourceJoin = (UnityEngine.UI.Button)confirmFields.FindProperty("btnConfirm").objectReferenceValue;
            var sourceClose = sourceJoin;
            var close = Button("Close", window.transform, sourceClose, new Vector2(-180, -250), "Đóng", out _);
            var join = Button("Join", window.transform, sourceJoin, new Vector2(180, -250), "Tham gia", out var joinLabel);
            var fields = new SerializedObject(popup);
            Set(fields, "commonMask", mask); Set(fields, "btnClose", close);
            Set(fields, "titleText", title); Set(fields, "levelText", level); Set(fields, "hpText", hpText);
            Set(fields, "playerCountText", players); Set(fields, "attemptsText", attempts);
            Set(fields, "messageText", message); Set(fields, "hpFill", hp);
            Set(fields, "joinButton", join); Set(fields, "joinLabel", joinLabel); Set(fields, "bossVisual", graphic);
            fields.ApplyModifiedPropertiesWithoutUndo();
            root.gameObject.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
            UnityEngine.Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
        }

        private static UnityEngine.UI.Button Button(string name, Transform parent, UnityEngine.UI.Button source,
            Vector2 position, string label, out Text text)
        {
            var button = UnityEngine.Object.Instantiate(source, parent);
            button.name = name;
            Place(button.GetComponent<RectTransform>(), position, new Vector2(275, 70));
            button.transform.localScale = Vector3.one;
            button.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            foreach (Transform child in button.transform) child.gameObject.SetActive(false);
            // Keep the original button artwork; replace its caption with Vietnamese text.
            foreach (var graphic in button.GetComponentsInChildren<UnityEngine.UI.Graphic>(true)) graphic.raycastTarget = false;
            if (button.targetGraphic != null) button.targetGraphic.raycastTarget = true;
            button.gameObject.SetActive(true);
            text = Text("Label", button.transform, label, Vector2.zero, new Vector2(255, 58), 29, TextAnchor.MiddleCenter);
            return button;
        }

        private static void Set(SerializedObject target, string name, UnityEngine.Object value) => target.FindProperty(name).objectReferenceValue = value;
        private static RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false); Place(rect, position, size); return rect;
        }
        private static void Place(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position; rect.sizeDelta = size;
        }
        private static UnityEngine.UI.Image Image(string name, Transform parent, Vector2 position, Vector2 size, Color color)
        {
            var image = Rect(name, parent, position, size).gameObject.AddComponent<UnityEngine.UI.Image>();
            image.color = color; image.raycastTarget = false; return image;
        }
        private static Text Text(string name, Transform parent, string value, Vector2 position, Vector2 size, int fontSize,
            TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var text = Rect(name, parent, position, size).gameObject.AddComponent<Text>();
            text.font = font; text.text = value; text.fontSize = fontSize; text.color = Ink;
            text.alignment = alignment; text.raycastTarget = false; text.horizontalOverflow = HorizontalWrapMode.Overflow;
            return text;
        }

    }
}
#endif
