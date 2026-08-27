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
        public PlantSeed GetPlantSeed(ushort slotIndex)
        {
            var seed = new PlantSeed()
            {
                SlotIndex = slotIndex
            };
            if(Seeds.TryGetValue(seed, out var mySeed))
            {
                return mySeed;
            }
            return null;
        }
    }
}