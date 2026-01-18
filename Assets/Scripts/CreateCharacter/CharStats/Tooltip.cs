using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter.CharStats
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private GameObject thisObject;
        [SerializeField] private Text text;
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform thisTransform;
        private Tween tween;
        public static Tooltip Instance { get; private set; }
        private void Start()
        {
            Instance = this;
            closeButton.onClick.AddListener(DoHide);
            SetActive(false);
        }
        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
            Instance = null;
        }
        
        public void SetText(string effective, string description)
        {
            var text = $"<color=#6495ED>Hiệu quả: </color>{effective}\n\n<color=#FF7F50>Mô tả: </color>{description}";
            this.text.text = text;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(thisTransform);
        }

        public void SetTextInfo(string infoName, string info)
        {
            var text = $"<color=#6495ED>{infoName}: </color>{info}";
            this.text.text = text;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(thisTransform);
        }
        public void DoShow()
        {
            tween?.Kill();
            thisTransform.localScale = Vector3.zero;
            SetActive(true);
            tween = thisTransform.DOScale(Vector3.one, 0.15f);
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