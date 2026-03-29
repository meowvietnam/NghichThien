using Network.ClientState;
using Network.Models;

namespace Network.Callback
{
    public interface IAllMapsCallback
    {
        void OnMoveToMapResult(MoveToMapResult result);
        void OnGetCharacterOnlineThisNode(GetCharacterOnlineThisNodeResult result);
        void OnGetMails(GetMailsResult result);
    }
}