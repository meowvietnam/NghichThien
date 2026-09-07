namespace Network.ClientState
{
    public class ConvertElementalSkillState : IClientState
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

        public MessageState State => MessageState.ConvertElementalSkill;
    }
}