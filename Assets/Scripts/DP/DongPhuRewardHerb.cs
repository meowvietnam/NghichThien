using Base;
using CreateCharacter;
using LuyenDan;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuRewardHerb : IDongPhuReward
    {
        public ItemRef Herb { get; set; }

        public void Claim(CharacterData charData)
        {
            charData.InventoryManager.Add(Herb);
        }

        public string GetText()
        {
            var herbData = LuyenDanResources.Instance.FindHerb(Herb.Id);
            return $"x{Utils.FormatCount(Herb.Count)} {herbData.Name}";
        }
    }
}