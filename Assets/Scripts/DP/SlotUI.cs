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
        [SerializeField] private ParticleSystem effect;
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
        public bool HasSeed()
        {
            return Seed != null;
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
            panel.SetCurrentSelectedSlot(this);
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
                seedUI.Clicked += OnSeedClicked;
            }
            else
            {
                seedUI.gameObject.SetActive(false);
            }
            UpdateSlotText();
        }
        private void OnSeedClicked()
        {
            var dongPhu = DongPhuPanel.Instance;
            dongPhu.HideInventory();
            dongPhu.SetCurrentSelectedSlot(this);
        }
        private void UpdateSlotText()
        {
            var isLocked = Slot.IsLocked;
            if (isLocked)
            {
                var levelUnlocked = DongPhuPanel.Instance.GetLevelConfig(Slot.UnlockLevel);
                var text =
                    $"Mở khoá <color=#{ColorUtility.ToHtmlStringRGB(levelUnlocked.Color)}>{levelUnlocked.Name}</color>";
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
        public void PlayEffect()
        {
            effect.Play();
        }
        private void OnHarvest(PlantSeed plantSeed)
        {
            SetSeed(null);
        }
        public void OnClose()
        {
            DongPhuPanel.Instance.SetCurrentSelectedSlot(null);
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
            DongPhuPanel.Instance.HideInventory();
        }
    }
}