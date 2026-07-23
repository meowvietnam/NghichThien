using UnityEngine;

namespace ConvertElementalSkill
{
    public class SkillVisualQuality
    {
        public Sprite Frame { get; }
        public Sprite QualityFull { get; }
        public Color Color { get; }

        public SkillVisualQuality(Sprite frame, Sprite qualityFull, Color color)
        {
            Frame = frame;
            QualityFull = qualityFull;
            Color = color;
        }
    }
    public interface ISkillItemData
    {
        SkillType SkillType { get; }
        int Id { get; }
        Sprite Icon { get; }
        BaseSkill.ETierSkill TierSkill { get; }
        BaseSkill.ETypeSkill[] ETypeSkills { get; }
        string Name { get; }
        BaseSkill ItemData { get;} 
        SkillVisualQuality VisualQuality { get; }
    }
}
