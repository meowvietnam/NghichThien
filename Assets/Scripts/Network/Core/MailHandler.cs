using System.Collections.Generic;
using System.Linq;
using AllMaps;
using CreateCharacter;
using LuyenDan;
using Network.Callback;
using Network.ClientState;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.Core
{
    public class MailHandler : IClientHandler
    {
        private IMailCallback callback;
        private Client Client => Client.Instance;
        private long startId;
        private bool hasNext;
        private int pageSize;
        private bool getLock;
        public long StartId => startId;
        public int PageSize => pageSize;
        public bool HasNext => hasNext;
        public bool GetLock => getLock;
        
        public MailHandler(IMailCallback callback)
        {
            this.callback = callback;
            startId = long.MaxValue;
            hasNext = true;
            pageSize = 50;
        }

        public void GetMails()
        {
            if (getLock) return;
            if (hasNext)
            {
                getLock = true;
                SendGetMails();
            }
        }
        private void SendGetMails()
        {
            var request = new GetMails()
            {
                StartId = startId,
                PageSize = pageSize
            };
            Client.Send(MessageTag.GetMails, request);
        }
        public void OnEnter()
        {
            GetMails();
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.GetMailsResult)
            {
                var result = reader.Read<GetMailsResult>();
                if (result.Success)
                {
                    var charData = Client.CharacterData;
                    charData.Social.Mails.AddRange(result.Mails);
                    startId = result.Mails.Last().Id;
                    hasNext = result.HasNext;
                    if (hasNext)
                    {
                        pageSize = result.Mails.Count;
                    }
                }
                callback.OnGetMails(result);
                getLock = false;
            }
            else if (tag == MessageTag.MarkMailRead)
            {
                var id = reader.Read<long>();
                var mails = Client.CharacterData.Social.Mails;
                var mail = mails.FirstOrDefault(x => x.Id == id);
                if (mail != null)
                {
                    mail.IsRead = true;
                }
                callback.MarkMailRead(id);
            }
            else if (tag == MessageTag.RemoveMailId)
            {
                var id = reader.Read<long>();
                var mails = Client.CharacterData.Social.Mails;
                var mail = mails.FirstOrDefault(x => x.Id == id);
                if (mail != null)
                {
                    mails.Remove(mail);
                }

                callback.RemoveMail(id);
            }
            else if (tag == MessageTag.RemoveAllMailsResult)
            {
                var removeMails = reader.ReadArray<long>().ToList();
                if (removeMails.Any())
                {
                    var mails = Client.CharacterData.Social.Mails;
                    mails.RemoveAll(x => removeMails.Contains(x.Id));
                    callback.RemoveAllMail(removeMails);
                }
            }
            else if (tag == MessageTag.ClaimMailResult)
            {
                var result = reader.Read<RewardResult>();
                ApplyClaim(result);
                callback.OnClaimMails(result);
            }
            else if (tag == MessageTag.ClaimAllMailsResult)
            {
                var result = reader.Read<RewardResult>();
                ApplyClaim(result);
                callback.OnClaimMails(result);
            }
            else if (tag == MessageTag.NewMail)
            {
                var mail = reader.Read<Mail>();
                var mails = Client.CharacterData.Social.Mails;
                mails.Add(mail);
                callback.AddMail(mail);
            }
        }

        private void ApplyClaim(RewardResult result)
        {
            var mails = Client.CharacterData.Social.Mails;
            foreach (var resultMail in result.Mails)
            {
                var mail = mails.FirstOrDefault(x => x.Id == resultMail);
                if (mail != null)
                {
                    mail.IsClaim = true;
                }
            }
            var rewards = result.Rewards;
            var charData = Client.CharacterData;
            foreach (var reward in rewards)
            {
                reward.ClaimReward(charData);
            }
        }



        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] clientHandler =
        {
            MessageTag.GetMailsResult, MessageTag.MarkMailRead, MessageTag.RemoveMailId,
            MessageTag.RemoveAllMailsResult,
            MessageTag.ClaimMailResult, MessageTag.ClaimAllMailsResult,
        };

        public MessageTag[] HandleTag => clientHandler;

        public void RemoveAll()
        {
            Client.Instance.Send(MessageTag.RemoveAllMails);
        }

        public void ReadMail(long id)
        {
            Client.Instance.Send(MessageTag.ReadMail, id);
        }

        public void ClaimAll()
        {
            Client.Instance.Send(MessageTag.ClaimAllMails);
        }
    }
}