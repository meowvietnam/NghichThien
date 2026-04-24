using LuyenDan;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class ThanThongBookDisplayer : IToolTipDisplayer
    {
        public ItemType SupportedType => ItemType.ThanThong;

        public void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Sách Thần Thông";
        }
    }

    public class CongPhapBookDisplayer : IToolTipDisplayer
    {
        public ItemType SupportedType => ItemType.CongPhap;

        public void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Sách Công Pháp";
        }
    }
}