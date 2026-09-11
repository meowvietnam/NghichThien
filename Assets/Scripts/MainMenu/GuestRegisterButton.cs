using System.Threading.Tasks;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using SignIn;
using SignIn.Guest;
using Start;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class GuestRegisterButton : BaseBehaviour, IRegisterContext
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject selfObject;
        [SerializeField] private RegisterPanel registerPanel;

        public void TryValidate()
        {
            var provider = AuthService.GetSignInProvider();
            if (provider == null)
            {
                selfObject.SetActive(false);
                return;
            }

            var active = provider.Value == SignInProvider.Guest;
            selfObject.SetActive(active);
        }

        private void Start()
        {
            SetEventButton(button, OpenRegisterPanel);
        }

        private void OpenRegisterPanel()
        {
            registerPanel.Show(this);
        }

        public RegisterModel GetDefaultRegisterModel()
        {
            return null;
        }

        public async Task<RegisterResult> Register(RegisterModel model)
        {
            var guestUsername = PlayerPrefs.GetString(GuestSignIn.USERNAME_KEY);
            var request = new GuestRegister()
            {
                GuestUsername = guestUsername,
                Model = model
            };
            var reader = await Client.Instance.MemRequestWait(MessageTag.GuestRegisterRequest, request);
            var result = reader.MemRead<RegisterResult>();
            return result;
        }
    }
}