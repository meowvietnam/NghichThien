using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AllMaps.TongMonChien
{
    /// <summary>
    /// Cho phép kéo một khối HUD bằng cách kéo phần nền của nó (nút bên trong vẫn bấm bình thường).
    /// Vị trí sau khi kéo được nhớ theo máy (PlayerPrefs); nhấp đúp vào nền để trả về vị trí tự động.
    /// </summary>
    public class HudDraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [Tooltip("Khoá lưu vị trí (PlayerPrefs) — mỗi khối một khoá riêng.")]
        [SerializeField] private string key = "Panel";

        private RectTransform rect;
        private RectTransform parentRect;
        private Canvas canvas;
        private string prefsKey;
        private Vector2 pointerOffset;

        /// <summary>Người chơi đã tự đặt vị trí: HUD không tự dời khối này nữa.</summary>
        public bool IsPinned { get; private set; }

        public event Action PositionReset;

        private void Awake()
        {
            Init(key);
        }

        private void Init(string panelKey)
        {
            rect = (RectTransform)transform;
            parentRect = (RectTransform)rect.parent;
            canvas = GetComponentInParent<Canvas>();
            prefsKey = $"TongMonChienHud.{panelKey}";
            try
            {
                if (PlayerPrefs.HasKey(prefsKey + ".x"))
                {
                    rect.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(prefsKey + ".x"),
                        PlayerPrefs.GetFloat(prefsKey + ".y"));
                    IsPinned = true;
                }
            }
            catch (Exception)
            {
                IsPinned = false;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!ToLocal(eventData, out var local)) return;
            pointerOffset = rect.anchoredPosition - local;
            rect.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!ToLocal(eventData, out var local)) return;
            rect.anchoredPosition = local + pointerOffset;
            ClampToScreen();
            IsPinned = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ClampToScreen();
            try
            {
                PlayerPrefs.SetFloat(prefsKey + ".x", rect.anchoredPosition.x);
                PlayerPrefs.SetFloat(prefsKey + ".y", rect.anchoredPosition.y);
                PlayerPrefs.Save();
            }
            catch (Exception)
            {
                // Không lưu được thì chỉ giữ vị trí trong phiên hiện tại.
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount < 2 || eventData.dragging) return;
            IsPinned = false;
            try
            {
                PlayerPrefs.DeleteKey(prefsKey + ".x");
                PlayerPrefs.DeleteKey(prefsKey + ".y");
            }
            catch (Exception)
            {
            }

            PositionReset?.Invoke();
        }

        /// <summary>Toạ độ con trỏ theo hệ anchoredPosition của khối (tính cả anchor).</summary>
        private bool ToLocal(PointerEventData eventData, out Vector2 anchored)
        {
            anchored = default;
            var cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? eventData.pressEventCamera : null;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, cam, out var local))
                return false;
            var anchorRef = Vector2.Scale(parentRect.rect.size, rect.anchorMin - parentRect.pivot);
            anchored = local - anchorRef;
            return true;
        }

        /// <summary>Giữ khối nằm trọn trong màn hình.</summary>
        public void ClampToScreen()
        {
            if (rect == null || parentRect == null) return;
            var parentSize = parentRect.rect.size;
            var size = rect.rect.size;
            var anchorPos = Vector2.Scale(parentSize, rect.anchorMin);
            // Vị trí góc dưới-trái của khối trong hệ toạ độ gốc (0,0) = góc dưới-trái cha.
            var min = anchorPos + rect.anchoredPosition - Vector2.Scale(size, rect.pivot);
            var clampedX = Mathf.Clamp(min.x, 0, Mathf.Max(0, parentSize.x - size.x));
            var clampedY = Mathf.Clamp(min.y, 0, Mathf.Max(0, parentSize.y - size.y));
            rect.anchoredPosition += new Vector2(clampedX - min.x, clampedY - min.y);
        }
    }
}
