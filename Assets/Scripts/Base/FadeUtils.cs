using System;
using DG.Tweening;

namespace Base
{
    public static class FadeUtils
    {
        public static Tween DoFade(this IFadable fadable,float start, float end,float duration ,bool interactable, Action onCompleted = null)
        {
            var canvasGroup = fadable.CanvasGroup; 
            fadable.Tween?.Kill();
            canvasGroup.interactable = interactable;
            canvasGroup.alpha = start;
            fadable.Tween = canvasGroup.DOFade(end, duration).OnComplete(() => onCompleted?.Invoke());
            return fadable.Tween;
        }
    }
}