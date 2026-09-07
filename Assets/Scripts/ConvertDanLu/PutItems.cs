using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Manages the 4 input slots and the Convert button.
    /// The button activates only when all 4 slots are filled with đan lô of the same Tier.
    /// Mirrors ConvertElementalSkill.PutItems.
    /// </summary>
    public class PutItems : BaseBehaviour
    {
        public const int RequiredCount = 4;

        [SerializeField] private Button convertButton;
        [SerializeField] private Slot[] slots;

        private void Start()
        {
            SetEventButton(convertButton, OnConvert);
        }

        public void Init()
        {
            foreach (var slot in slots)
            {
                slot.ClearNew();
            }

            UpdateButton();
        }

        public void Clear()
        {
            foreach (var slot in slots)
            {
                slot.ClearNew();
            }

            UpdateButton();
        }

        public void OnSelect(Slot slot)
        {
            UpdateButton();
            UpdateInfo();
        }

        public void OnDeselect(Slot slot)
        {
            UpdateButton();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            ConvertDanLuPanel.Instance.Page.InfoManager.Refresh();
        }

        private void UpdateButton()
        {
            // All 4 slots must be filled; mixed tiers are allowed (result determined by ratio)
            bool allFilled = slots.Count(x => x.DanData != null) == RequiredCount;
            convertButton.interactable = allFilled;
        }

        private void OnConvert()
        {
            ConvertDanLuPanel.Instance.Convert();
        }

        private void OnDestroy()
        {
            convertButton.onClick.RemoveAllListeners();
        }

        public Slot[] GetFilledSlots()
        {
            return slots.Where(x => x.DanData != null).ToArray();
        }

        public Slot[] GetSlots()
        {
            return slots;
        }
    }
}
