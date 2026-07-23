using LuyenDan;
using UnityEngine;

namespace ConvertElementalSkill
{
    public class CongPhapItemData : ISkillItemData
    {
        public SkillType SkillType => SkillType.CongPhap;
        public int Id { get; private set; }
        public Sprite Icon { get; private set; }
        public BaseSkill.ETypeSkill[] ETypeSkills { get; private set; }
        public string Name { get; private set; }
        public BaseSkill.ETierSkill TierSkill { get; private set; }
        public BaseSkill ItemData { get; private set; }
        public SkillVisualQuality VisualQuality { get; private set; }
        
        public CongPhapItemData(ItemRef itemRef, GameManager gameManager, UIManager ui)
        {
            Init(itemRef, gameManager, ui);
        }
        private void Init(ItemRef itemRef, GameManager gameManager, UIManager ui)
        {
            var item = gameManager.GetCongPhapWithId(itemRef.Id);
            ItemData = item;
            Id = itemRef.Id;
            Icon = item.spriteItem;
            Name = item.nameItem;
            TierSkill = item.eTierSkill;
            ETypeSkills = item.eTypeSkill;
            var visualQuality = ui.GetFrameAndQualityFullAndColorNameSkill(item);
            VisualQuality = new SkillVisualQuality(visualQuality.Item1, visualQuality.Item2, visualQuality.Item3);
        }
    }
}