using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class SlotUI : BaseBehaviour, IOpenInventoryContext
    {
        [SerializeField] private DongPhuSeedUI seedUI;
        [SerializeField] private GameObject lockObject;
        [SerializeField] private SlotInfo slotInfo;
        [SerializeField] private Button button;
        public PlantingSlot Slot { get; private set; }
        public PlantSeed Seed { get; private set; }

        private void Start()
        {
            SetEventButton(button, OnClick);
            seedUI.Clicked += OnSeedClick;
        }

        private void OnSeedClick()
        {
        }

        private void OnClick()
        {
            var panel = DongPhuPanel.Instance;
            if (!panel.IsMe())
            {
                return;
            }
            if (Seed != null)
            {
                return;
            }
            var selected = panel.Selected;
            var selectedTf = selected.transform;
            selectedTf.SetParent(transform);
            selectedTf.localPosition = Vector3.zero;
            selected.SetActive(true);
            panel.OpenInventoryWithContext(this);
        }

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
        public void OnClose()
        {
            var panel = DongPhuPanel.Instance;
            var selected = panel.Selected;
            selected.SetActive(true);
        }
        public void OnSelect(DongPhuSeedUI seedUI)
        {
            var herbId = seedUI.HerbId;
            var slotIndex = Slot.SlotIndex;
            var request = new PlantRequest()
            {
                HerbId = herbId,
                SlotIndex = slotIndex,
            };
            Client.Instance.MemSend(MessageTag.Plant, request);
        }

    }
}