using System;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.Core
{
    public class BossHandler : IClientHandler
    {
        private int requestedNode;
        private Action<BossStatus> statusCallback;
        private Action<JoinBossResult> joinCallback;

        public void GetStatus(int nodeId, Action<BossStatus> callback)
        {
            requestedNode = nodeId;
            statusCallback = callback;
            Client.Instance.MemSend(MessageTag.GetBossStatus, nodeId);
        }

        public void Join(BossStatus status, Action<JoinBossResult> callback)
        {
            if (joinCallback != null) return;
            joinCallback = callback;
            Client.Instance.MemSend(MessageTag.JoinBoss, new JoinBossRequest
            {
                SpawnId = status.SpawnId, NodeId = status.NodeId
            });
        }

        public void Handle(Message<MessageTag> message)
        {
            if (message.Tag == MessageTag.BossStatus)
            {
                var result = message.Reader.MemRead<BossStatus>();
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.NodeId != requestedNode) return;
                    var callback = statusCallback;
                    statusCallback = null;
                    callback?.Invoke(result);
                });
            }
            else if (message.Tag == MessageTag.JoinBossResult)
            {
                var result = message.Reader.MemRead<JoinBossResult>();
                NetworkManager.Instance.Invoke(() =>
                {
                    var callback = joinCallback;
                    joinCallback = null;
                    callback?.Invoke(result);
                });
            }
        }

        public void OnEnter() { }
        public void OnLeave() { statusCallback = null; joinCallback = null; }
        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                var callback = joinCallback;
                joinCallback = null;
                statusCallback = null;
                callback?.Invoke(new JoinBossResult { Message = "Mất kết nối tới máy chủ." });
            });
        }
        public MessageTag[] HandleTag => new[] { MessageTag.BossStatus, MessageTag.JoinBossResult };
    }
}
