using System;
using Base;
using UnityEngine;
using NativeTcp = System.Net.Sockets.TcpClient;

namespace Network
{
    public abstract class ClientConnection : BaseClient<MessageTag>
    {
        public TcpClient TcpClient { get; protected set; }

        // Lưu ở mức kết nối (không gắn vào đối tượng Tcp) để vẫn giữ listener khi đổi socket lúc kết nối lại.
        private event Action ReconnectedSuccess;

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
            ReconnectedSuccess += onReconnected;
        }

        public void RemoveReconnectedListener(Action onReconnected)
        {
            ReconnectedSuccess -= onReconnected;
        }

        /// <summary>
        /// Nhận socket mới sau khi xác thực kết nối lại. Chuyển nguyên protocol (cùng vòng đọc đang chạy) của kết nối
        /// tạm sang client này thay vì <c>Tcp.Reconnect</c>: Reconnect mở vòng đọc thứ hai trên cùng socket trong khi
        /// vòng đọc cũ của kết nối tạm vẫn còn một lệnh ReadAsync đang chờ (Mono không huỷ được), hai vòng đọc giành
        /// byte của nhau làm hỏng gói tin (MemoryPackSerializationException / sai độ dài).
        /// </summary>
        public void AdoptReconnected(ClientConnection source)
        {
            var newTcp = source.TcpClient;
            var oldTcp = TcpClient;
            if (newTcp == null || ReferenceEquals(newTcp, oldTcp)) return;

            if (oldTcp != null)
            {
                // Tháo socket cũ trước để Disconnect của nó không báo mất kết nối cho client này nữa.
                oldTcp.StopRead();
                RemoveProtocol(oldTcp);
                oldTcp.Disconnect();
            }

            source.TransferProtocol(newTcp, this);
            TcpClient = newTcp;
            ReconnectedSuccess?.Invoke();
        }
    }
}
