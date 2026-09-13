using System;
using System.Collections;
using AllMaps;
using UnityEngine;

namespace Tutorials
{
    public enum DialogPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [Serializable]
    public class TutorialStep
    {
        [field: SerializeField] public RectTransform TargetRect { get; set; }
        [field: SerializeField] public Renderer TargetRenderer { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DialogPosition DialogPosition { get; set; } = DialogPosition.TopRight;

        /// <summary>
        /// "Hand tut": khi true, TutorialHighlight sẽ KHÔNG tự đóng/chuyển bước khi người chơi
        /// chạm vào vùng tối xung quanh (clickDarkAreaToHide bị bỏ qua), đồng thời hiện tay chỉ
        /// (hand pointer) tại Target. Bước chỉ tiến tiếp khi code gameplay tự gọi
        /// Tutorial.CompleteInteractiveStep() (thường là khi người chơi bấm ĐÚNG nút/khu vực
        /// thật đang được khoanh sáng, hoặc khi một điều kiện gameplay thật sự xảy ra).
        /// </summary>
        [field: SerializeField] public bool RequireRealAction { get; set; }
    }

    public class Tutorial : BaseBehaviour
    {
        [SerializeField] private TutorialHighlight highlight;
        [SerializeField] private float startDelay = 0.5f;
        private TutorialStep[] steps;
        private bool isShowing;
        private int stepIndex;
        private Coroutine routine;
        private float currentStartDelay;
        public event Action<Renderer> OnSetTarget;
        public event Action OnCompleted;

        private void Start()
        {
            highlight.OnClose += OnClose;
        }

        /// <param name="startDelayOverride">
        /// Bỏ qua startDelay mặc định (truyền 0 để hiện ngay). Dùng khi cần đổi target liên tục
        /// trong lúc người chơi đang thao tác, VD đang kéo vật phẩm thì chuyển khung sáng sang ô
        /// đích - chờ thêm nửa giây nữa thì không kịp.
        /// </param>
        public void ShowTutorial(TutorialStep[] steps, float startDelayOverride = -1f)
        {
            // BẮT BUỘC dừng hẳn lượt tutorial trước đó (nếu còn) trước khi bắt đầu lượt mới.
            //
            // Các chuỗi hand-tut thường gọi ShowTutorial() ngay trong cùng frame với
            // CompleteInteractiveStep() của bước trước (VD: mua đủ 3 quyển Dưỡng Đao Thuật xong là
            // hiện ngay bước chọn Súc Thể). Khi đó coroutine cũ vẫn đang đứng chờ ở
            // WaitWhile(() => isShowing); frame kế tiếp nó tỉnh dậy và chạy stepIndex++ - đè lên
            // đúng cái stepIndex = 0 mà lượt mới vừa đặt. Coroutine mới sau startDelay thấy
            // stepIndex đã là 1 nên bỏ qua cả khối đầu lẫn vòng while và không hiện bước nào cả.
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            this.steps = steps;
            stepIndex = 0;
            isShowing = false;
            currentStartDelay = startDelayOverride >= 0f ? startDelayOverride : startDelay;

            if (steps.Length > 0)
            {
                routine = StartCoroutine(IETutorial());
            }
        }

        /// <summary>
        /// Gọi từ code gameplay khi người chơi vừa thực hiện ĐÚNG hành động thật (bấm đúng nút,
        /// kéo đúng vật phẩm, đạt đúng điều kiện...) mà bước hand-tut hiện tại (RequireRealAction)
        /// đang chờ. Bỏ qua nếu tutorial không đang hiển thị hoặc bước hiện tại không phải hand-tut.
        /// </summary>
        public void CompleteInteractiveStep()
        {
            if (!isShowing) return;
            if (steps == null || stepIndex >= steps.Length) return;
            if (!steps[stepIndex].RequireRealAction) return;

            highlight.CompleteFromRealAction();
        }

        private IEnumerator IETutorial()
        {
            if (currentStartDelay > 0f)
                yield return new WaitForSeconds(currentStartDelay);
            if (stepIndex == 0)
            {
                var step = steps[stepIndex];
                isShowing = true;
                if (step.TargetRect != null)
                {
                    highlight.SetDialogPosition(step.DialogPosition);
                    highlight.Show(step.TargetRect, step.Text);
                }
                else if (step.TargetRenderer != null)
                {
                    highlight.SetDialogPosition(step.DialogPosition);
                    highlight.Show(step.TargetRenderer, step.Text);
                    OnSetTarget?.Invoke(step.TargetRenderer);
                }
                else
                {
                    // Không throw: một bước lỗi target (VD renderer của quái/quest chưa dựng xong)
                    // mà ném exception sẽ giết luôn coroutine, khiến TẤT CẢ các bước còn lại và
                    // cả OnCompleted không bao giờ chạy. Bỏ qua bước đó và đi tiếp.
                    Debug.LogWarning($"[Tutorial] Bỏ qua bước {stepIndex}: không có target.");
                    isShowing = false;
                }

                highlight.SetIgnoreDarkAreaClick(step.RequireRealAction);
                highlight.SetHandPointerVisible(step.RequireRealAction);

                yield return new WaitWhile(() => isShowing);
                stepIndex++;
            }

            while (stepIndex < steps.Length)
            {
                var step = steps[stepIndex];
                isShowing = true;
                if (step.TargetRect != null)
                {
                    highlight.SetDialogPosition(step.DialogPosition);
                    highlight.MoveTo(step.TargetRect, step.Text);
                }
                else if (step.TargetRenderer != null)
                {
                    highlight.SetDialogPosition(step.DialogPosition);
                    highlight.MoveTo(step.TargetRenderer, step.Text);
                    OnSetTarget?.Invoke(step.TargetRenderer);
                }
                else
                {
                    // Không throw: một bước lỗi target (VD renderer của quái/quest chưa dựng xong)
                    // mà ném exception sẽ giết luôn coroutine, khiến TẤT CẢ các bước còn lại và
                    // cả OnCompleted không bao giờ chạy. Bỏ qua bước đó và đi tiếp.
                    Debug.LogWarning($"[Tutorial] Bỏ qua bước {stepIndex}: không có target.");
                    isShowing = false;
                }

                highlight.SetIgnoreDarkAreaClick(step.RequireRealAction);
                highlight.SetHandPointerVisible(step.RequireRealAction);

                yield return new WaitWhile(() => isShowing);
                stepIndex++;
            }

            routine = null;
            OnCompleted?.Invoke();
        }

        private bool OnClose()
        {
            isShowing = false;
            if (stepIndex >= steps.Length - 1)
            {
                GC.Collect();
                return true;
            }

            return false;
        }
    }
}