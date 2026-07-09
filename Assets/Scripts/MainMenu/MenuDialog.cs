using Base;
using Start;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MenuDialog : BaseBehaviour
    {
        [SerializeField] private Text accountAlias;
        [SerializeField] private Button playButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button deleteAccountButton;
        [SerializeField] private AuthService authService;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderSound;
        [SerializeField] private Button btnAccount;
        [SerializeField] private Popup popAccount;
        [SerializeField] private PopupConfirmPassword popupConfirmPassword;
        private void Start()
        {
            SetEventButton(btnAccount, () =>
            {
                popAccount.Show();
            });

            SetEventButton(playButton, OnPlay);
            SetEventButton(logoutButton, OnLogout);
            SetEventButton(deleteAccountButton, OnDeleteAccount);
            SoundManager.Instance.SetSliderAudio(sliderMusic, true);
            SoundManager.Instance.SetSliderAudio(sliderSound, false);
            sliderMusic.value = SoundManager.Instance.volumeMusic;
            sliderSound.value = SoundManager.Instance.volumeSound;
        }
        private void OnDeleteAccount()
        {
            var password = Utils.GenerateRandomNumber(4);
            popupConfirmPassword.SetPassword(password);
            popupConfirmPassword.ShowPopConfirmAction($"Xác nhận xóa tài khoản?\nNhập mã {password} để tiếp tục.", () =>
            {
                authService.DeleteAccount();
            });
        }

        private void OnLogout()
        {
            authService.SignOut();
        }

        private void OnPlay()
        {
            authService.Play();
        }
        public void SetAccountAlias(string alias)
        {
            this.accountAlias.text = alias;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (!active)
            {
                popAccount.Hide();
            }
        }
    }
}
