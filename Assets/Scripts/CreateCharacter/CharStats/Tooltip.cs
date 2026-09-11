using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter.CharStats
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private GameObject thisObject;
        [SerializeField] protected Text text;
        [SerializeField] private Button closeButton;
        [SerializeField] public RectTransform thisTransform;
        private Tween tween;
        public static Tooltip Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            closeButton?.onClick.AddListener(DoHide);
            SetActive(false);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
            Instance = null;
        }

        public virtual void SetText(string effective, string description)
        {
            var text = $"<color=#6495ED>Hiệu quả: </color>{effective}\n\n<color=#FF7F50>Mô tả: </color>{description}";
            this.text.text = text;
        }

        public virtual void SetText(string des)
        {
            this.text.text = des;
        }

        public virtual void SetTextInfo(string infoName, string info)
        {
            var text = $"<color=#6495ED>{infoName}: </color>{info}";
            this.text.text = text;
        }

        public void DoShow(int sign = 1, System.Action completeAction = null)
        {
            tween?.Kill();
            thisTransform.localScale = Vector3.zero;
            SetActive(true);
            tween = thisTransform.DOScale(new Vector3(1 * sign, 1, 1) * 1.5f, 0.15f).OnComplete(() =>
            {
                completeAction?.Invoke();
            });
        }

        public void DoHide()
        {
            tween?.Kill();
            tween = thisTransform.DOScale(Vector3.zero, 0.15f).OnComplete(() => SetActive(false));
        }

        public void SetScale(Vector2 scale)
        {
            thisTransform.localScale = scale;
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }

        public void SetPosition(Vector3 position)
        {
            thisTransform.position = position;
        }
    }
}