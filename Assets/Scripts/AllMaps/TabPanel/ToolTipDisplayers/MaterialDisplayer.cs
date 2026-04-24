using LuyenDan;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class MaterialDisplayer : IToolTipDisplayer
    {
        public ItemType SupportedType => ItemType.Material;
        public void Display(ToolTipContext ctx)
        {
            ctx.DescriptionText.text = string.Empty;
        }
    }
}