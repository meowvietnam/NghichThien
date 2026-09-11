using System;
using UnityEngine;
using UnityEngine.UI;

namespace IAP
{
    [Serializable]
    public class MonthlyPassPackData
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public MonthlyPassPackPanel Panel { get; private set; }
    }

    public class MonthlyPassPackUI : BaseBehaviour
    {
        [SerializeField] private Text packNameText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button viewButton;
        [SerializeField] private MonthlyPassPackData[] datas;
        private MonthlyPassPackPanel panel;

        public MonthlyPassPackPanel GetPanel()
        {
            return panel;
        }

        private void Start()
        {
            SetEventButton(viewButton, View);
        }

        public void SetDataIndex(int index, Transform container)
        {
            if (index < 0 || index >= datas.Length)
            {
                return;
            }

            var data = datas[index];
            SetIcon(data.Icon);
            panel = Instantiate(data.Panel, container);
            panel.gameObject.SetActive(false);
        }

        private void View()
        {
            panel.Show();
        }

        public void SetIcon(Sprite sprite)
        {
            iconImage.sprite = sprite;
        }

        public void SetName(string name)
        {
            packNameText.text = name;
        }
    }
}