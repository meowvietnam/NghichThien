using LongDH.LongDHModules;

namespace Network.ClientState
{
    public class CombatPvp : IClientState
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

        public MessageState State => MessageState.CombatPvp;
    }
}