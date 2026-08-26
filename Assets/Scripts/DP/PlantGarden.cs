using System.Collections.Generic;
using MemoryPack;
namespace DP
{
    [MemoryPackable]
    public partial class PlantGarden
    {
        public int CharId { get; set; }
        public HashSet<PlantingSlot> Slots { get; set; } = new();
        public HashSet<PlantSeed> Seeds { get; set; } = new();
    }
}