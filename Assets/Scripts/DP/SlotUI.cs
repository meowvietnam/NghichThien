using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class SlotUI : BaseBehaviour
    {
        [SerializeField] private DongPhuSeedUI seedUI;
        [SerializeField] private GameObject lockObject;
        [SerializeField] private Text textInfo;
        private bool isLocked;
        private int slotIndex;
        public void SetIsLocked(bool value)
        {
            isLocked = value;
            
        }
    }
}
