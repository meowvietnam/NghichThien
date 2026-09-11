using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class BasePanel : BaseBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private RectTransform background;
        [SerializeField] private Button confirmBtn;
        private Tween tween;

        private void Awake()
        {
            SetEventButton(confirmBtn, OnConfirm);
        }

        protected void SetConfirmButtonInteractable(bool isOpen)
        {
            confirmBtn.interactable = isOpen;
        }

        private void OnConfirm()
        {
            Hide();
            Confirm();
        }

        protected virtual void Confirm()
        {
        }

        private void OnDestroy()
        {
            confirmBtn.onClick.RemoveAllListeners();
        }

        public void Show(float duration = 0.25f)
        {
            tween?.Kill();
            selfObject.SetActive(true);
            tween = background.DOScale(Vector3.one, duration);
        }

        public void Hide(float duration = 0.25f)
        {
            tween?.Kill();
            tween = background.DOScale(Vector3.zero, duration).OnComplete(() => selfObject.SetActive(false));
        }
    }
}