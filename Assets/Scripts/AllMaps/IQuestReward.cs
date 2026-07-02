using CreateCharacter;
using LuyenDan;
using MemoryPack;

namespace AllMaps
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(QuestMoneyReward))]
    [MemoryPackUnion(1, typeof(QuestMoneyVipReward))]
    public partial interface IQuestReward
    {
        public ItemRef Target { get; }
        void Claim(CharacterData charData);
    }
}