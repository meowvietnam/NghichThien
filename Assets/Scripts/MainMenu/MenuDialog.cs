using Network;
using Network.Extensions;
using Start;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MenuDialog : BaseBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Button playButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private AuthService authService;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderSound;
        [SerializeField] private Button btnAccount;
        [SerializeField] private Popup popAccount;

        private void Start()
        {
            SetEventButton(btnAccount, () =>
            {
                popAccount.Show();
            });

            SetEventButton(playButton, OnPlay);
            SetEventButton(logoutButton, OnLogout);
            SoundManager.Instance.SetSliderAudio(sliderMusic, true);
            SoundManager.Instance.SetSliderAudio(sliderSound, false);
            sliderMusic.value = SoundManager.Instance.volumeMusic;
            sliderSound.value = SoundManager.Instance.volumeSound;
        }
        private void OnLogout()
        {
            authService.SignOut();
            SceneManager.LoadScene("MainMenu");
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
