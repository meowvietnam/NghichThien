using Network.Callback;
using Network.Extensions;
using Network.Models;

namespace Network.ClientState
{
    public class CreateCharacter : IClientState
    {
        private ICreateCharacterCallback callback;
        public CreateCharacter(ICreateCharacterCallback callback)
        {
            this.callback = callback;
        }
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.BackToSelectCharOk)
            {
                callback.BackToSelectChar();
            }
            else if (tag == MessageTag.SetCharNameResult)
            {
                var result = reader.Read<SetCharNameResult>();
                callback.OnSetNameResult(result);
            }
            else if (tag == MessageTag.SetSkinsResult)
            {
                var result = reader.Read<SetSkinResult>();
                callback.OnSetSkinsResult(result);
            }
            else if (tag == MessageTag.ApplyStatsResult)
            {
                var result = reader.Read<ApplyStatsResult>();
                callback.OnApplyStatsResult(result);
            }
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.CreateCharacter;
    }
}