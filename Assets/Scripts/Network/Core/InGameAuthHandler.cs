using CreateCharacter;
using LongDH.LongDHModules;
using Network.ClientState;
using Network.Extensions.MemoryPack;
using Network.Models;
using Start;
using UnityEngine;

namespace Network.Core
{
    public class InGameAuthHandler : IClientHandler
    {
        public void OnEnter()
        {
        }


        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.InGameLogoutResult)
            {
                var result = reader.MemRead<LogoutResult>();
                var client = Client.Instance;
                if (result.Success)
                {
                    client.SetChar(null);
                    client.SetUserData(null);
                    client.SetLogout(true);
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        AuthService.DeleteSignInProvider();
                        Observer.Notify(EventList.Network.IN_GAME_LOGOUT);
                        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                    }
                    else
                    {
                        NotifyGroup.Instance.Show(result.Message);
                    }
                });
            }
        }

        public void OnLeave()
        {
        }

        private static MessageTag[] handlers = { MessageTag.InGameLogoutResult };
        public MessageTag[] HandleTag => handlers;

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }
    }
}