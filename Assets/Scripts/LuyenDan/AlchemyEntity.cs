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
        [SerializeField] private Button clickButton;
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
        [SerializeField] private Text itemNameText;
        [TabGroup("Has Item")]
        [SerializeField] private GameObject hoverHighlight;
        public InventoryItemUI Item { get; protected set; }
        public bool IsEmpty => Item == null;
        // private void OnValidate()
        // {
        //     clickButton = GetComponent<Button>();
        //     selfObject   = gameObject;
        //
        //     var tf = selfObject.transform;
        //     emptySlot  = tf.Find("Null").gameObject;
        //     filledSlot = tf.Find("HasItem").gameObject;
        //
        //     itemIcon          = filledSlot.transform.Find("IconMask/Icon").GetComponent<Image>();
        //     qualityBackground = filledSlot.transform.Find("Quality").GetComponent<Image>();
        //     qualityIconUp     = qualityBackground.transform.Find("QualityUp").GetComponent<Image>();
        //     hoverHighlight    = tf.Find("Selected").gameObject;
        //     itemNameText      = filledSlot.transform.Find("NameMask/NameText").GetComponent<Text>();
        // }

        private void Start()
        {
            clickButton.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            clickButton.onClick.RemoveAllListeners();
        }
        public void SetActive(bool active) => selfObject.SetActive(active);
        public virtual void OnLuyenDanCompleted() { }

        public virtual void Select(InventoryItemUI item)
        {
            Item = item;
        }


        public void OnPointerEnter(PointerEventData eventData) => hoverHighlight.SetActive(true);
        public void OnPointerExit(PointerEventData eventData)  => hoverHighlight.SetActive(false);

        protected void SetData(IInventoryItemData data)
        {
            bool hasItem = data != null;
            emptySlot.SetActive(!hasItem);
            filledSlot.SetActive(hasItem);

            if (!hasItem) return;

            itemIcon.sprite  = data.Icon;
            itemNameText.text = data.Name;
            ApplyQualityTheme(data.Tier);
        }

        protected virtual void OnClick()
        {
            LuyenDanPanel.Instance.Alchermy.SetCurrentSelected(this);
        }

        private void ApplyQualityTheme(Tier tier)
        {
            var quality = LuyenDanPanel.Instance.Resources.Qualities.First(q => q.Tier == tier);
            qualityBackground.sprite = quality.Background;
            qualityIconUp.sprite     = quality.IconUp;
            itemNameText.color       = quality.Color;
        }
    }
}