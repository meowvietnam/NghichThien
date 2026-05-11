
using LuyenDan;
using LuyenKhi;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class MaterialDisplayer : ItemDisplayer
    {
        public override ItemType SupportedType => ItemType.Material;
        public override void Display(ToolTipContext ctx)
        {
            var material = (Material)ctx.IItemData;
            ctx.TypeText.text = material.Type;
            ctx.DescriptionText.text = material.Desc1;
            ctx.DescTextItem.text = material.Desc2;
            ctx.SetPriceText(material.Price);
        }
        public override int GetPriceItemSell(DataItemSnapshot ctx)
        {
            return  GameManager.PriceVatLieuWithTier[ctx.IItemData.Tier];

        }
    }
}