using System.Collections.Generic;
using MemoryPack;
using Rank;

namespace Events
{
    [MemoryPackable]
    public partial class RechargeMilestone1
    {
        public ushort Id { get; set; }
        public BaseSkill.ETypeSkill Type { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public List<IEventReward> Rewards { get; set; }
        public bool IsClaim { get; set; }
    }
}