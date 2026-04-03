using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Network;
using UnityEngine;
using UnityEngine.UI;
using Network.Extensions;

namespace AllMaps
{
    public class MailPanel : PanelGroup
    {
        [SerializeField] private MailUI mailUIPrefab;
        [SerializeField] private Transform mailUIParent;
        [SerializeField] private MailCell mailCellPrefab;
        [SerializeField] private Transform mailCellParent;
        [SerializeField] private ToggleGroup toggleGroup;
        private List<MailCell> mailCells = new List<MailCell>();
        private List<MailUI> mailUIs = new List<MailUI>();
        [SerializeField] private GameObject selfObject;
        [SerializeField] private RectTransform panelTransform;
        [SerializeField] private GameObject mask;
        [SerializeField] private Button close;
        private float time = 0;
        private MailUI opening;

        private void Start()
        {
            SetEventButton(close, OnClose);
        }

        private void FixedUpdate()
        {
            time += Time.fixedDeltaTime;
            if (time > 10)
            {
                time = 0;
                foreach (var mailCell in mailCells)
                {
                    mailCell.UpdateTime();
                }
            }
        }

        public void Init(List<Mail> mails)
        {
            foreach (var mail in mails)
            {
                var mailUI = Instantiate(mailUIPrefab, mailUIParent);
                mailUI.Init(mail);
                mailUIs.Add(mailUI);
                mailUI.SetActive(false);
                var mailCell = Instantiate(mailCellPrefab, mailCellParent);
                mailCell.Init(this, mail, toggleGroup);
                mailCells.Add(mailCell);
            }
        }

        private void OnClose()
        {
            SetActive(false);
        }

        private void OnDestroy()
        {
            close.onClick.RemoveAllListeners();
        }

        public void SelectMail(Mail mail)
        {
            var id = mail.Id;
            Client.Instance.Send(MessageTag.ReadMail, id);
            if (opening != null)
            {
                opening.SetActive(false);
            }

            var mailUI = mailUIs.FirstOrDefault(x => x.Mail.Id == id);
            if (mailUI != null)
            {
                opening = mailUI;
                opening.SetActive(true);
            }
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                mask.SetActive(true);
                selfObject.SetActive(true);
                selfObject.transform.SetAsFirstSibling();
                panelTransform.localScale = Vector2.zero;
                panelTransform.DOScale(Vector3.one, 0.25f);
            }
            else
            {
                panelTransform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
                {
                    mask.SetActive(false);
                    selfObject.SetActive(false);
                });
            }
        }

        public void MarkMailRead(int id)
        {
            var mailCell = mailCells.FirstOrDefault(x => x.Mail.Id == id);
            if (mailCell != null)
            {
                mailCell.MarkMailRead();
            }
        }

        public void RemoveMail(int id)
        {
            var mailCell = mailCells.FirstOrDefault(x => x.Mail.Id == id);
            if (mailCell != null)
            {
                mailCells.Remove(mailCell);
                Destroy(mailCell.gameObject);
            }

            var mailUI = mailUIs.FirstOrDefault(x => x.Mail.Id == id);
            if (mailUI != null)
            {
                mailUIs.Remove(mailUI);
                Destroy(mailUI.gameObject);
            }
        }

        public void Filter(MailFilterType type)
        {
            if (type == MailFilterType.All)
            {
                foreach (var mailCell in mailCells)
                {
                    mailCell.SetActive(true);
                }
            }
            else if(type == MailFilterType.NeverRead)
            {
                foreach (var mailCell in mailCells)
                {
                    var active = !mailCell.Mail.IsRead;
                    mailCell.SetActive(active);
                }
            }
        }
    }
}