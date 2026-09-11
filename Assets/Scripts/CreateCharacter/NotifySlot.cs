using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class NotifySlot : MonoBehaviour
    {
        [SerializeField] private GameObject thisObject;
        [SerializeField] private RectTransform thisTransform;
        [SerializeField] private Text notifyText;
        public bool IsActive => thisObject.activeSelf;
        private static float start = 0;
        private static float end = -494;

        public void SetText(string text)
        {
            notifyText.text = text;
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }

        public Tween Show(float time)
        {
            var anchorPosition = thisTransform.anchoredPosition;
            anchorPosition.x = start;
            thisTransform.anchoredPosition = anchorPosition;
            return thisTransform.DOAnchorPosX(end, time);
        }

        public Tween Hide(float time)
        {
            return thisTransform.DOAnchorPosX(start, time);
        }
    }
}