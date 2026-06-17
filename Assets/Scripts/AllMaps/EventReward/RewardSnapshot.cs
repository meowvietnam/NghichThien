using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace AllMaps.EventReward
{
    [System.Serializable]
    [MemoryPackable]
    public partial class RewardSnapshot
    {
        public List<ItemRef> ItemRefs { get; set; }
        public int Money { get; set; }
        public int MoneyVip { get; set; }

        public RewardSnapshot()
        {
            
        }
    }
}