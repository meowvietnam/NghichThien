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

            var furnaceData = ctx.FurnaceData;
            ctx.TypeText.text = "Lò Luyện Đan";
            ctx.DescriptionText.text = "Đan lô vật phẩm bắt buộc để có thể luyện chế đan dược";
            ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
            ctx.DescTextItem.text = $"<color=#ffb143>【Bền Bỉ】</color> {furnaceData.Durability}/100";
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
        }

        public override FurnaceData GetDataItemByIdRef(ushort Id)
        {
            return Client.Instance.CharacterData.InventoryManager.Furnaces.FirstOrDefault(x => x.SelfId == Id);
        }

        public override int GetPriceItemSell(ushort Id)
        {
            FurnaceData FurnaceData = GetDataItemByIdRef(Id);
            return FurnaceData.Durability * GameManager.PriceFurnaceWithTierPerDurability[LuyenDanResources.Instance.FindItem(FurnaceData).Tier];
        }
        public override int ValueTangQuaNPCWithItem(ushort id, int npcLevel)
        {
            int currentDurability = GetDataItemByIdRef(id).Durability;
            // Tính số độ bền bị mất, đảm bảo không bị âm và tối đa là 100
            int lostDurability = Mathf.Clamp(100 - currentDurability, 0, 100);
            // Cứ mỗi 25 điểm mất đi thì tính là 1 điểm phạt
            int penalty = lostDurability / 25;

            return base.ValueTangQuaNPCWithItem(id, npcLevel) - penalty;
        }
    }
}
