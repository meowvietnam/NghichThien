using System.Collections.Generic;
using System.Linq;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using Network;
using Network.Callback;
using Network.ClientState;
using Network.Core;
using UnityEngine;

namespace AllMaps
{
    public class MailManager : BaseBehaviour, IMailCallback
    {
        [SerializeField] private MailPanel mailPanel => Pool.Get<MailPanel>();
        [SerializeField] private MailButton mailButton => Pool.Get<UIHeadPanel>().mailBtn;

        private void Start()
        {
            var module = Client.Instance.CoreModule;
            module.RemoveHandler<MailHandler>();
            var mailHandler = new MailHandler(this);
            module.AddHandler(mailHandler);
        }
        public void OnGetMails(GetMailsResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (!result.Success)
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
                else
                {
                    var mails = result.Mails;
                    mailPanel.Init(mails);
                    mailButton.Init(mails);
                }
            });
        }

        public void MarkMailRead(int id)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.MarkMailRead(id);
                mailButton.Refresh();
            });
        }

        public void RemoveMail(int id)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.RemoveMail(id);
                mailButton.Refresh();
            });
        }

        public void RemoveAllMail(List<int> removeMails)
        {
            NetworkManager.Instance.Invoke(() => { mailPanel.RemoveAllMail(removeMails); });
        }

        public void OnClaimMails(RewardResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.OnClaimMails(result);
                var rewardItems  = result.RewardItems;
                var listItemRef = rewardItems.OtherItems;
                var furnaceList = rewardItems.Furnaces;
                var r = LuyenDanResources.Instance;
                var core = CoreConfig.Instance;
                var listItem = listItemRef.Select(x => r.FindItem(x)).ToList();
                listItem.AddRange(furnaceList.Select(x => r.FindItem(x)));
                var message = $"Bạn nhận được: {string.Join(", ", listItem.Select(x => $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(x.Tier).Color)}>{x.Name}</color>"))}";
                NotifyGroup.Instance.Show(message);
            });
        }
        public void AddMail(Mail mail)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.AddMail(mail);
            });
        }
    }
}