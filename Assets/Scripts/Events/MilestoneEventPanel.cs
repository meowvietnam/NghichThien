using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using DG.Tweening;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class MilestoneEventPanel : BaseBehaviour
    {
        private Event @event;
        [SerializeField] private Button closeButton;
        [SerializeField] private Text detailText;
        [SerializeField] private RectTransform content;
        [SerializeField] private ElementMilestone elementMilestonePrefab;
        private Tween tween;
        private List<ElementMilestone> elements = new();

        private void Start()
        {
            SetEventButton(closeButton, Hide);
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

        public void Init(Event @event)
        {
            foreach (var e in elements)
            {
                Destroy(e.gameObject);
            }

            elements.Clear();
            this.@event = @event;
            DateTime startAt = @event.StartAt.ToLocalTime();
            DateTime endAt = @event.EndAt.ToLocalTime();
            detailText.text =
                $"Sự kiện diễn ra từ {startAt:dd/MM/yyyy HH:mm} đến {endAt:dd/MM/yyyy HH:mm}" + "\n" +
                $"Thu thập vật phẩm đạt mốc nhận phần thưởng";
            var gameEvent = @event.GameEvent as MilestoneEvent;
            foreach (var milestone in gameEvent.Milestones)
            {
                var element = Instantiate(elementMilestonePrefab, content);
                elements.Add(element);
                element.Init(this, milestone);
            }
        }

        public async void OnClaim(ElementMilestone elementMilestone)
        {
            var milestone = elementMilestone.Milestone;
            var request = new HandleEventRequest()
            {
                EventId = @event.Id,
                Data = new MilestoneClaim()
                {
                    Id = milestone.Id
                },
            };
            var reader = await Client.Instance.MemRequestWait(MessageTag.HandleEventRequest, request);
            var result = reader.MemRead<IEventResultData>() as MilestoneClaimResult;
            if (result.Success)
            {
                foreach (var reward in result.Rewards)
                {
                    reward.Claim(Client.Instance.CharacterData);
                }

                NotifyGroup.Instance.Show(
                    $"Bạn nhận được: {string.Join(", ", result.Rewards.Select(x => x.GetRichText()))}");
                milestone.IsClaim = true;
                elementMilestone.UpdateButton();
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }
    }
}