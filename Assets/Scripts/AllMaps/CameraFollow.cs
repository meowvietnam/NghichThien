using UnityEngine;

namespace AllMaps
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] public Transform target;
        [SerializeField] private Camera cam;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float followSpeed;
        [SerializeField] private BoxCollider2D boundsBox;
        [SerializeField] public Transform camTransform;
        private Vector3 velocity;
        public static CameraFollow Instance;
        public bool IsFollow { get; set; }

        private void Awake()
        {
            Instance = this;
            IsFollow = true;
        }

        private void FixedUpdate()
        {
            if (!IsFollow) return;
            if (target == null)
            {
                return;
            }

            var bounds = boundsBox.bounds;
            Vector3 targetPosition = target.position + offset;
            var camPosition = camTransform.position;
            camPosition = Vector3.SmoothDamp(camPosition, targetPosition, ref velocity, 1f / followSpeed);
            var minBound = bounds.min;
            var maxBound = bounds.max;
            // Dùng cùng công thức giới hạn tầm nhìn với Camera2DDrag (camera đang là orthographic) —
            // nếu không, lúc chuyển từ follow sang kéo tay (hoặc ngược lại) camera sẽ bị giật do 2 script
            // tính vùng hợp lệ khác nhau.
            float halfHeight = cam.orthographic
                ? cam.orthographicSize
                : Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * Mathf.Abs(camPosition.z);
            float halfWidth = halfHeight * cam.aspect;
            float minX = minBound.x + halfWidth;
            float maxX = maxBound.x - halfWidth;
            float minY = minBound.y + halfHeight;
            float maxY = maxBound.y - halfHeight;
            camPosition.x = minX <= maxX ? Mathf.Clamp(camPosition.x, minX, maxX) : (minBound.x + maxBound.x) * 0.5f;
            camPosition.y = minY <= maxY ? Mathf.Clamp(camPosition.y, minY, maxY) : (minBound.y + maxBound.y) * 0.5f;
            camTransform.position = camPosition;
        }
    }
}