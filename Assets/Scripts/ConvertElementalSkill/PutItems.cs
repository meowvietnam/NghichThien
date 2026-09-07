using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class PutItems : BaseBehaviour
    {
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
            UpdateRate();
        }

        private void UpdateRate()
        {
            ConvertElementalSkillPanel.Instance.Page.RateManager.RefreshRateText();
        }

        public void OnDeselect(Slot slot)
        {
            UpdateButton();
            UpdateRate();
        }

        private void UpdateButton()
        {
            var active = slots.All(x => x.SkillData != null);
            convertButton.interactable = active;
        }

        private void OnConvert()
        {
            ConvertElementalSkillPanel.Instance.Convert();
        }

        private void OnDestroy()
        {
            convertButton.onClick.RemoveAllListeners();
        }

        public Slot[] GetAvailableSlots()
        {
            return slots.Where(x => x.SkillData != null).ToArray();
        }

        public Slot[] GetSlots()
        {
            return slots;
        }
    }
}