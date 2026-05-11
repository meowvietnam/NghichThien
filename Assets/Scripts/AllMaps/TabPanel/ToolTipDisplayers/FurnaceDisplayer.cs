using System.Linq;
using LuyenDan;
using Network;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class FurnaceDisplayer : ItemDisplayer
    {
        public override ItemType SupportedType => ItemType.Furnace;

        public override void Display(ToolTipContext ctx)
        {
            var furnaceData = ctx.FurnaceData;
            ctx.TypeText.text = "Lò Luyện Đan";
            ctx.DescriptionText.text = "Đan lô vật phẩm bắt buộc để có thể luyện chế đan dược";
            ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
            ctx.DescTextItem.text = $"<color=#ffb143>【Bền Bỉ】</color> {furnaceData.Durability}/100";
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
            ctx.SetPriceText?.Invoke(GetPrice(ctx));
        }
      
        public override int GetPriceItemSell(DataItemSnapshot ctx)
        {
            return ctx.FurnaceData.Durability * GameManager.PriceFurnaceWithTierPerDurability[ctx.IItemData.Tier];
        }

     

       
    }
}
