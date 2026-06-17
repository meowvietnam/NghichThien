using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using MemoryPack;

namespace AllMaps.EventReward
{
    [System.Serializable]
    [MemoryPackable]
    public partial class RewardSnapshot
    {
        public List<ItemRef> ItemRefs = new();
        public int Money = 0;
        public int MoneyVip = 0;
    }
}