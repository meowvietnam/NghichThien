using System.Collections.Generic;
using AllMaps;
using Network.ClientState;

namespace Network.Callback
{
    public interface IMailCallback
    {
        void OnGetMails(GetMailsResult result);
        void MarkMailRead(long id);
        void RemoveMail(long id);
        void RemoveAllMail(List<long> removeMails);
        void OnClaimMails(RewardResult result);
        void AddMail(Mail mail);
    }
}