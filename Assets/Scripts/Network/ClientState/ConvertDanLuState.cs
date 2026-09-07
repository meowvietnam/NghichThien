using Network;

namespace Network.ClientState
{
    /// <summary>
    /// Client state active while the ConvertDanLu panel is open.
    /// Mirrors ConvertElementalSkillState.
    /// </summary>
    public class ConvertDanLuState : IClientState
    {
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.ConvertDanLu;
    }
}
