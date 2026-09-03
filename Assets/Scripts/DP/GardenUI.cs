using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DP
{
    public class GardenUI : BaseBehaviour
    {
        [SerializeField] private SlotUI slotUIPrefab;
        [SerializeField] private RectTransform container;
        private List<SlotUI> slots = new List<SlotUI>();
        public void Init(PlantGarden garden)
        {
            var slots = garden.Slots.OrderBy(x => x.SlotIndex).ToList();
            var seeds = garden.Seeds;
            foreach (var slot in slots)
            {
                var seedRef = new PlantSeed()
                {
                    SlotIndex = slot.SlotIndex,
                };
                if (seeds.TryGetValue(seedRef, out var seed))
                {
                    
                }
                var slotUI = Instantiate(slotUIPrefab, container);
                slotUI.Init(slot, seed);
                this.slots.Add(slotUI);
            }
        }
        public SlotUI FindSlot(ushort slotIndex)
        {
            return slots.First(x => x.Slot.SlotIndex == slotIndex);
        }
    }
}