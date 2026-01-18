using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter.CharStats
{
    public class StatOptionGroupUI : MonoBehaviour
    {
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private StatOptionUI statOptionUIPrefab;
        private List<StatOptionUI> statOptionUIs = new List<StatOptionUI>();
        private List<StatOptionGroupUI> exclusives;
        public int Id { get; private set; }
        private event Action<StatOptionGroupUI, StatOptionUI> onSelect;
        private event  Action<StatOptionGroupUI, StatOptionUI> onUnselect;
        private StatOptionUI currentSelect;
        public void Init(Transform content,StatOptionGroup statOptionGroup, List<StatOptionGroupUI> exclusives, Action<StatOptionGroupUI, StatOptionUI> onSelect, Action<StatOptionGroupUI, StatOptionUI> onUnselect)
        {
            Id = statOptionGroup.Id;
            this.exclusives = exclusives;
            var statOptions = statOptionGroup.StatOptions;
            this.onSelect = onSelect;
            this.onUnselect = onUnselect;
            foreach (var statOption in statOptions)
            {
                var statOptionUI = Instantiate(statOptionUIPrefab, content);
                statOptionUI.Init(statOption, toggleGroup, OnSelect, OnUnselect);
                statOptionUIs.Add(statOptionUI);
            }
        }
        private void OnSelect(StatOptionUI statOptionUI)
        {
            currentSelect = statOptionUI;
            foreach (var statOptionGroupUI in exclusives)
            {
                statOptionGroupUI.DisableGroup();
            }
            onSelect(this, statOptionUI);
        }
        private void OnUnselect(StatOptionUI statOptionUI)
        {
            currentSelect = null;
            onUnselect(this, statOptionUI);
        }
        private void DisableGroup()
        {
            if (currentSelect != null)
            {
                onUnselect(this, currentSelect);
            }
            currentSelect = null;
            foreach (var statOptionUI in statOptionUIs)
            {
                statOptionUI.SetSelectWithoutNotify(false);
            }
        }

        private void OnDestroy()
        {
            onSelect = null;
            onUnselect = null;
        }
        public void SetId(int id)
        {
            Id = id;
        }
    }
}