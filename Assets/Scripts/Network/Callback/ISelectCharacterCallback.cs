using System.Collections.Generic;
using CreateCharacter;
using Network.ClientState;
using Network.Models;

namespace Network.Callback
{
    public interface ISelectCharacterCallback
    {
        void OnGetCharactes(List<CharacterData> characterDatas);
        void OnCanCreateCharacter(CanCreateChar result);
        void OnSelectCharacter(SelectCharacterResult result);
    }
}