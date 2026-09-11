using System.Collections.Generic;
using MemoryPack;

namespace Events
{
    [MemoryPackable]
    public partial class MilestoneEvent : IGameEvent
    {
        public List<EventMilestone> Milestones { get; set; }
    }
}