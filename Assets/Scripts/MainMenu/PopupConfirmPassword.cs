using System;
using CreateCharacter;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace MainMenu
{
    public class PopupConfirmPassword : PopupConfirmAction
    {
        [SerializeField] private InputField textField;
        private string password = string.Empty;
        
        public void SetPassword(string password)
        {
            this.password = password;
        }

        protected override void OnClickEvent(Action actionConfirm)
        {
            if (textField.text != password)
            {
                textField.text = string.Empty;
                NotifyGroup.Instance.Show("Mã xác nhận không đúng!");
                return;
            }
            textField.text = string.Empty;
            Hide();
            actionConfirm?.Invoke();
        }
    }
}
