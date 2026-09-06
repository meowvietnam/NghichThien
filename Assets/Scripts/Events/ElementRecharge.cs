using System;
using Base;
using Rank;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class ElementRecharge : BaseBehaviour
    {
        [SerializeField] private Text amountText;
        [SerializeField] private ItemRewardUI moneyVipPrefab;
        [SerializeField] private ItemRewardUI skillItemPrefab;
        [SerializeField] private ConvertElementalSkill.Prefabs.ItemUI itemUIPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private Button buyButton;
        private EventRechargePanel panel;
        private RechargeMilestone1 milestone;

        public bool IsThis(ushort id)
        {
            return milestone.Id == id;
        }
        private void Awake()
        {
            SetEventButton(buyButton, OnBuy);
        }

        private void OnBuy()
        {
            if (milestone == null)
            {
                return;
            }

            panel.OnBuy(milestone);
        }

        public void Init(EventRechargePanel panel,RechargeMilestone1 milestone)
        {
            this.panel = panel;
            this.milestone = milestone;
            amountText.text = $"{Utils.FormatCount(milestone.Amount)} VNĐ";
            var context = new RewardUIContext()
            {
                Container = content,
                MoneyVipPrefab = moneyVipPrefab,
                RandomSkillItemPrefab = skillItemPrefab,
                ItemUIPrefab = itemUIPrefab
            };
            UpdateButton();
            foreach (var eventReward in milestone.Rewards)
            {
                eventReward.ShowUI(context);
            }
        }

        public void UpdateButton()
        {
            buyButton.interactable = !milestone.IsClaim;
        }
    }
}