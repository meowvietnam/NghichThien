using CreateCharacter;
using LuyenDan;
using MemoryPack;

namespace IAP
{
    [MemoryPackable]
    public partial class StartPack : IIapReward
    {
        public int MoneyVip { get; set; }
        public int Money { get; set; }
        public ushort CongPhapId { get; set; }
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
            if(inventory.CongPhaps.TryGetValue(congPhapRef, out var congPhap))
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

        }
        public void InitUI(IapUIContext context)
        {
            if (context.OwnerIapProduct.HasPurchased)
            {
                return;
            }
            var prefab = context.StartPackUIPrefab;
            var iapHandler = context.IapHandler;
            var startPackUI = UnityEngine.Object.Instantiate(prefab, iapHandler.transform);
            startPackUI.SetMoneyVipText(MoneyVip);
            startPackUI.SetMoneyText(Money);
            startPackUI.SetCongPhap(CongPhapId);
            startPackUI.SetPurchaseAction(() =>
            {
                context.IapHandler.BuyProduct(context.IapProduct.StoreProductId);
            });
        }
    }
}