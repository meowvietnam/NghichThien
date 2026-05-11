using LuyenDan;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public abstract class SkillDisplayer : ItemDisplayer
    {

        public abstract override void Display(ToolTipContext ctx);
       
        public override int GetPriceItemSell(DataItemSnapshot ctx)
        {
            if (ctx.BaseItemData is BaseSkill skill)
            {
                return GameManager.PriceTTWithTier[skill.eTierSkill];

            }
            return 0;

        }
    }
    public class ThanThongBookDisplayer : SkillDisplayer
    {
        public override ItemType SupportedType => ItemType.ThanThong;

        public override void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Sách Thần Thông";
        }
        
    }
    
    public class CongPhapBookDisplayer : SkillDisplayer
    {
        public override ItemType SupportedType => ItemType.CongPhap;

        public override void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Sách Công Pháp";
        }
    }
}