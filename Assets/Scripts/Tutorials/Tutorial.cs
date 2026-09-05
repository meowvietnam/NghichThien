using System;
using System.Collections;
using AllMaps;
using UnityEngine;

namespace Tutorials
{
    public enum DialogPosition { TopLeft, TopRight, BottomLeft, BottomRight }
    [Serializable]
    public class TutorialStep
    {
        [field: SerializeField] public RectTransform TargetRect { get; set; }
        [field: SerializeField] public Renderer TargetRenderer { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DialogPosition DialogPosition { get; set; } = DialogPosition.TopRight;
    }

    public class Tutorial : BaseBehaviour
    {
        [SerializeField] private TutorialHighlight highlight;
        [SerializeField] private float startDelay = 0.5f;
        private TutorialStep[] steps;
        private bool isShowing;
        private int stepIndex;
        public event Action<Renderer> OnSetTarget;
        public event Action OnCompleted;
        private void Start()
        {
            highlight.OnClose += OnClose;
        }

        public void ShowTutorial(TutorialStep[] steps)
        {
            this.steps = steps;
            if (steps.Length > 0)
            {
                StartCoroutine(IETutorial());
            }
        }

        private IEnumerator IETutorial()
        {
            yield return new WaitForSeconds(startDelay);
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
                    isShowing = false;
                    throw new Exception("Null target");
                }

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
                    isShowing = false;
                    throw new Exception("Null target");
                }

                yield return new WaitWhile(() => isShowing);
                stepIndex++;
            }
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