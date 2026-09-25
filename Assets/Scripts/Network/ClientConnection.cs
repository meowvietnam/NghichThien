using System;
using Base;
using UnityEngine;
using NativeTcp = System.Net.Sockets.TcpClient;

namespace Network
{
    public abstract class ClientConnection : BaseClient<MessageTag>
    {
        public TcpClient TcpClient { get; protected set; }

        public ClientConnection()
        {
        }

        public ClientConnection(NativeTcp client)
        {
            TcpClient = new TcpClient(client);
            AddProtocol(TcpClient);
            TcpClient.StartRead();
        }

        public override bool ValidatePacketSize(int size)
        {
            Debug.Log(Utils.FormatCount(size) + " bytes");
            return size >= 0 && size < 1024 * 1024 * 128;
        }

        public void AddReconnectedListener(Action onReconnected)
        {
            TcpClient.ReconnectedSuccess += onReconnected;
        }

        public void RemoveReconnectedListener(Action onReconnected)
        {
            TcpClient.ReconnectedSuccess -= onReconnected;
        }
    }
}
