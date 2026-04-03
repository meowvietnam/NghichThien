using System;
using CreateCharacter;
using Network.ClientState;
using UnityEngine;

namespace Network
{
    public class Client : ClientConnection, ISender<MessageTag>
    {
        public static Client Instance {get; private set;}
        private MessageStateHandler<MessageTag,MessageState> stateModule;
        public CoreMessageHandler<MessageTag> CoreModule { get; private set; }
        public UserData UserData { get; private set;}
        public CharacterData CharacterData { get; private set;}
        public Client(UnauthenticatedClient client)
        {
            var tcpClient = client.TcpClient;
            client.TransferProtocol(tcpClient,this);
            TcpClient = tcpClient;
            Instance = this;
            CoreModule = new CoreMessageHandler<MessageTag>();
            stateModule = new MessageStateHandler<MessageTag,MessageState>();
            AddModule(CoreModule);
            AddModule(stateModule);
        }

        public void SetUserData(UserData userData)
        {
            UserData = userData;
        }
        public void ChangeState(IClientState state)
        {
            stateModule.ChangeState(state);
        }

        protected override void OnError(IProtocol<MessageTag> protocol, Exception ex)
        {
            Debug.LogException(ex);
        }

        public void Send(Span<byte> data)
        {
            TcpClient.Send(data);
        }

        public void SetChar(CharacterData characterData)
        {
            CharacterData = characterData;
        }
    }
}