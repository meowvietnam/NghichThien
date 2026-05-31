using Base;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;

namespace AllMaps.ItemProvider
{
    public abstract class SkillProvider : ItemProvider<BaseSkill>
    {

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);
        }
       
        public override int GetPriceItemSell(ushort Id)
        {
            var data = GetDataItemById(Id);
            if (data)
            {
                return GameManager.PriceTTWithTier[data.eTierSkill];

            }
            return 0;

        }
        
    }
    public class ThanThongBooProvider : SkillProvider
    {
        public override ItemType SupportedType => ItemType.ThanThong;

        public override void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Sách Thần Thông";
        }

        public override BaseSkill GetDataItemById(ushort Id)
        {
            return Pool.Get<GameManager>().GetThanThongWithId(Id);
        }
    }
    
    public class CongPhapBookProvider : SkillProvider
    {
        public override ItemType SupportedType => ItemType.CongPhap;

        public override void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Sách Công Pháp";
        }

        public override BaseSkill GetDataItemById(ushort Id)
        {
            return Pool.Get<GameManager>().GetCongPhapWithId(Id);
        }
    }
}