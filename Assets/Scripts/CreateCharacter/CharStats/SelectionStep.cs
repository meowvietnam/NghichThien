using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter.CharStats
{
    public class SelectionStep : Step
    {
        [SerializeField] private Transform content;
        [SerializeField] private Transform groupContainer;
        [SerializeField] private StatOptionGroupUI statOptionGroupUIPrefab;
        private List<StatOptionGroupUI> statOptionGroupUIs = new List<StatOptionGroupUI>();
        private SelectStats selectStats;
        private SelectionStepData selectionStepData;
        private List<StatOptionUI> selecteds = new List<StatOptionUI>();
        public void Init(SelectStats selectStats, SelectionStepData data)
        {
            this.selectStats = selectStats;
            this.selectionStepData = data;
            var groups = data.Groups;
            foreach (var group in groups)
            {
                var groupUI = Instantiate(statOptionGroupUIPrefab, groupContainer);
                groupUI.SetId(group.Id);
                statOptionGroupUIs.Add(groupUI);
            }
            for (int i = 0; i < statOptionGroupUIs.Count; i++)
            {
                var groupData = groups[i];
                var statOptionGroupUI = statOptionGroupUIs[i];
                var exclusives = statOptionGroupUIs
                    .Where(s => groupData.Exclusives
                        .Contains(s.Id))
                    .ToList();
                statOptionGroupUI.Init(content, groupData, exclusives, OnSelect, OnUnselect);
            }
        }
        private void OnSelect(StatOptionGroupUI groupUI,StatOptionUI optionUI)
        {
            selecteds.Add(optionUI);
            var statOption = optionUI.StatOption;
            var modifiers = statOption.Modifiers;
            foreach (var modifier in modifiers)
            {
                selectStats.Apply(modifier);
            }
            selectStats.Select(selectionStepData.Id,groupUI.Id,statOption.Id);
        }
        private void OnUnselect(StatOptionGroupUI groupUI,StatOptionUI optionUI)
        {
            selecteds.Remove(optionUI);
            var statOption = optionUI.StatOption;
            var modifiers = statOption.Modifiers;
            foreach (var modifier in modifiers)
            {
                selectStats.Remove(modifier);
            }
            selectStats.UnSelect(selectionStepData.Id,groupUI.Id,statOption.Id);
        }
        public override void OnEnter()
        {
            selectStats.SetTitle(selectionStepData.Title);
            selectStats.SetDescription(selectionStepData.Description);
            SetActive(true);
        }
        public override void OnExit()
        {
            SetActive(false);
        }

        public override bool Validate()
        {
            if (selecteds.Count == 0)
            {
                NotifyGroup.Instance.Show("Hãy chọn 1 thiên phú");
                return false;
            }
            return true;
        }
    }
}