using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class SlotUI : BaseBehaviour
    {
        [SerializeField] private DongPhuSeedUI seedUI;
        [SerializeField] private GameObject lockObject;
        [SerializeField] private SlotInfo slotInfo;
        public PlantingSlot Slot { get; private set; }
        public PlantSeed Seed { get; private set; }

        public void Init(PlantingSlot slot, PlantSeed seed)
        {
            Slot = slot;
            SetLocked(slot.IsLocked);
            Slot.OnLockedChanged += OnLockChanged;
            Slot.OnHarvest += OnHarvest;
            Slot.OnPlant += OnPlant;
            SetSeed(seed);       
        }

        private void OnPlant(PlantSeed seed)
        {
            SetSeed(seed);
        }

        private void OnLockChanged(bool isLocked)
        {
            SetLocked(isLocked);
        }

        public void SetSeed(PlantSeed seed)
        {
            Seed = seed;
            slotInfo.SetSeed(seed);
            if (seed != null)
            {
                seedUI.Init(seed.HerbId, 1);
                seedUI.gameObject.SetActive(true);
            }
            UpdateSlotText();
        }

        private void UpdateSlotText()
        {
            var isLocked = Slot.IsLocked;
            if (isLocked)
            {
                var levelUnlocked = DongPhuPanel.Instance.GetLevelConfig(Slot.UnlockLevel);
                var text =
                    $"Mở khoá <color=#{ColorUtility.ToHtmlStringRGB(levelUnlocked.Color)}>{levelUnlocked.Name}</color>)";
                slotInfo.SetInfoText(text);
            }
            else
            {
                if (Seed == null)
                {
                    slotInfo.SetInfoText(Slot.GetName());
                }
            }
        }
        public void SetLocked(bool value)
        {
            lockObject.SetActive(value);
            if (value)
            {
                seedUI.gameObject.SetActive(false);
            }
            UpdateSlotText();
        }

        private void OnHarvest(PlantSeed plantSeed)
        {
            SetSeed(null);
        }
    }
}