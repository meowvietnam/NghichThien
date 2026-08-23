using System;
using UnityEngine;
using UnityEngine.UI;
namespace ConvertElementalSkill
{
    public class TypeSkillToggle : MonoBehaviour
    {
        [field: SerializeField] public BaseSkill.ETypeSkill Type { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [SerializeField] private Toggle toggle;
        [SerializeField] private GameObject active;
        [SerializeField] private GameObject inactive;
        public (BaseSkill.ETypeSkill, GameObject, GameObject ) Get()
        {
            return (Type, active, inactive);
        }
        private void Start()
        {
            toggle.onValueChanged.AddListener(OnToggle);
            OnToggle(toggle.isOn);
        }
        private void OnToggle(bool isOn)
        {
            active.SetActive(isOn);
            inactive.SetActive(!isOn);
            if (isOn)
            {
                ConvertElementalSkillPanel.Instance.OnSelect(this);
            }
        }
        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggle);
        }
    }
}