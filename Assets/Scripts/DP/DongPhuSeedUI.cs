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
        public ushort HerbId { get; private set; }
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
            var toolTipItems = DongPhuPanel.Instance.ToolTipItems;
            var herbData = GetItemData();
            var itemRef = new ItemRef(HerbId,ItemType.Herb, 1);
            toolTipItems.ShowInfoItem(herbData, itemRef);
        }
        public void Init(ushort herbId, int count)
        {
            SetHerbId(herbId);
            SetCount(count);
        }

        private HerbData GetItemData()
        {
            return LuyenDanResources.Instance.FindHerb(HerbId);
        }
        public void SetHerbId(ushort herbId)
        {
            this.HerbId = herbId;
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
            countText.text = count > 1 ? Utils.FormatCount(count) : string.Empty;
        }
    }
}