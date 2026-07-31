using System;
using System.Threading.Tasks;
using LongDH.LongDHModules;
using Network;
using Network.ClientState;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SignIn
{
    public abstract class SignInButton : BaseBehaviour
    {
        [SerializeField] private Button button;

        protected virtual void Start()
        {
            SetEventButton(button, SignIn);
        }

        public async void SignIn()
        {
            try
            {
                Observer.Notify(EventList.Network.BEGIN_SIGN_IN, this);
                var request = await CreateRequest();
                if (request != null)
                {
                    var state = Client.Instance.State as AuthState;
                    if (state != null)
                    {
                        var result = await state.Login(request);
                        OnLoginResult(result);
                    }
                }
            }
            catch (Exception e)
            {
                // ignored
            }
            finally
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Observer.Notify(EventList.Network.SIGN_IN_END, this);
                });
            }
        }
        protected abstract Task<ILogin> CreateRequest();
        public abstract ISignInService GetSignInService();

        protected virtual void OnLoginResult(LoginResult result)
        {
            
        }
        public async Task SignOut()
        {
            await GetSignInService().SignOutAsync();
        }
    }
}