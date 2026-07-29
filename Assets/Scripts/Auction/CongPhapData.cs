using ConvertElementalSkill;
using LongDHModules;
using UnityEngine;

namespace Auction
{
    public class CongPhapData : ISkillItemData, IItemAuctionData
    {
        public SkillType SkillType => SkillType.CongPhap;
        public int Id { get; }
        public Sprite Icon { get; }
        public Color Color { get; }
        public Sprite QualityBackground { get; }
        public Sprite QualityIconUp { get; }
        public BaseSkill.ETierSkill TierSkill { get; }
        public BaseSkill.ETypeSkill[] ETypeSkills { get; }
        public string Name { get; }
        public BaseSkill ItemData { get; }
        public SkillVisualQuality VisualQuality { get; }

        public CongPhapData(int id)
        {
            var ui = Pool.Get<UIManager>();
            var gameManager = Pool.Get<GameManager>();
            var item = gameManager.GetCongPhapWithId(id);
            ItemData = item;
            Id = id;
            Icon = ui.spriteTTInven;
            Name = item.nameItem;
            TierSkill = item.eTierSkill;
            ETypeSkills = item.eTypeSkill;
            var visualQuality = ui.GetFrameAndQualityFullAndColorNameSkill(item);
            VisualQuality = new SkillVisualQuality(visualQuality.Item1, visualQuality.Item2, visualQuality.Item3);
            QualityIconUp = VisualQuality.Frame;
            QualityBackground = VisualQuality.QualityFull;
            Color = VisualQuality.Color;
        }

        public void ShowInfo(ToolTipItems toolTip)
        {
            toolTip.ShowInfoSkill(this);
        }
    }
}