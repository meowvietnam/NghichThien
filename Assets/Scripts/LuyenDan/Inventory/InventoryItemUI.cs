using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] private GameObject thisObject;
        [TabGroup("Null Item")]
        [SerializeField] private GameObject nullItem;
        
        [TabGroup("Has Item")]
        [SerializeField] private GameObject hasItem;
        [TabGroup("Has Item")]
        [SerializeField] private Image icon;
        [TabGroup("Has Item")]
        [SerializeField] private Image background;
        [TabGroup("Has Item")]
        [SerializeField] private Image iconUp;
        [TabGroup("Has Item")]
        [SerializeField] private Text countText;
        [TabGroup("Has Item")]
        [SerializeField] private Text nameText;
        [TabGroup("Has Item")]
        [SerializeField] private Button infoButton;
        private IInventoryItemData data;
        public Tier Tier => data.Tier;
        public void Init(IInventoryItemData data)
        {
            this.data = data;
            if (data == null)
            {
                nullItem.SetActive(true);
                hasItem.SetActive(false);
                return;
            }
            nullItem.SetActive(false);
            hasItem.SetActive(true);
            icon.sprite = data.Icon;
            nameText.text = data.Name;
            countText.text = data.Count.ToString();
            var tier = data.Tier;
            SetTier(tier);
            data.OnCountChanged += OnCountChanged;
        }
        
        private void OnCountChanged(int count)
        {
            countText.text = count.ToString();
        }
        private void SetTier(Tier tier)
        {
            var quality = LuyenDanPanel.Instance.Resources.Qualities.First(x => x.Tier == tier);
            background.sprite = quality.Background;
            iconUp.sprite = quality.IconUp;
            nameText.color = quality.Color;
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }
        private void OnDestroy()
        {
            if (data != null)
            {
                data.OnCountChanged -= OnCountChanged;
            }
        }
    }
}
