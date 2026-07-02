using CreateCharacter;
using LuyenDan;
using MemoryPack;

namespace AllMaps
{
    [MemoryPackable]
    public partial class QuestMoneyReward : IQuestReward
    {
        public ItemRef Target { get; private set; }
        public int Money { get; private set; }
        public void Claim(CharacterData charData)
        {
            var inventory = charData.InventoryManager;
            inventory.Decrease(Target);
            var stats = charData.Stats;
            stats.LinhThach += Money;
        }
    }
}