using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class SayDialog : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private GameObject selfObject;
        private Tween tween;

        public void Chat(string message, float duration)
        {
            text.text = message;
            selfObject.SetActive(true);
            tween?.Kill(false);
            tween = DOVirtual.DelayedCall(duration, () => selfObject.SetActive(false));
        }
    }
}