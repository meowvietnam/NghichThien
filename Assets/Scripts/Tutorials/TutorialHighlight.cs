using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Tutorials
{
    public class TutorialHighlight : BaseBehaviour
    {
        [Header("References")] [SerializeField]
        private Image top;

        [SerializeField] private Image bottom;
        [SerializeField] private Image left;
        [SerializeField] private Image right;

        [SerializeField] private RectTransform highlight;
        [SerializeField] private DialogText dialogText;
        [SerializeField] private Vector2 dialogOffset;
        [Header("Animation")] [SerializeField] private float showDuration = 0.2f;
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private float hideDuration = 0.2f;

        [SerializeField] private Ease showEase = Ease.OutBack;
        [SerializeField] private Ease moveEase = Ease.OutQuad;
        [SerializeField] private Ease hideEase = Ease.InBack;

        [Header("Highlight")] [SerializeField] private Vector2 padding = new Vector2(10f, 10f);

        [Header("Input")] [SerializeField] private bool clickDarkAreaToHide = true;

        private Canvas canvas;
        private RectTransform canvasRect;
        private Camera worldCamera;

        private RectTransform uiTarget;
        private Renderer worldTarget;

        private Tween highlightTween;
        private Tween overlayTween;

        private bool isMoving;
        private bool isVisible;
        public event Func<bool> OnClose;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();

            if (canvas == null)
            {
                Debug.LogError(
                    $"{nameof(TutorialHighlight)} requires a parent Canvas.",
                    this);

                return;
            }

            canvasRect = canvas.transform as RectTransform;

            worldCamera = Camera.main;

            SetPanelActive(false);
            highlight.gameObject.SetActive(false);
        }

        [Button]
        public void Show(RectTransform target, string text)
        {
            if (target == null)
                return;

            uiTarget = target;
            worldTarget = null;

            ShowInternal(text);
        }

        [Button]
        public void Show(Renderer target, string text)
        {
            if (target == null)
                return;

            uiTarget = null;
            worldTarget = target;

            ShowInternal(text);
        }

        private void UpdateDialogPosition(
            Vector2 targetPosition,
            Vector2 targetSize)
        {
            if (dialogText == null)
                return;

            RectTransform dialogRect =
                dialogText.transform as RectTransform;

            if (dialogRect == null)
                return;

            float targetRight =
                targetPosition.x + targetSize.x * 0.5f;

            float targetTop =
                targetPosition.y + targetSize.y * 0.5f;

            dialogRect.pivot =
                new Vector2(0f, 0f);

            dialogRect.anchorMin =
                new Vector2(0.5f, 0.5f);

            dialogRect.anchorMax =
                new Vector2(0.5f, 0.5f);

            dialogRect.anchoredPosition =
                new Vector2(
                    targetRight + dialogOffset.x,
                    targetTop + dialogOffset.y);
        }

        private void ShowInternal(string text)
        {
            KillTweens();

            isVisible = true;
            isMoving = false;

            SetPanelActive(true);
            highlight.gameObject.SetActive(true);

            SetPanelsAlpha(0f);

            highlight.localScale = Vector3.zero;

            UpdateHighlightImmediate();

            overlayTween = DOTween.To(
                    () => GetPanelAlpha(),
                    SetPanelsAlpha,
                    0.7f,
                    showDuration)
                .SetEase(Ease.Linear);

            highlightTween = highlight
                .DOScale(Vector3.one, showDuration)
                .SetEase(showEase);
            dialogText.ShowText(text);
        }

        public void MoveTo(RectTransform target, string text)
        {
            if (target == null)
                return;

            uiTarget = target;
            worldTarget = null;

            MoveToTarget(text);
        }

        public void MoveTo(Renderer target, string text)
        {
            if (target == null)
                return;

            uiTarget = null;
            worldTarget = target;

            MoveToTarget(text);
        }

        private void MoveToTarget(string text)
        {
            if (!isVisible)
            {
                ShowInternal(text);
                return;
            }

            highlightTween?.Kill();
            dialogText?.Close();
            GetTargetBounds(
                out Vector2 targetPosition,
                out Vector2 targetSize);
            isMoving = true;
            highlightTween = DOTween.Sequence()
                .Join(
                    highlight
                        .DOAnchorPos(
                            targetPosition,
                            moveDuration)
                        .SetEase(moveEase)
                )
                .Join(
                    highlight
                        .DOSizeDelta(
                            targetSize,
                            moveDuration)
                        .SetEase(moveEase)
                )
                .OnUpdate(UpdateOverlayFromHighlight)
                .OnComplete(() =>
                {
                    isMoving = false;

                    UpdateHighlightImmediate();

                    // Move xong mới hiện dialog với text mới
                    dialogText?.ShowText(text);
                });
        }

        private void GetTargetBounds(
            out Vector2 position,
            out Vector2 size)
        {
            if (uiTarget != null)
            {
                GetUIBounds(
                    uiTarget,
                    out position,
                    out size);

                return;
            }

            if (worldTarget != null)
            {
                GetWorldBounds(
                    worldTarget,
                    out position,
                    out size);

                return;
            }

            position = Vector2.zero;
            size = Vector2.zero;
        }


        private void LateUpdate()
        {
            if (!isVisible)
                return;

            if (uiTarget == null && worldTarget == null)
                return;

            if (isMoving)
                return;

            if (worldTarget != null)
            {
                UpdateHighlightImmediate();
            }
        }

        private void UpdateHighlightImmediate()
        {
            GetTargetBounds(
                out Vector2 position,
                out Vector2 size);

            highlight.anchoredPosition = position;
            highlight.sizeDelta = size;

            UpdateOverlay(position, size);
            UpdateDialogPosition(position, size);
        }

        private void UpdateOverlayFromHighlight()
        {
            UpdateOverlay(
                highlight.anchoredPosition,
                highlight.sizeDelta);
        }


        private void UpdateOverlay(
            Vector2 position,
            Vector2 size)
        {
            if (canvasRect == null)
                return;

            Rect rect = canvasRect.rect;

            float leftX =
                position.x -
                size.x * 0.5f;

            float rightX =
                position.x +
                size.x * 0.5f;

            float bottomY =
                position.y -
                size.y * 0.5f;

            float topY =
                position.y +
                size.y * 0.5f;

            // --------------------------------------------------------
            // TOP
            // --------------------------------------------------------

            SetPanelRect(
                top,
                new Vector2(
                    rect.width,
                    Mathf.Max(0f, rect.yMax - topY)),
                new Vector2(
                    0f,
                    topY +
                    Mathf.Max(0f, rect.yMax - topY) * 0.5f));

            // --------------------------------------------------------
            // BOTTOM
            // --------------------------------------------------------

            SetPanelRect(
                bottom,
                new Vector2(
                    rect.width,
                    Mathf.Max(0f, bottomY - rect.yMin)),
                new Vector2(
                    0f,
                    rect.yMin +
                    Mathf.Max(0f, bottomY - rect.yMin) * 0.5f));

            // --------------------------------------------------------
            // LEFT
            // --------------------------------------------------------

            SetPanelRect(
                left,
                new Vector2(
                    Mathf.Max(0f, leftX - rect.xMin),
                    Mathf.Max(0f, size.y)),
                new Vector2(
                    rect.xMin +
                    Mathf.Max(0f, leftX - rect.xMin) * 0.5f,
                    position.y));

            // --------------------------------------------------------
            // RIGHT
            // --------------------------------------------------------

            SetPanelRect(
                right,
                new Vector2(
                    Mathf.Max(0f, rect.xMax - rightX),
                    Mathf.Max(0f, size.y)),
                new Vector2(
                    rightX +
                    Mathf.Max(0f, rect.xMax - rightX) * 0.5f,
                    position.y));
        }

        private void SetPanelRect(
            Image image,
            Vector2 size,
            Vector2 position)
        {
            if (image == null)
                return;

            RectTransform rect =
                image.rectTransform;

            rect.anchorMin =
                new Vector2(0.5f, 0.5f);

            rect.anchorMax =
                new Vector2(0.5f, 0.5f);

            rect.pivot =
                new Vector2(0.5f, 0.5f);

            rect.anchoredPosition =
                position;

            rect.sizeDelta =
                size;
        }

        // ============================================================
        // UI Target
        // ============================================================

        private void GetUIBounds(
            RectTransform target,
            out Vector2 position,
            out Vector2 size)
        {
            Vector3[] corners =
                new Vector3[4];

            target.GetWorldCorners(corners);

            Vector2 min =
                WorldToCanvasPosition(corners[0]);

            Vector2 max =
                WorldToCanvasPosition(corners[2]);

            position =
                (min + max) * 0.5f;

            size =
                max - min;

            size +=
                padding * 2f;
        }

        // ============================================================
        // World Target
        // ============================================================

        private void GetWorldBounds(
            Renderer target,
            out Vector2 position,
            out Vector2 size)
        {
            if (worldCamera == null)
                worldCamera = Camera.main;

            position = Vector2.zero;
            size = Vector2.zero;

            if (worldCamera == null)
                return;

            Bounds bounds =
                target.bounds;

            Vector3[] corners =
            {
                new Vector3(
                    bounds.min.x,
                    bounds.min.y,
                    bounds.center.z),

                new Vector3(
                    bounds.min.x,
                    bounds.max.y,
                    bounds.center.z),

                new Vector3(
                    bounds.max.x,
                    bounds.min.y,
                    bounds.center.z),

                new Vector3(
                    bounds.max.x,
                    bounds.max.y,
                    bounds.center.z)
            };

            Vector2 min =
                new Vector2(
                    float.MaxValue,
                    float.MaxValue);

            Vector2 max =
                new Vector2(
                    float.MinValue,
                    float.MinValue);

            bool hasValidPoint = false;

            foreach (Vector3 corner in corners)
            {
                Vector3 screenPoint =
                    worldCamera.WorldToScreenPoint(corner);

                // Object phía sau camera.
                if (screenPoint.z < 0)
                    continue;

                Vector2 canvasPoint =
                    ScreenToCanvasPosition(
                        screenPoint);

                min =
                    Vector2.Min(
                        min,
                        canvasPoint);

                max =
                    Vector2.Max(
                        max,
                        canvasPoint);

                hasValidPoint = true;
            }

            if (!hasValidPoint)
                return;

            position =
                (min + max) * 0.5f;

            size =
                max - min;

            size +=
                padding * 2f;
        }

        // ============================================================
        // Coordinate Conversion
        // ============================================================

        private Vector2 WorldToCanvasPosition(
            Vector3 worldPosition)
        {
            Camera camera =
                GetCanvasCamera();

            Vector2 screenPosition =
                RectTransformUtility.WorldToScreenPoint(
                    camera,
                    worldPosition);

            return ScreenToCanvasPosition(
                screenPosition);
        }

        private Vector2 ScreenToCanvasPosition(
            Vector2 screenPosition)
        {
            Camera camera =
                GetCanvasCamera();

            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPosition,
                    camera,
                    out Vector2 localPosition);

            return localPosition;
        }

        private Camera GetCanvasCamera()
        {
            if (canvas.renderMode ==
                RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }

            return canvas.worldCamera;
        }

        // ============================================================
        // Complete
        // ============================================================

        /// <summary>
        /// Hoàn thành highlight ngay lập tức.
        /// Không animation.
        /// </summary>
        public void Complete()
        {
            KillTweens();

            isMoving = false;

            if (!isVisible)
                return;

            SetPanelsAlpha(1f);

            highlight.gameObject.SetActive(true);
            highlight.localScale =
                Vector3.one;

            UpdateHighlightImmediate();
        }

        // ============================================================
        // Hide
        // ============================================================

        public void Hide()
        {
            if (OnClose == null || OnClose())
            {
                KillTweens();
                isMoving = false;
                if (!isVisible)
                    return;
                isVisible = false;
                overlayTween = DOTween.To(
                        () => GetPanelAlpha(),
                        SetPanelsAlpha,
                        0f,
                        hideDuration)
                    .SetEase(Ease.Linear);
                highlightTween = highlight
                    .DOScale(
                        Vector3.zero,
                        hideDuration)
                    .SetEase(hideEase)
                    .OnComplete(() =>
                    {
                        SetPanelActive(false);

                        highlight.gameObject
                            .SetActive(false);

                        uiTarget = null;
                        worldTarget = null;
                    });
                dialogText.Close();
            }
        }

        // ============================================================
        // Helpers
        // ============================================================

        private void SetPanelActive(
            bool active)
        {
            if (top != null)
                top.gameObject.SetActive(active);

            if (bottom != null)
                bottom.gameObject.SetActive(active);

            if (left != null)
                left.gameObject.SetActive(active);

            if (right != null)
                right.gameObject.SetActive(active);
        }

        private void SetPanelsAlpha(
            float alpha)
        {
            SetImageAlpha(top, alpha);
            SetImageAlpha(bottom, alpha);
            SetImageAlpha(left, alpha);
            SetImageAlpha(right, alpha);
        }

        private void SetImageAlpha(
            Image image,
            float alpha)
        {
            if (image == null)
                return;

            Color color =
                image.color;

            color.a = alpha;

            image.color = color;
        }

        private float GetPanelAlpha()
        {
            if (top != null)
                return top.color.a;

            if (bottom != null)
                return bottom.color.a;

            if (left != null)
                return left.color.a;

            if (right != null)
                return right.color.a;

            return 0f;
        }

        private void KillTweens()
        {
            highlightTween?.Kill();
            overlayTween?.Kill();

            highlightTween = null;
            overlayTween = null;
        }

        private void OnDestroy()
        {
            KillTweens();
        }
    }
}