using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    [Serializable]
    public class MailCellImage
    {
        [field: SerializeField] public bool HasReward { get; private set; }
        [field: SerializeField] public Sprite Selected { get; private set; }
        [field: SerializeField] public Sprite Unselected { get; private set; }
    }

    public class MailCell : MonoBehaviour
    {
        [SerializeField] private Text mailFrom;
        [SerializeField] private Text title;
        [SerializeField] private Text time;
        [SerializeField] private Image background;
        [SerializeField] private Toggle toggle;
        [SerializeField] private GameObject isReadObject;
        [SerializeField] private MailCellImage[] images;
        private Mail mail;
        public Mail Mail => mail;
        private MailPanel mailPanel;

        private Sprite GetSelected()
        {
            var hasReward = mail?.HasReward() ?? false;
            return images.First(x => x.HasReward == hasReward).Selected;
        }

        private Sprite GetUnselected()
        {
            var hasReward = mail?.HasReward() ?? false;
            return images.First(x => x.HasReward == hasReward).Unselected;
        }

        public void Init(MailPanel mailPanel, Mail mail, ToggleGroup toggleGroup)
        {
            this.mailPanel = mailPanel;
            this.mail = mail;
            background.sprite = GetUnselected();
            title.text = mail.Title;
            UpdateTime();
            isReadObject.SetActive(!mail.IsRead);
            toggle.onValueChanged.AddListener(OnSelect);
            toggle.group = toggleGroup;
        }

        public void MarkMailRead()
        {
            isReadObject.SetActive(false);
        }

        public static string ToTimeAgo(TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 60)
                return $"{(int)timeSpan.TotalSeconds} giây trước";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
            return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }

        private void OnSelect(bool isOn)
        {
            background.sprite = isOn ? GetSelected() : GetUnselected();
            if (isOn)
            {
                mailPanel.SelectMail(mail);
            }
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnSelect);
        }

        public void UpdateTime()
        {
            time.text = ToTimeAgo(DateTime.UtcNow - mail.CreatedAt);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}