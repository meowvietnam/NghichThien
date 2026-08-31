using System;
using System.Collections.Generic;
using CreateCharacter;
using Events;
using LongDHModules;
using LuyenDan;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.EventReward
{
    public class EventRewardPanel : Popup, IUISingleton, INetworkEvent.IWatchAdsCombat, INetworkEvent.IWatchAdsSideQuest
    {
        [SerializeField] private Text txtDesc;
        [SerializeField] private Sprite spriteEvent;
        [SerializeField] private Sprite spriteWin;
        [SerializeField] private Sprite spriteLose;
        [SerializeField] private Image imgBGTilte;
        [SerializeField] private GameObject itemRewardPrefab;

        private ItemInventory itemInventoryPrefab =>
            itemRewardPrefab.transform.GetChild(0).GetComponent<ItemInventory>();

        [SerializeField] private Sprite spriteMoney;
        [SerializeField] private Sprite spriteMoneyVip;
        [SerializeField] private RewardSnapshot rewardDataCurrent;
        [SerializeField] private Button rewardAdsButton;
        [SerializeField] private GameObject rewardAdsObject;
        private Action onRewardAdsClick;

        [Button]
        public void InitShowEventReward(RewardSnapshot rewardData, string contentText, Action onRewardAdsClick = null)
        {
            SetEventRewardAds(onRewardAdsClick);
            InitShow(rewardData, contentText);
            imgBGTilte.sprite = spriteEvent;
            InitActionClose(null);
        }

        private void SetEventRewardAds(Action onRewardAdsClick)
        {
            this.onRewardAdsClick = onRewardAdsClick;
            var activeRewardAdsButton = onRewardAdsClick != null;
            rewardAdsObject.SetActive(activeRewardAdsButton);
        }

        public void InitShowCombatReward(RewardSnapshot rewardData, string contentText, ETypeResultCombat resultCombat,
            Action onRewardAdsClick = null)
        {
            SetEventRewardAds(onRewardAdsClick);
            InitShow(rewardData, contentText);
            if (resultCombat == ETypeResultCombat.Win)
            {
                imgBGTilte.sprite = spriteWin;
                InitActionClose(() => { Pool.Get<GameManager>().BackAllMapFromCombat(resultCombat); });
            }
            else
            {
                imgBGTilte.sprite = spriteLose;
                InitActionClose(() => { Pool.Get<GameManager>().BackAllMapFromCombat(resultCombat); });
            }
        }

        private void InitShow(RewardSnapshot rewardData, string contentText)
        {
            Show();
            Transform child = this.transform.GetChild(1);
            this.rewardDataCurrent = rewardData;
            child.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(this.rewardDataCurrent != null);
            child.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(this.rewardDataCurrent != null);
            child.transform.GetChild(2).gameObject.SetActive(this.rewardDataCurrent != null);
            SetReward();
            txtDesc.text = contentText;
        }

        private void SetVisualRewardMoney()
        {
            var itemReward = GetComponentItemReward();
            itemReward.itemInventory.InitIcon(spriteMoney, itemInventoryPrefab.frame.sprite);
            itemReward.itemInventory.SetQualityFull(itemInventoryPrefab.qualityFull.sprite);
            itemReward.itemInventory.textName.text = "Linh Thạch";
            itemReward.textCount.text = $"x{rewardDataCurrent.Money}";
            itemReward.itemInventory.btnShowInfor.gameObject.SetActive(false);
        }

        private void SetVisualRewardMoneyVip()
        {
            var itemReward = GetComponentItemReward();
            itemReward.itemInventory.InitIcon(spriteMoneyVip, itemInventoryPrefab.frame.sprite);
            itemReward.itemInventory.SetQualityFull(itemInventoryPrefab.qualityFull.sprite);
            itemReward.itemInventory.textName.text = "Tiên Ngọc";
            itemReward.textCount.text = $"x{rewardDataCurrent.MoneyVip}";
            itemReward.itemInventory.btnShowInfor.gameObject.SetActive(false);
        }

        private void SetVisualRewardItem(ItemRef itemRef)
        {
            var itemReward = GetComponentItemReward();
            ItemTypeHandlerBagFactory.Create(itemRef.Type).Handle(itemReward.itemInventory, itemRef, false);
            itemReward.textCount.text = $"x{itemRef.Count.ToString()}";
            itemReward.itemInventory.btnShowInfor.gameObject.SetActive(true);
        }

        (ItemInventory itemInventory, Text textCount) GetComponentItemReward()
        {
            var itemReward = GameManager.GetItemPoolWithPrefabInParent(itemRewardPrefab.transform);
            itemReward.transform.localScale = Vector3.one;
            ItemInventory itemInventory = itemReward.transform.GetChild(0).GetComponent<ItemInventory>();
            Text textCountReward = itemReward.transform.GetChild(1).GetComponent<Text>();
            return (itemInventory, textCountReward);
        }

        private void SetReward()
        {
            var parent = itemRewardPrefab.transform.parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(false);
            }

            if (rewardDataCurrent == null) return;
            if (rewardDataCurrent.Money > 0)
            {
                SetVisualRewardMoney();
            }

            if (rewardDataCurrent.MoneyVip > 0)
            {
                SetVisualRewardMoneyVip();
            }

            if (rewardDataCurrent?.ItemRefs != null)
            {
                foreach (var item in rewardDataCurrent.ItemRefs)
                {
                    SetVisualRewardItem(item);
                }
            }
            SetEventItems();
        }

        private void SetEventItems()
        {
            var items = rewardDataCurrent.itemEvents;
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                var data = item.GetItemEventData();
                var itemReward = GetComponentItemReward();
                itemReward.itemInventory.InitIcon(data.Icon, itemInventoryPrefab.frame.sprite);
                itemReward.itemInventory.SetQualityFull(itemInventoryPrefab.qualityFull.sprite);
                itemReward.itemInventory.textName.text = data.Name;
                itemReward.textCount.text = $"x{item.Count}";
                itemReward.itemInventory.btnShowInfor.gameObject.SetActive(false);
            }
        }

        public void Init()
        {
            Pool.Set(this);
            SetEventButton(rewardAdsButton, OnRewardAdsClick);
        }

        private void OnRewardAdsClick()
        {
            onRewardAdsClick?.Invoke();
        }

        public void SuccessWatchAdsCombat(RewardSnapshot reward)
        {
            SetEventRewardAds(null);
            rewardDataCurrent.AddReward(reward);
            SetReward();
            ShowEffect();
        }

        public void FailWatchAdsCombat(string message)
        {
            NotifyGroup.Instance.Show(message);
        }

        public void SuccessWatchAdsSideQuest(WatchAdsSideQuestResult result)
        {
            SetEventRewardAds(null);
            rewardDataCurrent.Money += result.AddMoney;
            rewardDataCurrent.MoneyVip += result.AddMoneyVip;
            SetReward();
            ShowEffect();
        }

        private void ShowEffect()
        {
            var parent = itemRewardPrefab.transform.parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.gameObject.activeInHierarchy)
                {
                    child.GetComponentInChildren<ParticleSystem>()?.Play();
                }
            }
        }

        public void FailWatchAdsSideQuest(string message)
        {
            NotifyGroup.Instance.Show(message);
        }
    }
}