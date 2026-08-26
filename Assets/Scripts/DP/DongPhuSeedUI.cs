using System;
using Base;
using LongDHModules;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class DongPhuSeedUI : BaseBehaviour
    {
        private ushort herbId;
        private int count;
        [SerializeField] private Image qualityUp;
        [SerializeField] private Image qualityFull;
        [SerializeField] private Image icon;
        [SerializeField] private Text countText;
        [SerializeField] private Text nameText;
        [SerializeField] private Button button;
        [SerializeField] private Button infoButton;
        public event Action Clicked;
        private void Start()
        {
            SetEventButton(infoButton, ShowInfo);
            SetEventButton(button, OnClick);
        }
        private void OnClick()
        {
            Clicked?.Invoke();
        }
        private void ShowInfo()
        {
            var toolTipItems = ToolTipItems.Instance;
            var herbData = GetItemData();
            var itemRef = new ItemRef(herbId,ItemType.Herb, 1);
            toolTipItems.ShowInfoItem(herbData, itemRef);
        }

        public bool IsThis(ushort herbId)
        {
            return this.herbId == herbId;
        }
        public void Init(ushort herbId, int count)
        {
            SetHerbId(herbId);
            SetCount(count);
        }

        private HerbData GetItemData()
        {
            return LuyenDanResources.Instance.FindHerb(herbId);
        }
        public void SetHerbId(ushort herbId)
        {
            this.herbId = herbId;
            var data = GetItemData();
            var quality = CoreConfig.Instance.FindQuality(data.Tier);
            qualityUp.sprite = quality.IconUp;
            qualityFull.sprite = quality.Background;
            icon.sprite = data.Icon;
            nameText.text = data.Name;
            nameText.color = quality.Color;
        }
        public void SetCount(int count)
        {
            this.count = count;
            countText.text = count > 1 ? Utils.FormatCount(count) : string.Empty;
        }
    }
}