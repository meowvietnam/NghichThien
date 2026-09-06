using System;
using Base;
using ConvertElementalSkill.Prefabs;
using CreateCharacter;
using LongDHModules;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using Rank;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class ElementShopEvent : BaseBehaviour
    {
        [SerializeField] private Button buyWithMoneyVipBtn;
        [SerializeField] private Button createOrderButton;

        [SerializeField] private Text moneyVipText;
        [SerializeField] private Text amountText;
        private ItemEventShop itemEvent;
        private ushort eventId;

        [Header("Context")] 
        [SerializeField] private ConvertElementalSkill.Prefabs.ItemUI itemUIPrefab;
        [SerializeField] private SkillItemUI skillItemUIPrefab;
        [SerializeField] private RectTransform itemContainer;
        [SerializeField] private ItemRewardUI moneyVipPrefab;
        [SerializeField] private ItemRewardUI randomSkillItemPrefab;

        private void Start()
        {
            SetEventButton(buyWithMoneyVipBtn, OnBuyWithMoneyVip);
            SetEventButton(createOrderButton, CreateOrder);
        }

        private async void CreateOrder()
        {
            var request = new HandleEventRequest()
            {
                EventId = eventId,
                Data = new CreateOrderEventShop()
                {
                    Id = itemEvent.Id
                }
            };
            var reader = await Client.Instance.MemRequestWait(MessageTag.HandleEventRequest, request);
            var result = reader.MemRead<IEventResultData>() as CreateOrderEventShopResult;
            if (result.Success)
            {
                Application.OpenURL(result.Link);
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }

        private void OnBuyWithMoneyVip()
        {
            var popup = Pool.Get<PopupConfirmAction>();
            popup.ShowPopConfirmAction($"Xác nhận mua bằng {Utils.FormatCount(itemEvent.MoneyVip)} Tiên ngọc", ConfirmBuy);
        }

        private async void ConfirmBuy()
        {
            var request = new HandleEventRequest()
            {
                EventId = eventId,
                Data = new BuyItemEventShop()
                {
                    Id = itemEvent.Id
                }
            };
            var reader = await Client.Instance.MemRequestWait(MessageTag.HandleEventRequest, request);
            var result = reader.MemRead<IEventResultData>() as BuyItemEventShopResult;
            NotifyGroup.Instance.Show(result.Message);
            if (result.Success)
            {
                Client.Instance.CharacterData.MoneyVip = result.NewMoneyVip;
                itemEvent.HasBuy = true;
                UpdateButton();
            }
        }
        private void UpdateButton()
        {
            var buttonActive = !itemEvent.HasBuy;
            buyWithMoneyVipBtn.interactable = buttonActive;
            createOrderButton.interactable = buttonActive;
        }

        private void UpdateTextButton()
        {
            moneyVipText.text = Utils.FormatCount(itemEvent.MoneyVip);
            amountText.text = $"{Utils.FormatCount(itemEvent.Amount)} VNĐ";
        }
        public void Init(ushort eventId, ItemEventShop item)
        {
            this.eventId = eventId;
            this.itemEvent = item;
            var context = CreateContext();
            item.Reward.ShowUI(context);
            UpdateButton();
            UpdateTextButton();
        }
        
        private RewardUIContext CreateContext()
        {
            return new RewardUIContext()
            {
                MoneyVipPrefab = moneyVipPrefab,
                RandomSkillItemPrefab = randomSkillItemPrefab,
                ItemUIPrefab = itemUIPrefab,
                SkillItemUIPrefab = skillItemUIPrefab,
                Container = itemContainer
            };
        }
    }
}
