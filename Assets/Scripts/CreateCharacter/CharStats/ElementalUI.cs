using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter.CharStats
{
    public class ElementalUI : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text percentText;
        [SerializeField] private Toggle toggle;
        public Elemental Elemental { get; private set; }
        private event Action<ElementalUI> onSelect;
        private event Action<ElementalUI> onUnselect;
        public StatModifier StatModifier { get; private set; }

        public void Init(Elemental elemental, Action<ElementalUI> onSelect, Action<ElementalUI> onUnselect)
        {
            StatModifier = new StatModifier(elemental.StatType, Action.Set, 0, 0);
            this.Elemental = elemental;
            var targetGraphic = (Image)toggle.targetGraphic;
            targetGraphic.sprite = elemental.Icon;
            var graphic = (Image)toggle.graphic;
            graphic.sprite = elemental.SelectSprite;
            nameText.text = elemental.Name;
            SetPercent(0);
            this.onSelect = onSelect;
            this.onUnselect = onUnselect;
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        public void SetPercent(float percent)
        {
            StatModifier.Value = percent;
            percentText.text = percent.ToString("F0") + "%";
        }

        private void OnValueChanged(bool isOn)
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

        public void ToggleWithoutNotify(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }

        private void OnDestroy()
        {
            onSelect = null;
            onUnselect = null;
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }
    }
}