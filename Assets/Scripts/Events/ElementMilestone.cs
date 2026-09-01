using System;
using DG.Tweening;
using Rank;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class ElementMilestone : BaseBehaviour
    {
        [SerializeField] private ItemEventMilestoneUI itemEventMilestoneUIPrefab;
        [SerializeField] private ItemRewardUI moneyVipPrefab;
        [SerializeField] private ItemRewardUI skillItemPrefab;
        [SerializeField] private ConvertElementalSkill.Prefabs.ItemUI itemUIPrefab;
        [SerializeField] private RectTransform milestoneContent;
        [SerializeField] private RectTransform content;
        [SerializeField] private Button button;
        [SerializeField] private Text buttonText;
        private MilestoneEventPanel panel;
        public EventMilestone Milestone { get; private set; }

        private void Start()
        {
            SetEventButton(button,OnClick);
        }

        private void OnClick()
        {
            panel.OnClaim(this);
        }

        public void Init(MilestoneEventPanel panel, EventMilestone milestone)
        {
            this.panel = panel;
            Milestone = milestone;
           
            for (int i = 0; i < milestone.Targets.Length; i++)
            {
                if (itemEventMilestoneUIPrefab == null)
                {
                    itemEventMilestoneUIPrefab = EventManager.Instance.ItemEventMilestoneUIPrefab;
                }
                var target = milestone.Targets[i];
                var progression = milestone.Progresses[i];
                var item = Instantiate(itemEventMilestoneUIPrefab, milestoneContent);
                item.Init(target, progression);
            }

            var context = new RewardUIContext()
            {
                Container = content,
                MoneyVipPrefab = moneyVipPrefab,
                SkillItemPrefab = skillItemPrefab,
                ItemUIPrefab = itemUIPrefab
            };
            foreach (var reward in milestone.Rewards)
            {
                reward.ShowUI(context);
            }
            UpdateButton();
        }

        public void UpdateButton()
        {
            if (Milestone.IsClaim)
            {
                buttonText.text = "Đã nhận";
                button.interactable = false;
            }
            else if (Milestone.IsCompleted())
            {
                buttonText.text = "Nhận thưởng";
                button.interactable = true;
            }
            else
            {
                buttonText.text = "Chưa đạt";
                button.interactable = false;
            }
        }
    }
}