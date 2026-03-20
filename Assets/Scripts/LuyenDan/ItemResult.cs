using System.Linq;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenDan
{
    public class ItemResult : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject thisObject;

        [TabGroup("Null Item")] [SerializeField]
        private GameObject nullItem;

        [TabGroup("Has Item")] [SerializeField]
        private GameObject hasItem;

        [TabGroup("Has Item")] [SerializeField]
        private Image icon;

        [TabGroup("Has Item")] [SerializeField]
        private Image background;

        [TabGroup("Has Item")] [SerializeField]
        private Image iconUp;

        [TabGroup("Has Item")] [SerializeField]
        private Text countText;

        [TabGroup("Has Item")] [SerializeField]
        private Text nameText;

        [TabGroup("Has Item")] [SerializeField]
        private GameObject hover;

        protected virtual void Init(IInventoryItemData data)
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
        public virtual void Init(ItemRef itemRef)
        {
            SetCount(itemRef.Count);
            var data = LuyenDanPanel.Instance.Resources.FindItem(itemRef);
            Init(data);
        }
        private void SetCount(int count)
        {
            countText.text = count > 1 ? count.ToString() : "";
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
    }
}