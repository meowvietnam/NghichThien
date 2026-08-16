using System.Collections.Generic;
using CreateCharacter;
using LuyenDan;
using MemoryPack;
using Network;
using UnityEngine;

namespace IAP
{
    [MemoryPackable]
    public partial class StartPack : IIapReward
    {
        public int MoneyVip { get; set; }
        public int Money { get; set; }
        public ushort CongPhapId { get; set; }
        [MemoryPackIgnore] private string productId;

        public void Execute(CharacterData charData)
        {
            charData.IsInfiniteEnergy = true;
            charData.IsNoAds = true;
            charData.MoneyVip += MoneyVip;
            charData.Stats.LinhThach += Money;
            var inventory = charData.InventoryManager;
            var congPhapRef = new ItemRef()
            {
                Type = ItemType.CongPhap,
                Id = CongPhapId,
                Count = 1,
            };
            inventory.Add(congPhapRef);
        }


        public void Refund(CharacterData charData)
        {
            charData.IsInfiniteEnergy = false;
            charData.IsNoAds = false;
            charData.MoneyVip -= MoneyVip;
            charData.Stats.LinhThach -= Money;
            var inventory = charData.InventoryManager;
            var congPhapRef = new ItemRef()
            {
                Type = ItemType.CongPhap,
                Id = CongPhapId,
            };
            if (inventory.CongPhaps.TryGetValue(congPhapRef, out var congPhap))
            {
                congPhap.Count -= 1;
                if (congPhap.Count < 1)
                {
                    inventory.CongPhaps.Remove(congPhapRef);
                }
            }
        }

        public void ShowUISuccess(IapUIContext context)
        {
            var gameObjects = context.IapHandler.IapObjects[productId];
            foreach (var gameObject in gameObjects)
            {
                gameObject.SetActive(false);
            }
        }

        public void InitUI(IapUIContext context)
        {
            productId = context.OwnerIapProduct.Product.StoreProductId;
            if (context.OwnerIapProduct.HasPurchased)
            {
                return;
            }

            var prefab = context.StartPackUIPrefab;
            var iapHandler = context.IapHandler;
            var startPackUI = Object.Instantiate(prefab, iapHandler.transform);
            startPackUI.ThisPopup.Hide();
            var btnStartPack = Object.Instantiate(context.BtnStartPackPrefab, context.IconListContainer);
            btnStartPack.transform.SetAsLastSibling();
            BaseBehaviour.SetEventButton(btnStartPack, () => { startPackUI.ThisPopup.Show(); });
            var listGameObjects = new List<GameObject>();
            listGameObjects.Add(startPackUI.gameObject);
            listGameObjects.Add(btnStartPack.gameObject);
            iapHandler.IapObjects[productId] = listGameObjects;
            startPackUI.SetMoneyVipText(MoneyVip);
            startPackUI.SetMoneyText(Money);
            startPackUI.SetCongPhap(CongPhapId);
            startPackUI.SetPurchaseAction(() =>
            {
                CanBuyProduct(context);
            });
        }
        private async void CanBuyProduct(IapUIContext context)
        {
            var product = context.OwnerIapProduct.Product;
            var store = product.Store;
            var productId = product.StoreProductId;
            var handler = context.IapHandler;
            var result = await handler.CanBuyProduct(store, productId);
            if (result.Success)
            {
                handler.BuyProduct(productId);
            }
            else
            {
                NotifyGroup.Instance?.Show(result.Message);
            }
        }
    }
}