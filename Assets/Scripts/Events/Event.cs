using System;
using MemoryPack;

namespace Events
{
    [MemoryPackable]
    public partial class Event
    {
        public int CharId { get; set; }
        public ushort Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public IGameEvent GameEvent { get; set; }
        public bool IsInEvent()
        {
            var now = DateTime.UtcNow;
            return now > StartAt &&  now < EndAt;
        }
    }
}