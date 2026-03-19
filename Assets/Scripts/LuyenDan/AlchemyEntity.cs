using System.Linq;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenDan
{
    public class AlchemyEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button btn;

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
        [SerializeField] private Text nameText;
        [TabGroup("Has Item")]
        [SerializeField] private GameObject hover;
        public InventoryItemUI Item { get; protected set; }

        private void OnValidate()
        {
            btn = GetComponent<Button>();
            thisObject = gameObject;
            var tf = thisObject.transform;
            nullItem = tf.Find("Null").gameObject;
            hasItem = tf.Find("HasItem").gameObject;
            icon = hasItem.transform.Find("IconMask/Icon").GetComponent<Image>();
            background = hasItem.transform.Find("Quality").GetComponent<Image>();
            iconUp = background.transform.Find("QualityUp").GetComponent<Image>();
            hover = tf.Find("Selected").gameObject;
            nameText = hasItem.transform.Find("NameMask/NameText").GetComponent<Text>();
        }
        private void Start()
        {
            btn.onClick.AddListener(OnClick);
        }
        protected void SetData(IInventoryItemData data)
        {
            var isHasItem = data != null;
            if (!isHasItem)
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
        }
        public virtual void OnLuyenDanCompleted()
        {

        }
        [Button]
        public virtual void Select(InventoryItemUI item)
        {
            Item = item;
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
        public void OnPointerEnter(PointerEventData eventData)
        {
            hover.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            hover.SetActive(false);
        }
        protected virtual void OnClick()
        {
            LuyenDanPanel.Instance.Alchermy.SetCurrentSelected(this);
        }
        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}