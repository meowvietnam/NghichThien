using System.Linq;
using CreateCharacter;
using Network.Callback;
using Network.Extensions;
using Network.Models;

namespace Network.ClientState
{
    public class SelectCharacter : IClientState
    {
        public ISelectCharacterCallback callback;

        public SelectCharacter(ISelectCharacterCallback callback)
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
            if (tag == MessageTag.GetCharactersResult)
            {
                var chars = reader.ReadArray<CharacterData>().ToList();
                Client.Instance.UserData.CharacterDatas = chars;
                callback.OnGetCharactes(chars);
            }
            else if (tag == MessageTag.CanCreateCharacterResult)
            {
                var result = reader.Read<CanCreateChar>();
                callback.OnCanCreateCharacter(result);
            }
            else if (tag == MessageTag.SelectCharacterResult)
            {
                var result = reader.Read<SelectCharacterResult>();
                callback.OnSelectCharacter(result);
            }
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.SelectCharacter;

        public void GetCharacters()
        {
            Client.Instance.Send(MessageTag.GetCharacters);
        }
    }
}