using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class InventoryItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject selfObject;
        [TabGroup("Null Item")]
        [SerializeField] private GameObject emptySlot;
        
        [TabGroup("Has Item")]
        [SerializeField] private GameObject filledSlot;
        [TabGroup("Has Item")]
        [SerializeField] private Image itemIcon;
        [TabGroup("Has Item")]
        [SerializeField] private Image qualityBackground;
        [TabGroup("Has Item")]
        [SerializeField] private Image qualityIconUp;
        [TabGroup("Has Item")]
        [SerializeField] private Text countText;
        [TabGroup("Has Item")]
        [SerializeField] private Text itemNameText;
        [TabGroup("Has Item")]
        [SerializeField] private Button infoButton;
        [TabGroup("Has Item")]
        [SerializeField] private Button selectButton;
        [TabGroup("Has Item")] 
        [SerializeField] private GameObject hoverHighlight;
        private ItemRef itemRef;
        [ShowInInspector, ReadOnly]
        public ItemRef ItemRef => itemRef;
        public bool IsHasItem { get; private set; }
        private void OnValidate()
        {
            selfObject   = gameObject;
        
            var tf = selfObject.transform;
            emptySlot  = tf.Find("Null").gameObject;
            filledSlot = tf.Find("HasItem").gameObject;
        
            itemIcon          = filledSlot.transform.Find("IconMask/Icon").GetComponent<Image>();
            qualityBackground = filledSlot.transform.Find("Quality").GetComponent<Image>();
            qualityIconUp     = qualityBackground.transform.Find("QualityUp").GetComponent<Image>();
            hoverHighlight    = tf.Find("HasItem/Selected").gameObject;
            itemNameText      = filledSlot.transform.Find("NameMask/NameText").GetComponent<Text>();
        }

        protected virtual void Init(IInventoryItemData data)
        {
            IsHasItem = data != null;
            if (!IsHasItem)
            {
                emptySlot.SetActive(true);
                filledSlot.SetActive(false);
                return;
            }
            emptySlot.SetActive(false);
            filledSlot.SetActive(true);
            itemIcon.sprite = data.Icon;
            itemNameText.text = data.Name;
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
            SetActive(count > 0);
        }
        private void SetTier(Tier tier)
        {
            var quality = LuyenDanPanel.Instance.Resources.Qualities.First(x => x.Tier == tier);
            qualityBackground.sprite = quality.Background;
            qualityIconUp.sprite = quality.IconUp;
            itemNameText.color = quality.Color;
        }
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
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
            hoverHighlight.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverHighlight.SetActive(false);
        }

        public virtual IInventoryItemData GetData()
        {
            return null;
        }
    }
}
