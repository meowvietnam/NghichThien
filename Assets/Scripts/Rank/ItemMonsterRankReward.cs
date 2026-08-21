using CreateCharacter;
using LuyenDan;
using MemoryPack;

namespace Rank
{
    [MemoryPackable]
    public partial class ItemMonsterRankReward : IMonsterRankReward
    {
        public ItemRef Item { get; set; }
        public void Claim(CharacterData charData)
        {
            charData.InventoryManager.Add(Item);
        }
    }
}