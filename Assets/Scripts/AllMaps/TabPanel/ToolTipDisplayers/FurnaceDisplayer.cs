using System.Linq;
using LuyenDan;
using Network;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class FurnaceDisplayer : IToolTipDisplayer
    {
        public ItemType SupportedType => ItemType.Furnace;

        public void Display(ToolTipContext ctx)
        {
            var furnaceData = ctx.FurnaceData;
            ctx.TypeText.text = "Lò Luyện Đan";
            ctx.DescriptionText.text = "Đan lô vật phẩm bắt buộc để có thể luyện chế đan dược";
            ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
            ctx.DescTextItem.text = $"<color=#ffb143>【Bền Bỉ】</color> {furnaceData.Durability}/100";
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
            ctx.SetPriceText?.Invoke(GetPrice(ctx));
        }
        private int GetPrice(ToolTipContext ctx)
        {
            return ctx.ActionShowItem == EActionShowItemInfor.None || ctx.ActionShowItem == EActionShowItemInfor.Sell
                ? GetPriceItemSell(ctx)
                : GetPriceItemBuy(ctx);
        }

        private int GetPriceItemSell(ToolTipContext ctx)
        {
            return ctx.FurnaceData.Durability * GameManager.PriceFurnaceWithTier[ctx.IItemData.Tier];
        }

        private int GetPriceItemBuy(ToolTipContext ctx)
        {
            return GetPriceItemSell(ctx) * 3;
        }
    }
}
