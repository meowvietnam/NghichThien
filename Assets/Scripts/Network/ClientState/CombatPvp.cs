using UnityEngine;

namespace Network.ClientState
{
    public class CombatPvp : IClientState
    {
        
        public void OnEnter()
        {
            Debug.Log("Join Combat Pvp");
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