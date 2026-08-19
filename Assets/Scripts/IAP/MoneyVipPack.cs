using System.Collections.Generic;
using CreateCharacter;
using MemoryPack;
using UnityEngine;

namespace IAP
{
    [MemoryPackable]
    public partial class MoneyVipPack : IIapReward
    {
        public int MoneyVip { get; set; }
        public int IconIndex { get; set; }

        public void Execute(CharacterData charData)
        {
            charData.MoneyVip += MoneyVip;
        }

        public void Refund(CharacterData charData)
        {
            charData.MoneyVip -= MoneyVip;
        }

        public void ShowUISuccess(IapUIContext context)
        {
        }

        public void InitUI(IapUIContext context)
        {
            var product = context.IapProduct;
            var ui = Object.Instantiate(context.MoneyVipPackUIPrefab, context.IapProductContainer);
            context.IapHandler.IapObjects[product.StoreProductId] = new List<GameObject>() { ui.gameObject };
            ui.SetMoneyVipText(MoneyVip);
            var icon = context.MoneyVipSprites[IconIndex];
            ui.SetIcon(icon);
            ui.SetPriceText(product.Price);
            ui.SetPurchaseAction(() =>
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