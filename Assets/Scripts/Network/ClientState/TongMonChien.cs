using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.ClientState
{
    /// <summary>Nhận các message của Tông môn chiến và chuyển về luồng chính cho controller của scene.</summary>
    public interface ITongMonChienCallback
    {
        void OnWarSnapshot(WarSnapshot snapshot);
        void OnWarDelta(WarDelta delta);
        void OnWarMoveResult(WarMoveResult result);
        void OnWarDeployPuppetResult(WarMoveResult result);
        void OnWarPuppetResult(WarPuppetResult result);
        void OnWarUseHpPillResult(WarUseHpPillResult result);
        void OnWarBattleMatched(WarBattleMatched matched);
        void OnWarKicked(WarKicked kicked);
        void OnWarNotice(WarNotice notice);
    }

    public class TongMonChien : IClientState
    {
        private readonly Client client;
        private readonly ITongMonChienCallback callback;

        public TongMonChien(ITongMonChienCallback callback)
        {
            client = Client.Instance;
            this.callback = callback;
        }

        public void OnEnter()
        {
            RequestSnapshot();
        }

        public void RequestSnapshot()
        {
            client.MemSend(MessageTag.WarGetSnapshot, new WarGetSnapshotRequest());
        }

        public void Move(int targetNodeId)
        {
            client.MemSend(MessageTag.WarMove, new WarMoveRequest { TargetNodeId = targetNodeId });
        }

        public void DeployPuppet(int puppetId, int targetNodeId)
        {
            client.MemSend(MessageTag.WarDeployPuppet,
                new WarDeployPuppetRequest { PuppetId = puppetId, TargetNodeId = targetNodeId });
        }

        /// <summary>
        /// Tạo rối. Gửi chi phí -1 để hỏi giá (server trả RequiresConfirmation), sau khi người chơi xác nhận thì
        /// gửi lại đúng chi phí đã xác nhận.
        /// </summary>
        public void CreatePuppet(long expectedContribPointsCost = -1, long expectedMoneyVipCost = -1)
        {
            client.MemSend(MessageTag.WarCreatePuppet, new WarCreatePuppetRequest
            {
                ExpectedContribPointsCost = expectedContribPointsCost,
                ExpectedMoneyVipCost = expectedMoneyVipCost
            });
        }

        public void UpgradePuppet(int puppetId, long expectedContribPointsCost = -1, long expectedMoneyVipCost = -1)
        {
            client.MemSend(MessageTag.WarUpgradePuppet, new WarUpgradePuppetRequest
            {
                PuppetId = puppetId,
                ExpectedContribPointsCost = expectedContribPointsCost,
                ExpectedMoneyVipCost = expectedMoneyVipCost
            });
        }

        /// <param name="puppetId">0 = dùng cho bản thân.</param>
        public void UseHpPill(int puppetId, ushort danId, ushort count)
        {
            client.MemSend(MessageTag.WarUseHpPill,
                new WarUseHpPillRequest { PuppetId = puppetId, DanId = danId, Count = count });
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            switch (tag)
            {
                case MessageTag.WarSnapshot:
                {
                    var model = reader.MemRead<WarSnapshot>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarSnapshot(model));
                    break;
                }
                case MessageTag.WarDelta:
                {
                    var model = reader.MemRead<WarDelta>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarDelta(model));
                    break;
                }
                case MessageTag.WarMoveResult:
                {
                    var model = reader.MemRead<WarMoveResult>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarMoveResult(model));
                    break;
                }
                case MessageTag.WarDeployPuppetResult:
                {
                    var model = reader.MemRead<WarMoveResult>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarDeployPuppetResult(model));
                    break;
                }
                case MessageTag.WarCreatePuppetResult:
                case MessageTag.WarUpgradePuppetResult:
                {
                    var model = reader.MemRead<WarPuppetResult>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarPuppetResult(model));
                    break;
                }
                case MessageTag.WarUseHpPillResult:
                {
                    var model = reader.MemRead<WarUseHpPillResult>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarUseHpPillResult(model));
                    break;
                }
                case MessageTag.WarBattleMatched:
                {
                    var model = reader.MemRead<WarBattleMatched>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarBattleMatched(model));
                    break;
                }
                case MessageTag.WarKicked:
                {
                    var model = reader.MemRead<WarKicked>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarKicked(model));
                    break;
                }
                case MessageTag.WarNotice:
                {
                    var model = reader.MemRead<WarNotice>();
                    NetworkManager.Instance.Invoke(() => callback.OnWarNotice(model));
                    break;
                }
            }
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.TongMonChien;
    }
}
