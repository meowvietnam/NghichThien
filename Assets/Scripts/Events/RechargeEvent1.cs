using System.Collections.Generic;
using MemoryPack;
namespace Events
{
    [MemoryPackable]
    public partial class RechargeEvent1 : IGameEvent
    {
        public List<RechargeMilestone1> Milestones { get; set; }
    }
}