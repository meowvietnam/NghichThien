using CreateCharacter;
using Network.Callback;
using Network.Extensions;
using Network.Models;
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
            if (tag == MessageTag.LoginDeveloperResult)
            {
                var result = reader.Read<LoginResult>();
                callback.OnLoginResult(result);
            }
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.AuthState;
    }
}