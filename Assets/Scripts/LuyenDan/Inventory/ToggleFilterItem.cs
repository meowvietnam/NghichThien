using System;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class ToggleFilterItem : FilterItem
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Graphic graphic;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color selectedColor;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
            if (!toggle.isOn)
            {
                graphic.color = normalColor;
            }
        }

        protected virtual void OnValueChanged(bool isOn)
        {
            var color = isOn ? selectedColor : normalColor;
            graphic.color = color;
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void Toggle(bool isOn)
        {
            toggle.isOn = isOn;
        }

        public override void ForceSelect()
        {
            Toggle(true);
            toggle.Select();
        }
    }
}