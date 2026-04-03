using Base;
using LongDH.LongDHModules;
using Network;
using UnityEngine;
using UnityEngine.UI;
using Network.Extensions;

namespace AllMaps
{
    public class MailUI : MonoBehaviour
    {
        [SerializeField] private ClampHeight clampHeight;
        [SerializeField] private Text timeText;
        [SerializeField] private Text titleText;
        [SerializeField] private Text contentText;
        [SerializeField] private RewardListUI rewardListUI;
        [SerializeField] private GameObject selfObject;
        private Mail mail;
        public Mail Mail => mail;
        private const string confirmRemoveMessage = "Xác nhận xóa khi chưa nhận vật phẩm?";

        public void Init(Mail mail)
        {
            this.mail = mail;
            timeText.text = mail.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
            titleText.text = mail.Title;
            contentText.text = mail.Content;
            rewardListUI.Init(this, mail.Rewards, mail.IsClaim);
            clampHeight.Clamp();
        }

        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }

        public void OnClaim()
        {
            Client.Instance.Send(MessageTag.ClaimMail, mail.Id);
        }

        public void RemoveMail()
        {
            if (mail.Rewards.Count > 0 && !mail.IsClaim)
            {
                var popup = Pool.Get<PopupConfirmAction>();
                popup.ShowPopConfirmAction(confirmRemoveMessage,ConfirmRemove);
                return;
            }
            ConfirmRemove();
        }
        private void ConfirmRemove()
        {
            Client.Instance.Send(MessageTag.RemoveMail, mail.Id);
        }
    }
}