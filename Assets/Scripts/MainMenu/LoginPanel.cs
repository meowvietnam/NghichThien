using System;
using System.Threading.Tasks;
using CreateCharacter;
using DG.Tweening;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using SignIn;
using Start;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class LoginPanel : SignInButton, ISignInService, IRegisterContext
    {
        [SerializeField] private RegisterPanel registerPanel;
        [SerializeField] private Transform panel;
        [SerializeField] private GameObject selfObject;
        [SerializeField] private InputField usernameInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private Toggle showPasswordToggle;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button registerButton;
        private Tween tween;
        private bool isInitialized;

        protected override void Start()
        {
            base.Start();
            SetEventButton(registerButton, OpenRegisterPanel);
            SetEventButton(cancelButton, () => Hide());
            if (showPasswordToggle != null)
            {
                showPasswordToggle.onValueChanged.AddListener((value) =>
                {
                    TogglePassword(passwordInputField, value);
                });
            }
        }

        private void OpenRegisterPanel()
        {
            Hide();
            registerPanel.Show(this);
        }

        public void Hide(float duration = 0.25f)
        {
            tween?.Kill();
            tween = panel.DOScale(Vector3.zero, duration).OnComplete(() => selfObject.SetActive(false));
        }

        public void Show(float duration = 0.25f)
        {
            tween?.Kill();
            selfObject.SetActive(true);
            tween = panel.DOScale(Vector3.one, duration);
        }

        private void TogglePassword(InputField inputField, bool value)
        {
            inputField.contentType = value ? InputField.ContentType.Standard : InputField.ContentType.Password;
            inputField.ForceLabelUpdate();
        }

        private void Init()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            var localAccount = AuthService.GetLocalAccount();
            if (localAccount == null)
            {
                return;
            }

            usernameInputField.text = localAccount.Username;
            passwordInputField.text = localAccount.Password;
        }

        private static bool TryValidateLogin(string username, string password, out string message)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                message = "Vui lòng nhập tên đăng nhập";
                return false;
            }

            if (username.Length < 4)
            {
                message = "Tên đăng nhập phải có ít nhất 4 ký tự";
                return false;
            }

            if (username.Length > 32)
            {
                message = "Tên đăng nhập không được quá 32 ký tự";
                return false;
            }

            foreach (var character in username)
            {
                if (!char.IsWhiteSpace(character))
                {
                    continue;
                }

                message = "Tên đăng nhập không được chứa khoảng trắng";
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                message = "Vui lòng nhập mật khẩu";
                return false;
            }

            if (password.Length < 6)
            {
                message = "Mật khẩu phải có ít nhất 6 ký tự";
                return false;
            }

            if (password.Length > 64)
            {
                message = "Mật khẩu không được quá 64 ký tự";
                return false;
            }

            message = string.Empty;
            return true;
        }

        protected override async Task<ILogin> CreateRequest()
        {
            Init();
            var username = usernameInputField.text;
            var password = passwordInputField.text;
            var result = TryValidateLogin(username, password, out var message);
            if (!result)
            {
                NotifyGroup.Instance.Show(message);
                throw new Exception(message);
            }
            var credential = await SignInAsync() as LoginWithUsernamePassword;
            return credential;
        }

        protected override void OnLoginResult(LoginResult result)
        {
            if (result.Success)
            {
                var localAccount = new LocalAccount()
                {
                    Username = usernameInputField.text,
                    Password = passwordInputField.text,
                };
                AuthService.SetLocalAccount(localAccount);
                Hide();
            }
        }

        public override ISignInService GetSignInService()
        {
            return this;
        }

        public Task<ISignInCredential> SignInAsync()
        {
            return Task.FromResult<ISignInCredential>(new LoginWithUsernamePassword()
            {
                Username = usernameInputField.text,
                Password = passwordInputField.text,
            });
        }

        public Task SignOutAsync()
        {
            AuthService.DeleteLocalAccount();
            return Task.CompletedTask;
        }

        public RegisterModel GetDefaultRegisterModel()
        {
            return new RegisterModel()
            {
                Username = string.IsNullOrWhiteSpace(usernameInputField.text) ? string.Empty : usernameInputField.text,
                Password = string.IsNullOrWhiteSpace(passwordInputField.text) ? string.Empty : passwordInputField.text,
            };
        }

        public async Task<RegisterResult> Register(RegisterModel model)
        {
            var reader = await Client.Instance.MemRequestWait(MessageTag.RegisterRequest, model);
            var result = reader.MemRead<RegisterResult>();
            return result;
        }

        public SignInProvider Provider => SignInProvider.UsernamePassword;
    }
}