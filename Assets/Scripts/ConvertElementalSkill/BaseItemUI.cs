using Base;
using LuyenDan;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class BaseItemUI : BaseBehaviour
    {
        [SerializeField, TabGroup("Has Item")] private Button selectButton;
        [SerializeField, TabGroup("Has Item")] private Button infoButton;
        [SerializeField] private GameObject selfObject;

        [TabGroup("Null Item")] [SerializeField]
        private GameObject emptySlot;

        [TabGroup("Has Item")] [SerializeField]
        private GameObject filledSlot;

        [TabGroup("Has Item")] [SerializeField]
        private Image itemIcon;

        [TabGroup("Has Item")] [SerializeField]
        private Image qualityBackground;

        [TabGroup("Has Item")] [SerializeField]
        private Image qualityIconUp;

        [TabGroup("Has Item")] [SerializeField]
        private Text countText;

        [TabGroup("Has Item")] [SerializeField]
        private Text itemNameText;

        [TabGroup("Has Item")] [SerializeField]
        private GameObject hoverHighlight;

        public SkillData SkillData { get; private set; }

        [Button]
        public virtual void GetReference()
        {
            selfObject = gameObject;
            selectButton = selfObject.GetComponent<Button>();
            emptySlot = transform.Find("Null").gameObject;
            filledSlot = transform.Find("HasItem").gameObject;
            itemIcon = transform.Find("HasItem/IconMask/Icon").GetComponent<Image>();
            qualityBackground = transform.Find("HasItem/Quality").GetComponent<Image>();
            qualityIconUp = transform.Find("HasItem/Quality/QualityUp").GetComponent<Image>();
            countText = transform.Find("HasItem/CountText").GetComponent<Text>();
            itemNameText = transform.Find("HasItem/NameMask/NameText").GetComponent<Text>();
            hoverHighlight = transform.Find("Selected").gameObject;
        }

        public void SetActive(bool active)
        {
            if (selfObject != null)
                selfObject.SetActive(active);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverHighlight != null)
                hoverHighlight.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverHighlight != null)
                hoverHighlight.SetActive(false);
        }

        public void SetCount(int count)
        {
            if (countText != null)
            {
                countText.text = count > 1 ? Utils.FormatCount(count) : "";
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

        private void OnCountChanged(int count)
        {
            SetCount(count);
            SetActive(count > 0);
        }

        protected virtual void Start()
        {
            if (selfObject != null)
            {
                SetEventButton(selectButton, OnSelected);
            }

            if (infoButton != null)
            {
                SetEventButton(infoButton, OnShowInfo);
            }
        }

        public virtual void SetData(IItemData itemData, ItemRef itemRef)
        {
            bool hasItem = itemData != null;
            if (emptySlot != null) emptySlot.SetActive(!hasItem);
            if (filledSlot != null) filledSlot.SetActive(hasItem);
            if (!hasItem) return;
            if (itemIcon != null) itemIcon.sprite = itemData.Icon;
            if (itemNameText != null) itemNameText.text = itemData.Name;
            SetCount(itemRef.Count);
            var quaility = CoreConfig.Instance.FindQuality(itemData.Tier);
            SetQuality(quaility);
        }

        private void SetQuality(Quality quality)
        {
            if (qualityBackground != null) qualityBackground.sprite = quality.Background;
            if (qualityIconUp != null) qualityIconUp.sprite = quality.IconUp;
            if (itemNameText != null) itemNameText.color = quality.Color;
        }

        public void SetData(SkillData skillData)
        {
            if (SkillData != null)
            {
                SkillData.ItemRef.OnCountChanged -= OnCountChanged;
            }

            SkillData = skillData;
            if (skillData != null)
            {
                SetSkillData(skillData.ItemData);
                var itemRef = skillData.ItemRef;
                SetCount(itemRef.Count);
                itemRef.OnCountChanged += OnCountChanged;
            }
            else
            {
                SetSkillData(null);
            }
        }

        protected virtual void OnSelected()
        {
        }

        protected virtual void OnShowInfo()
        {
            ConvertElementalSkillPanel.Instance.ToolTipItems.ShowInfoSkill(SkillData.ItemData);
        }

        protected virtual void OnDestroy()
        {
            if (selfObject != null)
            {
                selectButton.onClick.RemoveAllListeners();
            }

            if (infoButton != null)
            {
                infoButton.onClick.RemoveAllListeners();
            }

            if (SkillData != null)
            {
                SkillData.ItemRef.OnCountChanged -= OnCountChanged;
            }
        }

        public void DestroySelf()
        {
            Destroy(selfObject);
        }
    }
}