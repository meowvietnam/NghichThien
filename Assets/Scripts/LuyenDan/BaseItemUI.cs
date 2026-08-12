using System.Linq;
using Base;
using ConvertElementalSkill;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LuyenDan
{
    public abstract class BaseItemUI : BaseBehaviour, IPointerEnterHandler, IPointerExitHandler
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
            var quality = CoreConfig.Instance.Qualities.First(q => q.Tier == tier);
            if (qualityBackground != null) qualityBackground.sprite = quality.Background;
            if (qualityIconUp != null) qualityIconUp.sprite = quality.IconUp;
            if (itemNameText != null) itemNameText.color = quality.Color;
        }

        public virtual void SetCount(int count)
        {
            if (countText != null)
            {
                countText.text = count > 1 ? Utils.FormatCount(count) : "";
            }
        }

        public virtual void SetData(IItemData data)
        {
            bool hasItem = data != null;
            if (emptySlot != null) emptySlot.SetActive(!hasItem);
            if (filledSlot != null) filledSlot.SetActive(hasItem);

            if (!hasItem) return;

            if (itemIcon != null) itemIcon.sprite = data.Icon;
            if (itemNameText != null) itemNameText.text = data.Name;
            
            SetTier(data.Tier);
        }
        public void SetSkill(SkillData skillData)
        {
            if (skillData != null)
            {
                SetSkillData(skillData.ItemData);
                var itemRef = skillData.ItemRef;
                SetCount(itemRef.Count);
            }
            else
            {
                SetSkillData(null);
            }
        }
        private void SetQuality(SkillVisualQuality quality)
        {
            if (qualityBackground != null) qualityBackground.sprite = quality.Frame;
            if (qualityIconUp != null) qualityIconUp.sprite = quality.QualityFull;
            if (itemNameText != null) itemNameText.color = quality.Color;
        }
        private void SetSkillData(ISkillItemData data)
        {
            bool hasItem = data != null;
            if (emptySlot != null) emptySlot.SetActive(!hasItem);
            if (filledSlot != null) filledSlot.SetActive(hasItem);
            if (!hasItem) return;
            if (itemIcon != null) itemIcon.sprite = data.Icon;
            if (itemNameText != null) itemNameText.text = data.Name;
            SetQuality(data.VisualQuality);
        }
    }
}
