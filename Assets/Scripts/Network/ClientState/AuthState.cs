using System.Threading.Tasks;
using Network.Callback;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using SignIn;
using Start;
using UnityEngine;

namespace Network.ClientState
{
    public class AuthState : IClientState
    {
        private IAuthCallback callback;
        public AuthState(IAuthCallback callback)
        {
            this.callback = callback;
        }

        public void OnEnter()
        {
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.AuthState;

        public async Task<LoginResult> Login(ILogin loginRequest)
        {
            var reader = await Client.Instance.MemRequestWait(MessageTag.LoginRequest, loginRequest);
            var result = reader.MemRead<LoginResult>();
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    AuthService.SetSignInProvider(result.Provider);
                }
                else
                {
                    Debug.Log(result.Message);
                    AuthService.DeleteSignInProvider();
                }
            });
            callback.OnLoginResult(result);
            return result;
        }
    }
}