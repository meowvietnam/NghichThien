using DG.Tweening;
using UnityEngine;

namespace Base
{
    public interface IFadable
    {
        CanvasGroup CanvasGroup { get; }
        Tween Tween { get; set; }
    }
}