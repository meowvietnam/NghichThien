using System.Linq;
using LuyenDan;
using Network;

namespace AllMaps.ItemProvider
{
    public class FurnaceProvider : ItemProvider<FurnaceData>
    {
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

        public override FurnaceData GetDataItemById(ushort Id)
        {
            return Client.Instance.CharacterData.InventoryManager.Furnaces.FirstOrDefault(x => x.SelfId == Id);
        }

        public override int GetPriceItemSell(ushort Id)
        {
            FurnaceData FurnaceData = GetDataItemById(Id);
            return FurnaceData.Durability * GameManager.PriceFurnaceWithTierPerDurability[LuyenDanResources.Instance.FindItem(FurnaceData).Tier];
        }
    }
}
