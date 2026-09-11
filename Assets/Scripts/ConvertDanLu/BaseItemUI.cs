using Base;
using LuyenDan;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Base UI component for displaying a đan lô (furnace) or đan phương (recipe) item slot.
    /// Mirrors ConvertElementalSkill.BaseItemUI adapted for IDanItemData / Quality.
    /// </summary>
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

        public DanItemData DanData { get; private set; }

        [Button]
        public virtual void GetReference()
        {
            selfObject    = gameObject;
            selectButton  = selfObject.GetComponent<Button>();
            emptySlot     = transform.Find("Null").gameObject;
            filledSlot    = transform.Find("HasItem").gameObject;
            itemIcon          = transform.Find("HasItem/IconMask/Icon").GetComponent<Image>();
            qualityBackground = transform.Find("HasItem/Quality").GetComponent<Image>();
            qualityIconUp     = transform.Find("HasItem/Quality/QualityUp").GetComponent<Image>();
            countText         = transform.Find("HasItem/CountText").GetComponent<Text>();
            itemNameText      = transform.Find("HasItem/NameMask/NameText").GetComponent<Text>();
            hoverHighlight    = transform.Find("Selected").gameObject;
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

        private void SetVisual(IDanItemData data)
        {
            bool hasItem = data != null;
            if (emptySlot  != null) emptySlot.SetActive(!hasItem);
            if (filledSlot != null) filledSlot.SetActive(hasItem);
            if (!hasItem) return;

            if (itemIcon     != null) itemIcon.sprite     = data.Icon;
            if (itemNameText != null) itemNameText.text   = data.Name;

            var q = data.VisualQuality;
            if (qualityBackground != null) qualityBackground.sprite = q.Background;
            if (qualityIconUp     != null) qualityIconUp.sprite     = q.IconUp;
            if (itemNameText      != null) itemNameText.color        = q.Color;
        }

        private void OnCountChanged(int count)
        {
            SetCount(count);
            SetActive(count > 0);
        }

        protected virtual void Start()
        {
            if (selectButton != null)
            {
                SetEventButton(selectButton, OnSelected);
            }

            if (infoButton != null)
            {
                SetEventButton(infoButton, OnShowInfo);
            }
        }

        public void SetData(DanItemData danItemData)
        {
            if (DanData != null)
            {
                DanData.ItemRef.OnCountChanged -= OnCountChanged;
            }

            DanData = danItemData;
            if (danItemData != null)
            {
                SetVisual(danItemData.ItemData);
                var itemRef = danItemData.ItemRef;
                SetCount(itemRef.Count);
                itemRef.OnCountChanged += OnCountChanged;
            }
            else
            {
                SetVisual(null);
            }
        }

        protected virtual void OnSelected()
        {
        }

        protected virtual void OnShowInfo()
        {
        }

        protected virtual void OnDestroy()
        {
            if (selectButton != null)
                selectButton.onClick.RemoveAllListeners();

            if (infoButton != null)
                infoButton.onClick.RemoveAllListeners();

            if (DanData != null)
                DanData.ItemRef.OnCountChanged -= OnCountChanged;
        }

        public void DestroySelf()
        {
            Destroy(selfObject);
        }
    }
}
