using System;
using AllMaps;
using UnityEngine;

namespace Tutorials
{
    public class Camera2DDrag : MonoBehaviour
    {
        [Header("Camera")] [SerializeField] private Camera targetCamera;

        [Header("Settings")] [SerializeField] private float dragSpeed = 1f;
        [SerializeField] private bool useUnscaledTime = false;

        [Header("Bounds")] [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private Vector3 lastWorldPosition;
        private bool isDragging;
        [SerializeField] private BoxCollider2D boundsBox;

        private void OnValidate()
        {
            if (boundsBox != null)
            {
                minBounds = boundsBox.bounds.min;
                maxBounds = boundsBox.bounds.max;
            }
        }

        private void Awake()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouse();
#else
        HandleTouch();
#endif
        }

        private void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                BeginDrag(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                Drag(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }

        private void HandleTouch()
        {
            if (Input.touchCount == 0)
                return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    BeginDrag(touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    Drag(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndDrag();
                    break;
            }
        }

        private void BeginDrag(Vector2 screenPosition)
        {
            isDragging = true;
            CameraFollow.Instance.IsFollow = false;
            lastWorldPosition = ScreenToWorld(screenPosition);
        }

        private void Drag(Vector2 screenPosition)
        {
            if (!isDragging)
                return;

            Vector3 currentWorldPosition = ScreenToWorld(screenPosition);

            Vector3 delta = lastWorldPosition - currentWorldPosition;
            delta *= dragSpeed;

            Vector3 newPosition = targetCamera.transform.position + delta;

            if (useBounds)
                newPosition = ClampPosition(newPosition);

            targetCamera.transform.position = newPosition;

            // Cập nhật lại sau khi camera đã di chuyển
            lastWorldPosition = ScreenToWorld(screenPosition);
        }

        private void EndDrag()
        {
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
            position.x = Mathf.Clamp(
                position.x,
                minBounds.x,
                maxBounds.x
            );

            position.y = Mathf.Clamp(
                position.y,
                minBounds.y,
                maxBounds.y
            );

            return position;
        }
    }
}