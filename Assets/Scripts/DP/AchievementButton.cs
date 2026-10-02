using System;
using CreateCharacter;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    /// <summary>
    /// Nút Thành tựu trong scene Động phủ: mở bảng thành tựu của bản thân và hiện chấm đỏ khi có mốc nhận được.
    /// </summary>
    public class AchievementButton : BaseBehaviour
    {
        public const string PanelPrefabPath = "Assets/Scripts/DP/Prefabs/DongPhuAchievementPanel.prefab";

        [SerializeField] private Button button;
        [Tooltip("Prefab bảng thành tựu (Tools → Động phủ → Tạo prefab Thành tựu).")]
        [SerializeField] private DongPhuAchievementPanel panelPrefab;
        [Tooltip("Chấm đỏ báo có mốc nhận được. Bỏ trống thì tự tạo.")]
        [SerializeField] private GameObject badge;

        private static AchievementButton instance;
        private static int claimableCount;
        private DongPhuAchievementPanel panel;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (panelPrefab == null)
                panelPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<DongPhuAchievementPanel>(PanelPrefabPath);
        }
#endif

        private void Awake()
        {
            instance = this;
            if (badge == null) badge = CreateBadge();
            RefreshBadge();
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        /// <summary>Số mốc thành tựu đang nhận được (server báo qua DongPhuAchievementNotify / sau khi nhận).</summary>
        public static void SetClaimableCount(int count)
        {
            claimableCount = Math.Max(0, count);
            if (instance != null) instance.RefreshBadge();
        }

        private void RefreshBadge()
        {
            if (badge != null) badge.SetActive(claimableCount > 0);
        }

        private void OnClick()
        {
            if (panelPrefab == null)
            {
                DongPhuPanel.Instance.NotifyGroup.Show("Chưa có prefab bảng thành tựu");
                return;
            }

            if (panel == null)
            {
                var canvas = GetComponentInParent<Canvas>();
                var parent = canvas != null ? canvas.rootCanvas.transform : transform.root;
                panel = Instantiate(panelPrefab, parent);
                panel.transform.localScale = Vector3.zero;
            }

            panel.Show();
        }

        /// <summary>Chấm đỏ mặc định ở góc trên phải nút.</summary>
        private GameObject CreateBadge()
        {
            // Dùng đúng chấm đỏ của nút Thư; không có nút Thư thì mới vẽ chấm tạm.
            var redPoint = AllMaps.MailButton.CreateRedPoint(transform);
            if (redPoint != null) return redPoint;
            var go = new GameObject("Badge", typeof(RectTransform), typeof(Image));
            var rect = (RectTransform)go.transform;
            rect.SetParent(transform, false);
            rect.anchorMin = rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-8, -8);
            rect.sizeDelta = new Vector2(22, 22);
            var image = go.GetComponent<Image>();
            image.sprite = CircleSprite();
            image.color = new Color(0.95f, 0.18f, 0.15f);
            image.raycastTarget = false;
            return go;
        }

        private static Sprite circle;

        private static Sprite CircleSprite()
        {
            if (circle != null) return circle;
            const int size = 32;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var c = (size - 1) / 2f;
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                var d = Vector2.Distance(new Vector2(x, y), new Vector2(c, c)) / c;
                tex.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01((1f - d) * 8f)));
            }

            tex.Apply();
            circle = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return circle;
        }
    }
}
