using System;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class TypeSkillToggle : MonoBehaviour
    {
        [SerializeField] private GameObject active;
        [SerializeField] private GameObject inactive;
        [SerializeField] private Toggle toggle;
        [SerializeField] private EventRechargePanel panel;
        [SerializeField] private BaseSkill.ETypeSkill type;


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
                panel.OnSelected(type);
            }
        }
    }
}