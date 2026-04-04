using System.Linq;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LuyenDan
{
    public abstract class BaseItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected GameObject selfObject;

        [TabGroup("Null Item")]
        [SerializeField] protected GameObject emptySlot;

        [TabGroup("Has Item")]
        [SerializeField] protected GameObject filledSlot;

        [TabGroup("Has Item")]
        [SerializeField] protected Image itemIcon;

        [TabGroup("Has Item")]
        [SerializeField] protected Image qualityBackground;

        [TabGroup("Has Item")]
        [SerializeField] protected Image qualityIconUp;

        [TabGroup("Has Item")]
        [SerializeField] protected Text countText;

        [TabGroup("Has Item")]
        [SerializeField] protected Text itemNameText;

        [TabGroup("Has Item")]
        [SerializeField] protected GameObject hoverHighlight;

        public virtual void SetActive(bool active)
        {
            if (selfObject != null)
                selfObject.SetActive(active);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverHighlight != null)
                hoverHighlight.SetActive(true);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (hoverHighlight != null)
                hoverHighlight.SetActive(false);
        }

        protected virtual void SetTier(Tier tier)
        {
            var quality = LuyenDanResources.Instance.Qualities.First(q => q.Tier == tier);
            if (qualityBackground != null) qualityBackground.sprite = quality.Background;
            if (qualityIconUp != null) qualityIconUp.sprite = quality.IconUp;
            if (itemNameText != null) itemNameText.color = quality.Color;
        }

        protected virtual void SetCount(int count)
        {
            if (countText != null)
            {
                countText.text = count > 1 ? count.ToString() : "";
            }
        }

        protected virtual void SetData(IItemData data)
        {
            bool hasItem = data != null;
            if (emptySlot != null) emptySlot.SetActive(!hasItem);
            if (filledSlot != null) filledSlot.SetActive(hasItem);

            if (!hasItem) return;

            if (itemIcon != null) itemIcon.sprite = data.Icon;
            if (itemNameText != null) itemNameText.text = data.Name;
            
            SetTier(data.Tier);
        }
    }
}
