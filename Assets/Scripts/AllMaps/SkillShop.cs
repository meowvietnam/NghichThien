using MemoryPack;

namespace AllMaps
{
    [MemoryPackable]
    public partial class SkillShop : Shop
    {
        public override EShopType GetTypeShop()
        {
            return EShopType.Skill;
        }
    }
}