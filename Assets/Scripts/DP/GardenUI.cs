using UnityEngine;

namespace DP
{
    public class GardenUI : BaseBehaviour
    {
        [SerializeField] private SlotUI slotUIPrefab;
        [SerializeField] private RectTransform container;
        public void Init(PlantGarden garden)
        {
            var slots = garden.Slots;
        }
    }
}