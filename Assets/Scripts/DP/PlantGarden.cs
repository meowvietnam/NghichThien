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

        public PlantingSlot GetSlot(ushort slotIndex)
        {
            var slot = new PlantingSlot()
            {
                SlotIndex = slotIndex,
            };
            if (Slots.TryGetValue(slot, out var mySlot))
            {
                return mySlot;
            }
            return null;
        }
    }
}