using Base;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class ItemEventUI : BaseBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text nameText;
        [SerializeField] private Text countText;

        public void Init(ItemEvent item)
        {
            var data = item.GetItemEventData();
            SetCount(item.Count);
            SetName(data.Name);
            SetIcon(data.Icon);
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public void SetIcon(Sprite icon)
        {
            this.icon.sprite = icon;
        }

        public void SetCount(int count)
        {
            var text = count > 1 ? Utils.FormatCount(count) : string.Empty;
            countText.text = text;
        }

        public void SetCountText(string text)
        {
            countText.text = text;
        }
    }
}
