using System;
using CreateCharacter;
using DG.Tweening;
using Network.Models;
using Start;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class RegisterPanel : BaseBehaviour
    {
        [SerializeField] private InputField usernameInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private InputField confirmPasswordInputField;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button cancelRegisterButton;
        [SerializeField] private Toggle showPasswordToggle;
        [SerializeField] private Toggle showConfirmPasswordToggle;
        [SerializeField] private Transform panel;
        [SerializeField] private GameObject selfObject;
        [SerializeField] private AuthService authService;
        private IRegisterContext context;
        private Tween tween;

        private void Start()
        {
            SetEventButton(registerButton, Register);
            SetEventButton(cancelRegisterButton, Cancel);
            if (showPasswordToggle != null)
            {
                showPasswordToggle.onValueChanged.AddListener((value) =>
                {
                    TogglePassword(passwordInputField, value);
                });
            }
            if (showConfirmPasswordToggle != null)
            {
                showConfirmPasswordToggle.onValueChanged.AddListener((value) =>
                {
                    TogglePassword(confirmPasswordInputField, value);
                });
            }
        }

        private void TogglePassword(InputField inputField, bool value)
        {
            inputField.contentType = value ? InputField.ContentType.Standard : InputField.ContentType.Password;
            inputField.ForceLabelUpdate();
        }
        private async void Register()
        {
            var username = usernameInputField.text.Trim();
            var password = passwordInputField.text;
            var confirmPassword = confirmPasswordInputField.text;
            var notifyGroup = NotifyGroup.Instance;

            if (!TryValidateRegister(username, password, confirmPassword, out var message))
            {
                notifyGroup.Show(message);
                return;
            }

            if (context == null)
            {
                notifyGroup.Show("Không tìm thấy luồng đăng ký");
                return;
            }

            registerButton.interactable = false;
            try
            {
                var model = new RegisterModel()
                {
                    Username = username,
                    Password = password,
                };
                var result = await context.Register(model);
                if (result == null)
                {
                    notifyGroup.Show("Đăng ký thất bại");
                    return;
                }

                notifyGroup.Show(result.Message);
                if (result.Success)
                {
                    authService.RegisterSuccess(model);
                    Hide();
                }
                authService.OnRegisterResult(result);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                notifyGroup.Show($"Đăng ký thất bại: {ex.Message}");
            }
            finally
            {
                registerButton.interactable = true;
            }
        }

        private static bool TryValidateRegister(string username, string password, string confirmPassword, out string message)
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
            if (string.IsNullOrEmpty(confirmPassword))
            {
                message = "Vui lòng nhập lại mật khẩu";
                return false;
            }
            if (password != confirmPassword)
            {
                message = "Mật khẩu xác nhận không khớp";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private void Cancel()
        {
            Hide();
        }

        public void Show(IRegisterContext context, float duration = 0.25f)
        {
            this.context = context;
            var defaultModel = context.GetDefaultRegisterModel();
            if (defaultModel != null)
            {
                usernameInputField.text = defaultModel.Username;
                passwordInputField.text = defaultModel.Password;
                confirmPasswordInputField.text = string.Empty;
            }
            tween?.Kill();
            selfObject.SetActive(true);
            tween = panel.DOScale(Vector3.one, duration);
        }

        public void Hide(float duration = 0.25f)
        {
            tween?.Kill();
            tween = panel.DOScale(Vector3.zero, duration).OnComplete(() => selfObject.SetActive(false));
        }
    }
}
