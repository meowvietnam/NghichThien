using System.Collections.Generic;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuInventory
    {
        public int CharId { get; set; }
        public HashSet<Seed> Seeds { get; set; } = new();
    }
}