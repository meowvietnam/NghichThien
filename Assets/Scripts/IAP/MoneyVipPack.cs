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
            context.IapHandler.IapObjects[product.StoreProductId] = ui.gameObject;
            ui.SetMoneyVipText(MoneyVip);
            var icon = context.MoneyVipSprites[IconIndex];
            ui.SetIcon(icon);
            ui.SetPriceText(product.Price);
            ui.SetPurchaseAction(() => { context.IapHandler.BuyProduct(product.StoreProductId); });
        }
    }
}