using System.Collections.Generic;
using System.IO;
using System.Linq;
using CreateCharacter.CharStats;
using CreateCharacter.States;
using DG.Tweening;
using Network;
using Network.Extensions;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class SelectStats : MonoBehaviour
    {
        [SerializeField] private GameObject thisObject;
        [SerializeField] private StepConfig stepConfig;
        [SerializeField] private Text title;
        [SerializeField] private Text description;
        [SerializeField] private Transform thisTransform;
        [SerializeField] private CharacterStats orginalStats;
        [SerializeField] private int originalMoney;

        public CharacterStats CharacterStats => CreateCharacterSystem.Instance.NewCharacter.Stats;
        private int money;
        public int Money => money;
        [SerializeField] private List<CurrentInfo> currentInfos;

        // private Dictionary<int, Dictionary<int, List<StatModifier>>> statModifiersDictionary = new();
        [SerializeField] private Text remainingText;
        private List<Step> steps = new ();
        private Step currentStep;
        private int currentStepIndex = -1;
        private CreateCharacterPanel createCharacterPanel;
        [SerializeField] private Image nextImage;
        [SerializeField] private Sprite nextSprite;
        [SerializeField] private Sprite finalSprite;
        private List<SelectionModel> selectionModels = new List<SelectionModel>();
        private List<StatModifier> statModifiers = new List<StatModifier>();

        public void Init(CreateCharacterPanel createCharacterPanel)
        {
            this.createCharacterPanel = createCharacterPanel;
            CharacterStats.CopyFrom(orginalStats);
            SetMoney(originalMoney);
            foreach (var stepData in stepConfig.StepDatas)
            {
                var step = stepData.CreateUI(this, thisTransform);
                step.SetActive(false);
                steps.Add(step);
            }

            foreach (var currentInfo in currentInfos)
            {
                currentInfo.Init(this);
            }
        }

        public void FirstStep()
        {
            currentStep?.SetActive(false);
            currentStepIndex = 0;
            currentStep = steps[currentStepIndex];
            currentStep.OnEnter();
        }
        public void NextStep()
        {
            if (currentStepIndex == steps.Count - 1)
            {
                if (money < 0)
                {
                    NotifyGroup.Instance.Show("Điểm thiên phú không thể là số âm");
                    return;
                }

                Final();
                return;
            }

            currentStep?.SetActive(false);
            currentStepIndex++;
            if (currentStepIndex == steps.Count - 1)
            {
                nextImage.sprite = finalSprite;
            }

            currentStep = steps[currentStepIndex];
            currentStep.OnEnter();
        }
        private void Final()
        {
            CreateCharacterSystem.Instance.Final(selectionModels);
        }

        public void PreviousStep()
        {
            if (currentStepIndex == 0)
            {
                currentStepIndex--;
                createCharacterPanel.SetState(new CreateSkin(createCharacterPanel));
                return;
            }

            currentStep?.SetActive(false);
            currentStepIndex--;
            if (currentStepIndex != steps.Count - 1)
            {
                nextImage.sprite = nextSprite;
            }

            currentStep = steps[currentStepIndex];
            currentStep.OnEnter();
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }

        public void SetDescription(string description)
        {
            this.description.text = description;
        }

        public void SetTitle(string title)
        {
            this.title.text = title;
        }

        private void SetMoney(int money)
        {
            this.money = money;
            remainingText.text = money.ToString();
        }

        public bool Apply(StatModifier statOptionModifier)
        {
            if (statModifiers.Contains(statOptionModifier))
            {
                ReUpdate();
                return false;
            }

            statModifiers.Add(statOptionModifier);
            CharacterStats.Apply(statOptionModifier);
            SetMoney(money - statOptionModifier.Cost);
            return true;
        }

        public void ReUpdate()
        {
            CharacterStats.CopyFrom(orginalStats);
            money = originalMoney;
            var statModifiers = this.statModifiers.OrderBy(s => s.Action);
            foreach (var statModifier in statModifiers)
            {
                money -= statModifier.Cost;
                CharacterStats.Apply(statModifier);
            }

            SetMoney(money);
        }

        public bool Remove(StatModifier statOptionModifier)
        {
            if (!statModifiers.Remove(statOptionModifier))
            {
                return false;
            }

            ReUpdate();
            return true;
        }

        public bool ValidateStep()
        {
            return currentStep.Validate();
        }

        public object GetOrginalValue(StatType statType)
        {
            return orginalStats.GetValue(statType);
        }

        public void Select(int stepId, int groupId, int selectionId)
        {
            var model = selectionModels.FirstOrDefault(x => x.Id == stepId);
            if (model == null)
            {
                model = new SelectionModel(stepId);
                selectionModels.Add(model);
            }

            var group = model.Groups.FirstOrDefault(x => x.Id == groupId);
            if (group == null)
            {
                group = new SelectionGroup(groupId);
                model.Groups.Add(group);
            }

            if (!group.Items.Contains(selectionId))
            {
                group.Items.Add(selectionId);
            }
        }

        public void UnSelect(int stepId, int groupId, int selectionId)
        {
            var model = selectionModels.FirstOrDefault(x => x.Id == stepId);
            if (model == null)
            {
                return;
            }
            var group = model.Groups.FirstOrDefault(x => x.Id == groupId);
            if (group == null)
            {
                return;
            }
            group.Items.Remove(selectionId);
            if (group.Items.Count == 0)
            {
                model.Groups.Remove(group);
            }
        }
    }
}