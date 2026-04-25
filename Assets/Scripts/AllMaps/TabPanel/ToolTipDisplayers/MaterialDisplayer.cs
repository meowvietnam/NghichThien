using LuyenDan;
using LuyenKhi;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class MaterialDisplayer : IToolTipDisplayer
    {
        public ItemType SupportedType => ItemType.Material;
        public void Display(ToolTipContext ctx)
        {
            var material = (Material)ctx.IItemData;
            ctx.DescriptionText.text = material.Desc1;
            ctx.DescTextItem.text = material.Desc2;
            ctx.SetPriceText(material.Price);
        }
    }
}