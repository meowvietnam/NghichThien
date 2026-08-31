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
    public class EventRechargePanel : BaseBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Text detailText;
        private Tween tween;
        private Event @event;
        private Dictionary<BaseSkill.ETypeSkill, List<RechargeMilestone1>> milestoneDict;
        private BaseSkill.ETypeSkill currentSelect = BaseSkill.ETypeSkill.Kim;
        private bool isInit;
        [SerializeField] private RectTransform content;
        [SerializeField] private ElementRecharge elementRechargePrefab;
        private void Start()
        {
            SetEventButton(closeButton, OnClose);
        }

        private void OnClose()
        {
            Hide();
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
            this.@event = @event;
         

            var gameEvent = @event.GameEvent as RechargeEvent1;
            milestoneDict = gameEvent.Milestones
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => 
                    x.OrderBy(i => i.Amount).ToList());
            isInit = true;
            SelectSkillType(currentSelect);
        }

        public void OnSelected(BaseSkill.ETypeSkill type)
        {
            currentSelect = type;
            if (!isInit) return;
            SelectSkillType(type);
        }

        private void SelectSkillType(BaseSkill.ETypeSkill type)
        {
            DateTime startAt = @event.StartAt.ToLocalTime();
            DateTime endAt = @event.EndAt.ToLocalTime();
            detailText.text =
                $"Sự kiện diễn ra từ {startAt:dd/MM/yyyy HH:mm} đến {endAt:dd/MM/yyyy HH:mm} (không tính vào xếp hạng tổng nạp)" + "\n"+
                $"Kỹ năng nhận được hệ {BaseSkill.GetTextTypeSkill(type)}";
            var milestones = milestoneDict[type];
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
            foreach (var milestone in milestones)
            {
                var elementRecharge = Instantiate(elementRechargePrefab, content);
                elementRecharge.Init(this,milestone);
            }
        }
        public async void OnBuy(RechargeMilestone1 milestone)
        {
            var request = new HandleEventRequest()
            {
                EventId = @event.Id,
                Data = new CreateOrderRechargeEvent1()
                {
                    Id = milestone.Id
                },
            };
            var reader = await Client.Instance.MemRequestWait(MessageTag.HandleEventRequest, request);
            var result = reader.MemRead<IEventResultData>() as RechargeEvent1Result;
            if (result.Success)
            {
                Application.OpenURL(result.Link);
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }
    }
}