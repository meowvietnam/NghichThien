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
        [SerializeField]
        public Transform camTransform;
        private Vector3 velocity;
        public static CameraFollow Instance;
        private void Awake()
        {
            Instance = this;
        }

        private void FixedUpdate()
        {
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
            float camZ = Mathf.Abs(camPosition.z);
            float halfHeight = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * camZ;
            float halfWidth = halfHeight * cam.aspect;
            float clampX = Mathf.Clamp(camPosition.x, minBound.x + halfWidth, maxBound.x - halfWidth);
            float clampY = Mathf.Clamp(camPosition.y, minBound.y + halfHeight, maxBound.y - halfHeight);
            camPosition.x = clampX;
            camPosition.y = clampY;
            camTransform.position = camPosition;
        }
    }
}