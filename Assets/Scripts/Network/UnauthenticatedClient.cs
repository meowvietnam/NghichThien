using Network.Callback;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
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
                NetworkManager.Instance.Invoke(() =>
                {
                    client.TcpClient.Send(MessageTag.CheckVersion, NetworkManager.Instance.Version);
                });
            }

            public void Handle(Message<MessageTag> message)
            {
                var tag = message.Tag;
                var reader = message.Reader;
                if (tag == MessageTag.CheckVersionResult)
                {
                    var result = reader.MemRead<CheckVersionResult>();
                    var net = NetworkManager.Instance;
                    net.Invoke(() =>
                    {
                        client.callback.OnCheckVersionResult(result);
                    });
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
                    
                    if (result.Success)
                    {
                        client.ChangeState(new CheckVersionState(client));
                    }
                    NetworkManager.Instance.Invoke(() =>
                    {
                        client.callback.OnConnectResult(result);
                    });
                }
                else if (tag == MessageTag.ReconnectResult)
                {
                    var result = reader.MemRead<Result>();
                    var net = NetworkManager.Instance;
                    if (result.Success)
                    {
                        net.ReconnectSuccess();   
                    }
                    net.Invoke(() =>
                    {
                        client.callback.OnReconnectResult(result);
                    });
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
            NetworkManager.Instance.Invoke(() =>
            {
                callback.OnDisconnect();  
            });          
        }
    }
}