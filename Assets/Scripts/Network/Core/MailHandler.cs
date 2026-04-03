using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LuyenDan;
using Network.Callback;
using Network.ClientState;
using Network.Extensions;
using UnityEngine;

namespace Network.Core
{
    public class MailHandler : IClientHandler
    {
        private IMailCallback callback;
        private Client Client => Client.Instance;

        public MailHandler(IMailCallback callback)
        {
            this.callback = callback;
        }

        public void OnEnter()
        {
            Client.Send(MessageTag.GetMails);
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
                    charData.Mails = result.Mails;
                }

                callback.OnGetMails(result);
            }
            else if (tag == MessageTag.MarkMailRead)
            {
                var id = reader.Read<int>();
                var mails = Client.CharacterData.Mails;
                var mail = mails.FirstOrDefault(x => x.Id == id);
                if (mail != null)
                {
                    mail.IsRead = true;
                }

                callback.MarkMailRead(id);
            }
            else if (tag == MessageTag.RemoveMailId)
            {
                var id = reader.Read<int>();
                var mails = Client.CharacterData.Mails;
                var mail = mails.FirstOrDefault(x => x.Id == id);
                if (mail != null)
                {
                    mails.Remove(mail);
                }

                callback.RemoveMail(id);
            }
            else if (tag == MessageTag.RemoveAllMailsResult)
            {
                var removeMails = reader.ReadArray<int>().ToList();
                if (removeMails.Any())
                {
                    var mails = Client.CharacterData.Mails;
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
        }

        private void ApplyClaim(RewardResult result)
        {
            var mails = Client.CharacterData.Mails;
            foreach (var resultMail in result.Mails)
            {
                var mail = mails.FirstOrDefault(x => x.Id == resultMail);
                if (mail != null)
                {
                    mail.IsClaim = true;
                }
            }

            var inventory = Client.CharacterData.InventoryManager;
            AddToInventory(inventory, result.RewardItems);
        }

        private void Add(HashSet<ItemRef> inventory, ItemRef itemRef)
        {
            if (!inventory.Add(itemRef))
            {
                var item = inventory.First(x => x.Id == itemRef.Id);
                item.Count += itemRef.Count;
            }
        }

        private void AddToInventory(InventoryManager inventory, RewardItems rewardItems)
        {
            var items = rewardItems.OtherItems;
            var furnaces = rewardItems.Furnaces;
            foreach (var furnaceData in furnaces)
            {
                inventory.Furnaces.Add(furnaceData);
            }

            foreach (var itemRef in items)
            {
                var type = itemRef.Type;
                if (type == ItemType.Herb)
                {
                    Add(inventory.Herbs, itemRef);
                }
                else if (type == ItemType.LinhDan)
                {
                    Add(inventory.LinhDans, itemRef);
                }
                else if (type == ItemType.ThanThong)
                {
                    Add(inventory.ThanThongs, itemRef);
                }
                else if (type == ItemType.CongPhap)
                {
                    Add(inventory.CongPhaps, itemRef);
                }
                else if (type == ItemType.VuKhi)
                {
                    Add(inventory.VuKhis, itemRef);
                }
                else if (type == ItemType.PhongCu)
                {
                    Add(inventory.PhongCus, itemRef);
                }
                else if (type == ItemType.TrangSuc)
                {
                    Add(inventory.TrangSucs, itemRef);
                }
                else if (type == ItemType.Recipe)
                {
                    Add(inventory.Recipes, itemRef);
                }
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
    }
}