using Base;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class SelectGender : MonoBehaviour
    {
        [SerializeField] private Gender gender;
        [SerializeField] private Toggle toggle;

        public Gender Gender => gender;

        public void Init()
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        public void ToggleOn()
        {
            toggle.isOn = true;
        }

        private void OnToggleChanged(bool isOn)
        {
            if (isOn)
            {
                SetPlayerAvatar.Instance.SetGender(gender);
            }
        }
    }
}