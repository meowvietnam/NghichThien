using System.Collections.Generic;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhu
    {
        public int CharId { get; set; }
        public int LikeCount { get; set; }
        public string Name { get; set; }
        public DongPhuLevel Level { get; set; }
        public PlantGarden Garden { get; set; } = new();
    }
}