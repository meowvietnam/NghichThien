using System.Collections.Generic;
using CreateCharacter;
using MemoryPack;
using Network;
using UnityEngine;

namespace IAP
{
    [MemoryPackable]
    public partial class MonthlyPassVipPack : IIapReward
    {
        public int MoneyVipPerDay { get; set; }
        public void ShowUISuccess(IapUIContext context)
        {
            var productId = context.ProductId;
            var gameObjects = context.IapHandler.IapObjects[productId];
            foreach (var gameObject in gameObjects)
            {
                gameObject.SetActive(false);
            }
        }

        public void InitUI(IapUIContext context)
        {
            var product = context.IapProduct;
            var ui = Object.Instantiate(context.MonthlyPassPackUIPrefab, context.IapProductContainer);
            ui.SetDataIndex(1, context.IapHandler.transform);
            ui.SetName(product.Name);
            
            var panel = ui.GetPanel();
            panel.SetMoney(MoneyVipPerDay);
            panel.SetPurchaseEvent(() => CanBuyProduct(context));
            context.IapHandler.IapObjects[product.StoreProductId] = new List<GameObject>()
            {
                ui.gameObject, 
                panel.gameObject
            };
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