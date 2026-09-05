using System;
using System.Collections;
using AllMaps;
using UnityEngine;

namespace Tutorials
{
    [Serializable]
    public class TutorialStep
    {
        [field: SerializeField] public RectTransform TargetRect { get; private set; }
        [field: SerializeField] public Renderer TargetRenderer { get; private set; }
        [field: SerializeField] public string Text { get; private set; }
    }

    public class Tutorial : BaseBehaviour
    {
        [SerializeField] private TutorialHighlight highlight;
        [SerializeField] private float startDelay = 0.5f;
        [SerializeField] private TutorialStep[] steps;
        private bool isShowing;
        private int stepIndex;

        private void Start()
        {
            highlight.OnClose += OnClose;
            if (steps.Length > 0)
            {
                StartCoroutine(IETutorial());
            }
        }

        private IEnumerator IETutorial()
        {
            var cam = CameraFollow.Instance;
            yield return new WaitForSeconds(startDelay);
            if (stepIndex == 0)
            {
                var step = steps[stepIndex];
                isShowing = true;
                if (step.TargetRect != null)
                {
                    highlight.Show(step.TargetRect, step.Text);
                }
                else if (step.TargetRenderer != null)
                {
                    highlight.Show(step.TargetRenderer, step.Text);
                    cam.target = step.TargetRenderer.transform;
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
                    highlight.MoveTo(step.TargetRect, step.Text);
                }
                else if (step.TargetRenderer != null)
                {
                    highlight.MoveTo(step.TargetRenderer, step.Text);
                    cam.target = step.TargetRenderer.transform;
                }
                else
                {
                    isShowing = false;
                    throw new Exception("Null target");
                }
                yield return new WaitWhile(() => isShowing);
                stepIndex++;
            }
        }

        private bool OnClose()
        {
            isShowing = false;
            if (stepIndex >= steps.Length - 1)
            {
                return true;
            }
            return false;
        }
    }
}