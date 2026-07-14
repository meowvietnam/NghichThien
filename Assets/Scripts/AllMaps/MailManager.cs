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
            mailHandler.SyncLoadedMails();
            module.AddHandler(mailHandler);
            Observer.AddObserver(EventList.GamePlay.UPDATE_MAILS, RefreshMails);
            RefreshMails();
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
                    mailPanel.OnAdd(mails);
                    mailButton.OnAdd(mails);
                }
            });
        }

        public void MarkMailRead(long id)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.MarkMailRead(id);
                mailButton.Refresh();
            });
        }

        public void RemoveMail(long id)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.RemoveMail(id);
                mailButton.Refresh();
            });
        }

        public void RemoveAllMail(List<long> removeMails)
        {
            NetworkManager.Instance.Invoke(() => { mailPanel.RemoveAllMail(removeMails); });
        }

        public void OnClaimMails(RewardResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.OnClaimMails(result);
                var rewardItems = result.Rewards;

                var message = $"Bạn nhận được: {string.Join(", ", rewardItems.Select(x => x.GetRichText()))}";
                NotifyGroup.Instance.Show(message);
            });
        }

        public void AddMail(Mail mail)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                var message = "Bạn có 1 thư mới!";
                NotifyGroup.Instance.Show(message);
                mailPanel.AddMail(mail);
                mailButton.Refresh();
            });
        }

        private void RefreshMails()
        {
            NetworkManager.Instance.Invoke(() =>
            {
                var handler = Client.Instance.CoreModule.GetHandler<MailHandler>();
                handler?.SyncLoadedMails();
                var mails = Client.Instance.CharacterData?.Social?.Mails ?? new List<Mail>();
                mailPanel.SyncMails(mails);
                mailButton.Refresh();
            });
        }

        private void OnDestroy()
        {
            Observer.RemoveObserver(EventList.GamePlay.UPDATE_MAILS, RefreshMails);
        }
    }
}
