using System;
using System.Threading.Tasks;
using Network.Callback;
using Network.Extensions;
using Network.Models;
using UnityEngine;
using NativeTcp = System.Net.Sockets.TcpClient;

namespace Network
{
    public class UnauthenticatedClient : ClientConnection
    {
        private enum MessageState
        {
            ConnectState,
            CheckVersionState
        }
        private class CheckVersionState : IMessageState<MessageTag, MessageState>
        {
            private UnauthenticatedClient client;
            public CheckVersionState(UnauthenticatedClient client)
            {
                this.client = client;
            }
            public void OnEnter()
            {
                client.TcpClient.Send(MessageTag.CheckVersion,NetworkManager.Instance.Version);
            }

            public void Handle(Message<MessageTag> message)
            {
                var tag = message.Tag;
                var reader = message.Reader;
                if (tag == MessageTag.CheckVersionResult)
                {
                    var success = reader.Read<bool>();
                    client.callback.OnCheckVersionResult(success);
                }
            }
            public void OnLeave()
            {
            }
            public MessageState State => MessageState.CheckVersionState;
        }

        private class ConnectState: IMessageState<MessageTag, MessageState>
        {
            private UnauthenticatedClient client;
            public ConnectState(UnauthenticatedClient client)
            {
                this.client = client;
            }
            public MessageState State => MessageState.ConnectState;

            public void OnEnter()
            {
                if (!client.isReconnect)
                {
                    client.TcpClient.Send(MessageTag.Connect);
                }
            }
            public void Handle(Message<MessageTag> message)
            {
                var tag = message.Tag;
                var reader = message.Reader;
                if (tag == MessageTag.ConnectResult)
                {
                    var result = reader.Read<ConnectResult>();
                    client.callback.OnConnectResult(result);
                    if (result.Success)
                    {
                        client.ChangeState(new CheckVersionState(client));
                        AdsService.Instance.Init();
                    }
                }
                else if (tag == MessageTag.ReconnectResult)
                {
                    
                }
            }
            public void OnLeave()
            {
            }
        }
        private MessageStateHandler<MessageTag, MessageState> module;
        private IUnauthenticatedClientCallback callback;
        private bool isReconnect;
        public UnauthenticatedClient(NativeTcp client, IUnauthenticatedClientCallback callback, bool isReconnect) : base(client)
        {
            this.isReconnect = isReconnect;
            this.callback = callback;
            module = new MessageStateHandler<MessageTag, MessageState>();
            module.ChangeState(new ConnectState(this));
            AddModule(module);
        }
        private void ChangeState(IMessageState<MessageTag, MessageState> state)
        {
            module.ChangeState(state);
        }
        protected override void OnDisconnected(IProtocol<MessageTag> protocol)
        {
            callback.OnDisconnect();            
        }
    }
}