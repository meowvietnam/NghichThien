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
        private ItemRef itemRef;
        [ShowInInspector, ReadOnly]
        public ItemRef ItemRef => itemRef;
        public bool IsHasItem { get; private set; }
        protected virtual void Init(IInventoryItemData data)
        {
            IsHasItem = data != null;
            if (!IsHasItem)
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

        public virtual void Init(ItemRef itemRef)
        {
            this.itemRef = itemRef;
            SetCount(itemRef.Count);
            var data = LuyenDanPanel.Instance.Resources.FindItem(itemRef);
            if (data != null)
            {
                itemRef.OnCountChanged += OnCountChanged;
            }
            Init(data);
        }

        private void SetCount(int count)
        {
            countText.text = count > 1 ? count.ToString() : "";
        }
        protected virtual void OnSelected()
        {
            Debug.Log("OnSelected");
        }
        private void OnCountChanged(int count)
        {
            SetCount(count);
            if (count <= 0)
            {
                SetActive(false);
            }
            else
            {
                SetActive(true);
            }
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
            if (IsHasItem)
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

        public virtual IInventoryItemData GetData()
        {
            return null;
        }
    }
}
