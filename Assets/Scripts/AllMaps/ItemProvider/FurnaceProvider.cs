using System.Linq;
using LuyenDan;
using Network;
using UnityEngine;

namespace AllMaps.ItemProvider
{
    public class FurnaceProvider : ItemProvider<FurnaceData>
    {
        public override int BaseValueLevelWithTier => 3;

        public override ItemType SupportedType => ItemType.Furnace;

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);

            var furnaceData = ctx.FurnaceData ?? new FurnaceData
            {
                Id = ctx.ItemRef.TempId != 0 ? ctx.ItemRef.TempId : ctx.ItemRef.Id,
                Durability = 100,
            };
            ctx.TypeText.text = "Lò Luyện Đan";
            ctx.DescriptionText.text = "Đan lô vật phẩm bắt buộc để có thể luyện chế đan dược";
            ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
            ctx.DescTextItem.text = $"<color=#ffb143>【Bền Bỉ】</color> {furnaceData.Durability}/100";
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
        }

        public override int GetPrice(ToolTipContext ctx)
        {
            var fData = ctx.FurnaceData;
            if (fData != null)
            {
                return ctx.ActionShowItem == EActionShowItemInfor.None || ctx.ActionShowItem == EActionShowItemInfor.Sell
                    ? GetPriceItemSell(fData.SelfId)
                    : GetPriceItemBuy(fData.SelfId);
            }
            return base.GetPrice(ctx);
        }

        public override FurnaceData GetDataItemByIdRef(ushort id)
        {
            var inventoryManager = Client.Instance.CharacterData.InventoryManager;
            var furnaceData = new FurnaceData()
            {
                SelfId = id,
                Durability = 100,
            };
            if (inventoryManager.Furnaces.TryGetValue(furnaceData, out var furnace))
            {
                return furnace;
            }

            return null;
        }

        public override int GetPriceItemSell(ushort id)
        {
            FurnaceData furnaceData = GetDataItemByIdRef(id);
            if (furnaceData == null)
            {
                furnaceData = new FurnaceData()
                {
                    SelfId = 0,
                    Id = id,
                    Durability = 100,
                };
            }
            var item = LuyenDanResources.Instance.FindItem(furnaceData);
            var basePrice = GameManager.PriceFurnaceWithTierPerDurability[item.Tier];
            return furnaceData.Durability * basePrice;
        }

        public override (int, int) ValueTangQuaNPCWithItem(ushort id, int npcLevel)
        {
            int currentDurability = GetDataItemByIdRef(id).Durability;
            // Tính số độ bền bị mất, đảm bảo không bị âm và tối đa là 100
            int lostDurability = Mathf.Clamp(100 - currentDurability, 0, 100);
            // Cứ mỗi 25 điểm mất đi thì tính là 1 điểm phạt
            int penalty = lostDurability / 25;

            return (base.ValueTangQuaNPCWithItem(id, npcLevel).Item1 - penalty,
                base.ValueTangQuaNPCWithItem(id, npcLevel).Item2);
        }
    }
}
