using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LuyenDan
{
    public class SelectNum : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Slider slider;
        [SerializeField] private Button reduceButton;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button confirm;
        [SerializeField] private Button back;
        [SerializeField] private UnityEvent confirmAction;
        public event Action<int> ValueChanged;
        private void Start()
        {
            slider.onValueChanged.AddListener(OnValueChanged);
            reduceButton.onClick.AddListener(Reduce);
            increaseButton.onClick.AddListener(Increase);
            confirm.onClick.AddListener(Confirm);
            back.onClick.AddListener(Back);
        }

        private void Reduce()
        {
            var value = slider.value;
            value--;
            slider.value = Math.Clamp(value, slider.minValue, slider.maxValue);
        }

        private void Increase()
        {
            var value = slider.value;
            value++;
            slider.value = Math.Clamp(value, slider.minValue, slider.maxValue);
        }

        private void Back()
        {
            SetActive(false);
        }

        private void Confirm()
        {
            SetActive(false);
            confirmAction?.Invoke();
        }
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
            if (!active)
            {
                ValueChanged = null;
            }
        }
        private void OnValueChanged(float value)
        {
            ValueChanged?.Invoke((int)value);
        }
        public void SetText(string text)
        {
            this.text.text = text;
        }

        public void SetMin(int min)
        {
            slider.minValue = min;
        }
  
        public void SetMax(int max)
        {
            slider.maxValue = max;
        }
        public int GetValue()
        {
            return (int)slider.value;
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveAllListeners();
            back.onClick.RemoveAllListeners();
            confirm.onClick.RemoveAllListeners();
            increaseButton.onClick.RemoveAllListeners();
            reduceButton.onClick.RemoveAllListeners();
            confirmAction = null;
            ValueChanged = null;
        }
        public void SetValue(int value)
        {
            slider.value = value;
        }
    }
}