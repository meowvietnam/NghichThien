using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter.CharStats
{
    public class StatOptionUI : BaseBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Text nameText;
        [SerializeField] private Text costText;
        [SerializeField] private Button infoButton;
        public StatOption StatOption { get; private set; }
        private event Action<StatOptionUI> onSelect;
        private event Action<StatOptionUI> onUnselect;
        [SerializeField] private Transform tooltipTransform;
        public void Init(StatOption statOption, ToggleGroup toggleGroup, Action<StatOptionUI> onSelect, Action<StatOptionUI> onUnselect)
        {
            toggle.group = toggleGroup;
            StatOption = statOption;
            nameText.text = statOption.Name;
            costText.text = statOption.Modifiers.Sum(m => m.Cost).ToString();
            this.onSelect = onSelect;
            this.onUnselect = onUnselect;
            toggle.onValueChanged.AddListener(OnValueChanged);
            infoButton.onClick.AddListener(ShowInfo);
        }
        private void ShowInfo()
        {
            var tooltip = Tooltip.Instance;
            tooltip.SetPosition(tooltipTransform.position);
            tooltip.DoShow();
            tooltip.SetText(StatOption.Effective, StatOption.Description);
        }

        public void OnValueChanged(bool isOn)
        {
            if (isOn)
            {
                onSelect(this);
            }
            else
            {
                onUnselect(this);
            }
        }
        public void SetSelectWithoutNotify(bool isSelect)
        {
            toggle.SetIsOnWithoutNotify(isSelect);
        }
        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
            infoButton.onClick.RemoveListener(ShowInfo);
            onSelect = null;
            onUnselect = null;
        }
    }
}
