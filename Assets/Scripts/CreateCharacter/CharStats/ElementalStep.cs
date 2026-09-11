using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter.CharStats
{
    public class ElementalStep : Step
    {
        [SerializeField] private ElementalUI elementalUIPrefab;
        [SerializeField] private Text selectNumText;
        [SerializeField] private Transform content;
        [SerializeField] private float[] unselectPercent;
        private List<ElementalUI> elementalUIs = new List<ElementalUI>();
        private List<ElementalUI> selecteds = new List<ElementalUI>();
        private SelectStats selectStats;
        private ElementalStepData elementalStepData;

        public void Init(SelectStats selectStats, ElementalStepData elementalStepData)
        {
            this.selectStats = selectStats;
            this.elementalStepData = elementalStepData;
            var elementals = elementalStepData.Elementals;
            foreach (var elemental in elementals)
            {
                var elementalUI = Instantiate(elementalUIPrefab, content);
                elementalUIs.Add(elementalUI);
                elementalUI.Init(elemental, OnSelect, OnUnselect);
            }

            CalculatePercent();
            foreach (var elementalUI in elementalUIs)
            {
                selectStats.Apply(elementalUI.StatModifier);
            }
        }

        private void SelectNumElemental(int num)
        {
            var currentSelectNum = selecteds.Count;
            while (currentSelectNum > num)
            {
                var elementalUI = selecteds[0];
                elementalUI.ToggleWithoutNotify(false);
                OnUnselect(elementalUI);
                currentSelectNum--;
            }

            if (currentSelectNum < num)
            {
                var n = num - currentSelectNum;
                var available = elementalUIs
                    .Where(x => !selecteds.Contains(x))
                    .OrderBy(_ => Random.value)
                    .Take(n)
                    .ToList();
                foreach (var elementalUI in available)
                {
                    elementalUI.ToggleWithoutNotify(true);
                    selecteds.Add(elementalUI);
                    selectStats.Select(elementalStepData.Id, elementalStepData.GroupId, elementalUI.Elemental.Id);
                }
            }

            CalculatePercent();
            selectStats.ReUpdate();
        }

        private void OnSelect(ElementalUI elementalUI)
        {
            selecteds.Add(elementalUI);
            CalculatePercent();
            selectStats.ReUpdate();
            selectStats.Select(elementalStepData.Id, elementalStepData.GroupId, elementalUI.Elemental.Id);
        }

        private void CalculatePercent()
        {
            var unselecteds = elementalUIs
                .Where(x => !selecteds.Contains(x))
                .ToList();
            var unselectedCount = unselecteds.Count;
            var unselectedPercent = unselectedCount > 0 ? unselectPercent[unselectedCount - 1] : 0;
            foreach (var elementalUI in unselecteds)
            {
                elementalUI.SetPercent(unselectedPercent);
            }

            var selectedPercent = (100 - unselectedPercent * unselectedCount) / selecteds.Count;
            foreach (var elementalUI in selecteds)
            {
                elementalUI.SetPercent(selectedPercent);
            }
        }

        private void OnUnselect(ElementalUI elementalUI)
        {
            selecteds.Remove(elementalUI);
            CalculatePercent();
            selectStats.ReUpdate();
            selectStats.UnSelect(elementalStepData.Id, elementalStepData.GroupId, elementalUI.Elemental.Id);
        }

        public override void OnEnter()
        {
            selectStats.SetTitle(elementalStepData.Title);
            selectStats.SetDescription(elementalStepData.Description);
            var linhCan = selectStats.CharacterStats.LinhCan;
            var totalSelect = (int)linhCan + 1;
            selectNumText.text = $"Lựa chọn {totalSelect} linh căn";
            SelectNumElemental(totalSelect);
            SetActive(true);
        }

        public override void OnExit()
        {
            SetActive(false);
        }

        public override bool Validate()
        {
            var linhCan = selectStats.CharacterStats.LinhCan;
            var require = (int)linhCan + 1;
            if (selecteds.Count != require)
            {
                NotifyGroup.Instance.Show($"Hãy chọn {require} linh căn");
                return false;
            }

            return true;
        }
    }
}