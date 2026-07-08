using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SpecialLevelUpResult : Result
    {
        public StatsSnapshot StatsSnapshot { get; set; }
    }
}