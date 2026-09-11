using System.Collections.Generic;
using MemoryPack;

namespace Events
{
    [MemoryPackable]
    public partial class EventShop : IGameEvent
    {
        public string Title { get; set; }
        public List<ItemEventShop> Items { get; set; }
    }
}