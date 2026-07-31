using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class OpenLoginPanelButton : BaseBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private LoginPanel loginPanel;
        private void Start()
        {
            SetEventButton(button, Open);
        }

        private void Open()
        {
            loginPanel.Show();
        }
    }
}
