using System;
using Network.Models.Clans;
using UnityEngine;

namespace Clan
{
    public class ClanInheritanceSlotElement : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button selectButton;
        [SerializeField] private UnityEngine.UI.Text slotText;
        [SerializeField] private UnityEngine.UI.Text effectText;
        [SerializeField] private UnityEngine.UI.Text valueText;
        [SerializeField] private GameObject lockedRoot;
        [SerializeField] private UnityEngine.UI.Text lockedText;

        public ClanInheritanceTrigger Trigger { get; private set; }
        public int SlotIndex { get; private set; }
        public ClanInheritanceEffect Effect { get; private set; }
        public bool IsUnlocked { get; private set; }

        public event Action<ClanInheritanceSlotElement> Clicked;

        private void Awake()
        {
            if (selectButton == null) selectButton = GetComponent<UnityEngine.UI.Button>();
            selectButton?.onClick.AddListener(OnClicked);
        }

        public void Init(
            ClanInheritanceTrigger trigger,
            int slotIndex,
            ClanInheritanceEffect effect,
            bool unlocked,
            int unlockLevel)
        {
            Trigger = trigger;
            SlotIndex = slotIndex;
            Effect = effect;
            IsUnlocked = unlocked;

            if (slotText != null) slotText.text = $"Ô {slotIndex + 1}";
            if (effectText != null)
                effectText.text = effect == null ? "Chưa chọn truyền thừa" : ClanInheritancePanel.GetEffectName(effect.BuffType);
            if (valueText != null)
            {
                valueText.text = effect == null ? "+—" : $"+{effect.Value:n0}";
                valueText.gameObject.SetActive(unlocked);
            }
            if (lockedRoot != null) lockedRoot.SetActive(!unlocked);
            if (lockedText != null) lockedText.text = $"Mở ở cấp {unlockLevel}";
            if (selectButton != null) selectButton.interactable = unlocked;
        }

        private void OnClicked()
        {
            if (IsUnlocked) Clicked?.Invoke(this);
        }

        private void OnDestroy()
        {
            selectButton?.onClick.RemoveListener(OnClicked);
            Clicked = null;
        }
    }
}
