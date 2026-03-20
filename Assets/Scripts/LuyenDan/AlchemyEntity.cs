using System.Linq;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenDan
{

    public class AlchemyEntity : BaseItemUI
    {
        [SerializeField] private Button clickButton;
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

        public virtual void OnLuyenDanCompleted()
        {
            
        }

        public virtual void Select(InventoryItemUI item)
        {
            Item = item;
        }

        protected virtual void OnClick()
        {
            LuyenDanPanel.Instance.Alchermy.SetCurrentSelected(this);
        }
    }
}