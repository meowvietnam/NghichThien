using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class InventoryItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        [TabGroup("Has Item")]
        [SerializeField] private Button selectButton;
        [TabGroup("Has Item")] 
        [SerializeField] private GameObject hover;
        private IInventoryItemData data;
        private ItemRef itemRef;
        public Tier Tier => data.Tier;
        private void Init(IInventoryItemData data)
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
            var tier = data.Tier;
            SetTier(tier);
            selectButton.onClick.AddListener(OnSelected);
        }

        public void Init(ItemRef itemRef)
        {
            this.itemRef = itemRef;
            var data = LuyenDanPanel.Instance.Resources.FindItem(itemRef);
            Init(data);
            
        }
        private void OnSelected()
        {
            Debug.Log("OnSelected");
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
                itemRef.OnCountChanged -= OnCountChanged;
                selectButton.onClick.RemoveListener(OnSelected);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hover.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hover.SetActive(false);
        }
    }
}
