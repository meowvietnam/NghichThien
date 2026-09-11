using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class ShopEventPanel : BaseBehaviour
    {
        [SerializeField] private ElementShopEvent elementPrefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private Text titleText;
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform rootTf;
        private List<ElementShopEvent> elements = new();
        private Event @event;
        private Tween tween;
        
        private void Start()
        {
            SetEventButton(closeButton, Hide);
        }

        public void Init(Event @event)
        {
            this.@event = @event;
            var gameEvent = @event.GameEvent as EventShop;
            var startAt = @event.StartAt.ToLocalTime();
            var endAt = @event.EndAt.ToLocalTime();
            titleText.text = $"Sự kiện diễn ra từ {startAt:dd/MM/yyyy HH:mm} đến {endAt:dd/MM/yyyy HH:mm}" + "\n"
                + gameEvent.Title;
            foreach (var e in elements)
            {
                Destroy(e.gameObject);
            }

            elements.Clear();
            foreach (var item in gameEvent.Items)
            {
                var element = Instantiate(elementPrefab, container);
                element.Init(@event.Id, item);
                elements.Add(element);
            }
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rootTf);
        }

        public void Hide()
        {
            tween?.Kill();
            tween = transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => { gameObject.SetActive(false); });
        }

        public void Show()
        {
            tween?.Kill();
            gameObject.SetActive(true);
            tween = transform.DOScale(Vector3.one, 0.25f);
        }
    }
}