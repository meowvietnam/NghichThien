using System;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class FilterItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Graphic graphic;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color selectedColor;
        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
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
        public void Toggle(bool isOn)
        {
            toggle.isOn = isOn;
        }
        public void ForceSelect()
        {
            Toggle(true);
            toggle.Select();
        }
    }
}