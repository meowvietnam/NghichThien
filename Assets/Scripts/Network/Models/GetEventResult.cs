using Events;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class GetEventResult : Result
    {
        public Event Event { get; set; }
    }
}