using CreateCharacter;
using MemoryPack;

namespace Events
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(MilestoneEvent))]
    [MemoryPackUnion(1, typeof(RechargeEvent1))]
    public partial interface IGameEvent
    {
    }
}