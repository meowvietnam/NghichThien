using System.Collections.Generic;
using Network.ClientState;

namespace Network.Callback
{
    public interface IMailCallback
    {
        void OnGetMails(GetMailsResult result);
        void MarkMailRead(int id);
        void RemoveMail(int id);
        void RemoveAllMail(List<int> removeMails);
        void OnClaimMails(RewardResult result);
    }
}