using System.Collections.Generic;
using Network.ClientState;
using Network.Models;

namespace Network.Callback
{
    public interface IAllMapsCallback
    {
        void OnMoveToMapResult(MoveToMapResult result);
        void OnGetCharacterOnlineThisNode(GetCharacterOnlineThisNodeResult result);
        void OnGetMails(GetMailsResult result);
        void MarkMailRead(int id);
        void RemoveMail(int id);
        void RemoveAllMail(List<int> removeMails);
    }
}