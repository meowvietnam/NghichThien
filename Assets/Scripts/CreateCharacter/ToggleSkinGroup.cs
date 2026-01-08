using System;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class ToggleSkinGroup : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Text label;
        [SerializeField] private Color normal;
        [SerializeField] private Color selected;
        public string Id { get; private set; }
        private event Action<ToggleSkinGroup> onSelected;
        public void Init(SkinGroup skinGroup,ToggleGroup toggleGroup, Action<ToggleSkinGroup> onSelected)
        {
            Id = skinGroup.Id;
            label.text = skinGroup.Name;
            toggle.group = toggleGroup;
            this.onSelected = onSelected;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            UpdateLabelColor(toggle.isOn);
        }

        private void UpdateLabelColor(bool isOn)
        {
            label.color = isOn ? selected : normal;
        }
        private void OnToggleValueChanged(bool isOn)
        {
            UpdateLabelColor(isOn);
            if(isOn)
            {
                onSelected?.Invoke(this);
            }
        }
        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            onSelected = null;
        }
    }
}
