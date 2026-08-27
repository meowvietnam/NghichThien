using System;
using Base;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class ShopItem : BaseBehaviour
    {
        [SerializeField] private DongPhuSeedUI seedUI;
        [SerializeField] private Text priceText;
        private SeedItemShop seedItemShop;
        private void Start()
        {
            seedUI.Clicked += OnSeedClick;
        }
        private void OnSeedClick()
        {
            var dongPhu = DongPhuPanel.Instance;
            var selectNum = dongPhu.SelectNum;
            var herbData = seedItemShop.GetHerb();
            var quality = CoreConfig.Instance.FindQuality(herbData.Tier);
            void UpdateText(int count)
            {
                var price = count * seedItemShop.PricePerSeed;
                var text = $"Xác nhận mua x{Utils.FormatCount(count)} hạt giống <color=#{ColorUtility.ToHtmlStringRGB(quality.Color)}>{herbData.Name}</color> sử dụng {Utils.FormatCount(price)} Linh thạch";
                selectNum.SetText(text);
            }
            selectNum.SetMin(1);
            selectNum.SetMax(seedItemShop.Count);
            selectNum.SetValue(1);
            UpdateText(selectNum.GetValue());
            selectNum.ValueChanged += UpdateText;
            selectNum.confirmAction.RemoveAllListeners();
            selectNum.confirmAction.AddListener(() =>
            {
                OnConfirm(selectNum.GetValue());
            });
            selectNum.Show(0.5f);
        }
        private async void OnConfirm(int count)
        {
            var request = new BuySeed()
            {
                HerbId = seedItemShop.HerbId,
                Count = count
            };
            var client = Client.Instance;
            var reader = await client.MemRequestWait(MessageTag.BuySeedRequest, request);
            var result = reader.MemRead<BuySeedResult>();
            DongPhuPanel.Instance.OnBuySeedResult(result);
        }
        public void Init(SeedItemShop itemShop)
        {
            seedItemShop = itemShop;
            seedUI.Init(itemShop.HerbId, itemShop.Count);
            UpdateCount();
            itemShop.OnCountChanged += OnCountChanged; 
            priceText.text = Utils.FormatCount(itemShop.PricePerSeed);
        }

        private void OnCountChanged(int count)
        {
            UpdateCount();
        }
        private void UpdateCount()
        {
            var count = seedItemShop.Count;
            Debug.Log(count);
            if (count <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                seedUI.SetCount(count);
            }
        }

        private void OnDestroy()
        {
            seedItemShop.OnCountChanged -= OnCountChanged;
        }
    }
}