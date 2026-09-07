using System;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class ToggleSkinCategory : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Text label;
        [SerializeField] private GameObject thisObject;
        public string Id { get; private set; }
        private event Action<ToggleSkinCategory> onToggleOn;

        public void Init(SkinCategory skinCategory, ToggleGroup toggleGroup, Action<ToggleSkinCategory> onToggleOn)
        {
            Id = skinCategory.Id;
            label.text = skinCategory.Name;
            toggle.group = toggleGroup;
            this.onToggleOn = onToggleOn;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                onToggleOn?.Invoke(this);
            }
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }

        public void ToggleOn()
        {
            toggle.isOn = true;
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            onToggleOn = null;
        }
    }
}