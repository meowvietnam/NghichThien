using AllMaps;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tutorials
{
    // Gắn script này lên GameObject có Collider2D dùng làm vùng nhận input (boundsBox) — không phải
    // lên GameObject của Camera. EventSystem sẽ raycast qua Physics2DRaycaster (cần có trên Camera)
    // và gọi OnPointerDown/OnDrag/OnPointerUp thẳng vào GameObject này khi trúng boundsBox.
    // Nhờ đi qua EventSystem nên UI (panel, nút...) phía trên tự động chặn input này, không cần
    // tự kiểm tra IsPointerOverGameObject thủ công nữa.
    public class Camera2DDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Camera")] [SerializeField] private Camera targetCamera;

        [Header("Settings")] [SerializeField] private float dragSpeed = 1f;
        [SerializeField] private bool useUnscaledTime = false;

        [Header("Bounds")] [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds => minBoundsCollider + minBoundsAdd;
        [SerializeField] private Vector2 maxBounds => maxBoundsCollider + maxBoundAdd;

        [SerializeField] private Vector2 minBoundsAdd;
        [SerializeField] private Vector2 maxBoundAdd;
        [SerializeField] private Vector2 minBoundsCollider;
        [SerializeField] private Vector2 maxBoundsCollider;

        private Vector3 lastWorldPosition;
        private bool isDragging;
        [SerializeField] private BoxCollider2D boundsBox;

        private void OnValidate()
        {
            if (boundsBox != null)
            {
                minBoundsCollider = boundsBox.bounds.min;
                maxBoundsCollider = boundsBox.bounds.max;
            }
        }

        private void Awake()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isDragging = true;
            CameraFollow.Instance.IsFollow = false;
            lastWorldPosition = ScreenToWorld(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            Vector3 currentWorldPosition = ScreenToWorld(eventData.position);

            Vector3 delta = lastWorldPosition - currentWorldPosition;
            delta *= dragSpeed;

            Vector3 newPosition = targetCamera.transform.position + delta;

            if (useBounds)
                newPosition = ClampPosition(newPosition);

            targetCamera.transform.position = newPosition;

            // Cập nhật lại sau khi camera đã di chuyển
            lastWorldPosition = ScreenToWorld(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            isDragging = false;
            CameraFollow.Instance.IsFollow = true;
        }

        private Vector3 ScreenToWorld(Vector2 screenPosition)
        {
            float distance = Mathf.Abs(targetCamera.transform.position.z);

            return targetCamera.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    distance
                )
            );
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            // Giới hạn TẦM NHÌN (viewport) trong boundsBox, không chỉ điểm neo của camera —
            // trừ đi nửa chiều rộng/cao khung hình để camera không lộ ra ngoài boundsBox ở sát biên.
            float halfHeight = targetCamera.orthographic
                ? targetCamera.orthographicSize
                : Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * Mathf.Abs(position.z);
            float halfWidth = halfHeight * targetCamera.aspect;

            float minX = minBounds.x + halfWidth;
            float maxX = maxBounds.x - halfWidth;
            float minY = minBounds.y + halfHeight;
            float maxY = maxBounds.y - halfHeight;

            // boundsBox nhỏ hơn tầm nhìn camera theo 1 trục thì canh giữa trục đó thay vì kẹp lung tung.
            position.x = minX <= maxX ? Mathf.Clamp(position.x, minX, maxX) : (minBounds.x + maxBounds.x) * 0.5f;
            position.y = minY <= maxY ? Mathf.Clamp(position.y, minY, maxY) : (minBounds.y + maxBounds.y) * 0.5f;

            return position;
        }
    }
}
