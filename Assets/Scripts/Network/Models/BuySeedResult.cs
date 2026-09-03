using DP;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class BuySeedResult : Result
    {
        public Seed Seed { get; set; }
        public SeedItemShop UpdatedSeed { get; set; }
        public int NewLinhThach { get; set; }
    }
}