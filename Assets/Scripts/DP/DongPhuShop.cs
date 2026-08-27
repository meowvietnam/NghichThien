using System.Collections.Generic;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuShop
    {
        public int CharId { get; set; }
        public HashSet<SeedItemShop> Seeds { get; set; } = new();
    }
}