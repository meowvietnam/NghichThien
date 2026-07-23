using LuyenDan;

namespace ConvertElementalSkill
{
    public class SkillData
    {
        public ItemRef ItemRef { get; }
        public ISkillItemData ItemData { get; }
        public SkillData(ItemRef itemRef, ISkillItemData itemData)
        {
            ItemRef = itemRef;
            ItemData = itemData;
        }
    }
}