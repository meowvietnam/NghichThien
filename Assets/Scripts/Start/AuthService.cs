using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using MainMenu;
using Network;
using Network.Callback;
using Network.ClientState;
using UnityEngine;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using SignIn;
using SignIn.Google;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Start
{
    public class AuthService : BaseBehaviour, IAuthCallback
    {
        [SerializeField] private MenuDialog menu;
        [SerializeField] private GameObject buttonGroups;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private SignInButton[] signInButtons;
        [SerializeField] private string signInProgressText = "Đang đăng nhập...";
        [SerializeField] private string signOutProgressText = "Đang đăng xuất...";
        [SerializeField] private string playProgressText = "Đang vào game...";

        private void Start()
        {
            var state = new AuthState(this);
            Client.Instance.ChangeState(state);
            Observer.AddObserver(EventList.Network.BEGIN_SIGN_IN, OnBeginSignIn);
            Observer.AddObserver(EventList.Network.SIGN_IN_END, OnSignInEnd);
        }

        private void OnSignInEnd(object[] data)
        {
            progressBar.DoKill(() => {});
        }

        private void OnBeginSignIn(object[] data)
        {
            progressBar.SetText(signInProgressText);
            progressBar.DoShow(0.5f,0,null);
        }

        public void OnLoginResult(LoginResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                NotifyGroup.Instance.Show(result.Message);
                if (result.Success)
                {
                    Client.Instance.SetUserData(result.UserData.User);
                    menu.SetTitle(result.WelcomeText);
                    menu.SetActive(true);
                    buttonGroups.SetActive(false);
                }
            });
        }

        private void OnDestroy()
        {
            Observer.RemoveObserver(EventList.Network.BEGIN_SIGN_IN, OnBeginSignIn);
            Observer.RemoveObserver(EventList.Network.SIGN_IN_END, OnSignInEnd);
        }

        public async void SignOut()
        {
            progressBar.SetText(signOutProgressText);
            progressBar.DoShow(0.25f,0,null);
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.LogoutRequest);
            var result = reader.MemRead<LogoutResult>();
            if (result.Success)
            {
                client.SetUserData(null);
            }

            NetworkManager.Instance.Invoke(async () =>
            {
                progressBar.DoKill(() => {});
                if (result.Success)
                {
                    foreach (var button in signInButtons)
                    {
                        await button.SignOut();
                    }

                    menu.SetActive(false);
                    buttonGroups.SetActive(true);
                }
                else
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
            });
        }

        public async void Play()
        {
            progressBar.SetText(playProgressText);
            progressBar.DoShow(0.25f,0,null);
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.PlayRequest);
            var result = reader.MemRead<PlayResult>();
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    progressBar.DoKill(() =>
                    {
                        SceneManager.LoadSceneAsync("CreateCharacter");
                    });
                }
                else
                {
                    progressBar.DoKill(() => {});
                    NotifyGroup.Instance.Show(result.Message);
                }
            });
        }
    }
}
