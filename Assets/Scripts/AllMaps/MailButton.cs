using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class MailButton : BaseBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject redPoint;
        [SerializeField] private MailPanel mailPanel;

        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        private void OnClick()
        {
            mailPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }

        public void OnAdd(List<Mail> mails)
        {
            Refresh();
        }

        public void Refresh()
        {
            var mails = Client.Instance.CharacterData?.Social?.Mails ?? new List<Mail>();
            var isNeverRead = mails.Any(x => !x.IsRead);
            redPoint.SetActive(isNeverRead);
        }
    }
}
