using System.Collections.Generic;
using MemoryPack;
using Rank;

namespace Events
{
    [MemoryPackable]
    public partial class EventMilestone
    {
        public ushort Id { get; set; }
        public ItemEvent[] Targets { get; set; }
        public List<IEventReward> Rewards { get; set; }
        public bool IsClaim { get; set; }
        public int[] Progresses { get; private set; }

        public bool IsCompleted()
        {
            var progresses = Progresses;
            for (int i = 0; i < progresses.Length; i++)
            {
                var progress = progresses[i];
                var target = Targets[i];
                if (progress < target.Count)
                {
                    return false;
                }
            }

            return true;
        }
    }
}