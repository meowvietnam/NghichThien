using Network;
using Network.Extensions;
using Start;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MenuDialog : BaseBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Button playButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private AuthService authService;
        private void Start()
        {
            SetEventButton(playButton, OnPlay);
            SetEventButton(logoutButton, OnLogout);
        }
        private void OnLogout()
        {
            authService.SignOut();
        }

        private void OnPlay()
        {
            authService.Play();
        }
        public void SetTitle(string title)
        {
            this.title.text = title;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
