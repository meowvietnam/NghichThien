using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using Network.Core;
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
        [SerializeField] private GameObject selfObject;
        [SerializeField] private RectTransform panelTransform;
        [SerializeField] private GameObject mask;
        [SerializeField] private Button close;
        [SerializeField] private Button claimAllBtn;
        [SerializeField] private Button removeAllBtn;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField, Range(0f, 1f)] private float loadMoreThreshold = 0.1f;
        private float time = 0;
        [SerializeField] private string removeAllMessage;
        private MailHandler mailHandler;
        private MailUI mailUIInstance;
        private Func<MailCell, bool> filter;

        private Func<MailCell, bool> GetFilter()
        {
            if (filter == null)
            {
                filter = All;
            }

            return filter;
        }

        private void Start()
        {
            filter = All;
            SetEventButton(close, OnClose);
            SetEventButton(claimAllBtn, ClaimAll);
            SetEventButton(removeAllBtn, RemoveAll);
            mailHandler = Client.Instance.CoreModule.GetHandler<MailHandler>();
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private bool All(MailCell cell)
        {
            return true;
        }

        private bool Unread(MailCell cell)
        {
            return !cell.Mail.IsRead;
        }

        private void OnScrollValueChanged(Vector2 normalizedPosition)
        {
            if (normalizedPosition.y <= loadMoreThreshold)
            {
                mailHandler.GetMails();
            }
        }

        private void RemoveAll()
        {
            var popup = Pool.Get<PopupConfirmAction>();
            popup.ShowPopConfirmAction(removeAllMessage, OnRemoveAllConfirm);
        }

        private void OnRemoveAllConfirm()
        {
            mailHandler.RemoveAll();
        }

        private void ClaimAll()
        {
            mailHandler.ClaimAll();
        }

        private void FixedUpdate()
        {
            time += Time.fixedDeltaTime;
            if (time > 10)
            {
                time = 0;
                lock (mailCells)
                {
                    foreach (var mailCell in mailCells)
                    {
                        mailCell.UpdateTime();
                    }
                }
            }
        }

        public void OnAdd(List<Mail> mails)
        {
            var ids = mails.Select(x => x.Id).ToHashSet();
            for (var i = mailCells.Count - 1; i >= 0; i--)
            {
                var mailCell = mailCells[i];
                if (mailCell == null || mailCell.Mail == null || ids.Contains(mailCell.Mail.Id))
                {
                    mailCells.RemoveAt(i);
                    Destroy(mailCell.gameObject);
                }
            }

            foreach (var mail in mails.OrderByDescending(x => x.CreatedAt))
            {
                AddMail(mail, false);
            }

            SortMailCellsByDate();
        }

        public void SyncMails(List<Mail> mails)
        {
            foreach (var mailCell in mailCells)
            {
                if (mailCell != null)
                {
                    Destroy(mailCell.gameObject);
                }
            }

            mailCells.Clear();
            mailUIInstance?.SetActive(false);
            OnAdd(mails ?? new List<Mail>());
        }

        private void OnClose()
        {
            SetActive(false);
        }

        private void OnDestroy()
        {
            close.onClick.RemoveAllListeners();
            claimAllBtn.onClick.RemoveAllListeners();
            removeAllBtn.onClick.RemoveAllListeners();
            scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }

        public void SelectMail(Mail mail)
        {
            var id = mail.Id;
            if (mailUIInstance == null) return;
            mailUIInstance.Init(mail);
            mailUIInstance.SetActive(true);
            if (!mail.IsRead)
            {
                mailHandler.ReadMail(id);
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

        public void MarkMailRead(long id)
        {
            var mailCell = mailCells.FirstOrDefault(x => x.Mail.Id == id);
            if (mailCell != null)
            {
                mailCell.MarkMailRead();
            }
        }

        public void RemoveMail(long id)
        {
            var mailCell = mailCells.FirstOrDefault(x => x.Mail.Id == id);
            if (mailCell != null)
            {
                mailCells.Remove(mailCell);
                Destroy(mailCell.gameObject);
            }

            var mail = mailUIInstance.Mail;
            if (mail == null) return;
            var idInstance = mail.Id;
            if (idInstance == id)
            {
                mailUIInstance.SetActive(false);
            }
        }

        public void Filter(MailFilterType type)
        {
            if (type == MailFilterType.All)
            {
                filter = All;
            }
            else if (type == MailFilterType.NeverRead)
            {
                filter = Unread;
            }

            foreach (var mailCell in mailCells)
            {
                var active = filter(mailCell);
                mailCell.SetActive(active);
            }
        }

        public void RemoveAllMail(List<long> removeMails)
        {
            removeMails.ForEach(RemoveMail);
        }

        public void OnClaimMails(RewardResult result)
        {
            var mailIds = result.Mails;
            if (mailUIInstance == null) return;
            var mail = mailUIInstance.Mail;
            if (mail == null) return;
            var id = mail.Id;
            if (mailIds.Contains(id))
            {
                mailUIInstance.MarkAsClaimed();
            }
        }

        public void AddMail(Mail mail)
        {
            AddMail(mail, true);
        }

        private void AddMail(Mail mail, bool sortAfterAdd)
        {
            if (mailUIInstance == null)
            {
                var mailUI = Instantiate(mailUIPrefab, mailUIParent);
                mailUI.SetActive(false);
                mailUIInstance = mailUI;
            }

            var mailCell = Instantiate(mailCellPrefab, mailCellParent);
            mailCell.Init(this, mail, toggleGroup);
            var active = GetFilter()(mailCell);
            mailCell.SetActive(active);
            mailCells.Add(mailCell);
            if (sortAfterAdd)
            {
                SortMailCellsByDate();
            }
        }

        private void SortMailCellsByDate()
        {
            mailCells = mailCells
                .OrderByDescending(x => x.Mail.CreatedAt)
                .ThenByDescending(x => x.Mail.Id)
                .ToList();

            var reverseArrangement =
                mailCellParent.GetComponent<HorizontalOrVerticalLayoutGroup>()?.reverseArrangement == true;
            for (var i = 0; i < mailCells.Count; i++)
            {
                var siblingIndex = reverseArrangement ? mailCells.Count - 1 - i : i;
                mailCells[i].transform.SetSiblingIndex(siblingIndex);
            }
        }
    }
}