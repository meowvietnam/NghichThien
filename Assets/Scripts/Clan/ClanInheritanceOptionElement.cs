using System;
using Combat;
using LuyenKhi;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;

namespace Clan
{
    public class ClanInheritanceOptionElement : MonoBehaviour
    {
        private const float EffectIconSize = 28f;

        [SerializeField] private Button selectButton;
        [SerializeField] private Text nameText;
        [SerializeField] private Text valueText;
        [SerializeField] private Image effectIcon;
        [SerializeField] private GameObject selectedRoot;
        [SerializeField] private BuffAndDeBuffIconResources iconResources;
        public EffectType EffectType { get; private set; }
        public event Action<ClanInheritanceOptionElement> Clicked;

        private void Awake()
        {
            if (selectButton == null) selectButton = GetComponent<Button>();
            selectButton?.onClick.AddListener(OnClicked);
        }

        public void Init(EffectType effectType, int minValue, int maxValue, bool selected)
        {
            EffectType = effectType;
            if (nameText != null) nameText.text = ClanInheritancePanel.GetEffectName(effectType);
            SetEffectIcon(effectType);
            if (valueText != null) valueText.text = minValue == maxValue
                ? $"+{minValue:n0}"
                : $"+{minValue:n0}-{maxValue:n0}";
            if (selectedRoot != null) selectedRoot.SetActive(selected);
            if (selectButton != null) selectButton.interactable = !selected;
        }

        private void OnClicked() => Clicked?.Invoke(this);

        private void SetEffectIcon(EffectType effectType)
        {
            Image icon = GetOrCreateEffectIcon();
            if (icon == null)
            {
                return;
            }

            Sprite sprite = iconResources == null ? null : iconResources.GetSprite(effectType);
            icon.sprite = sprite;
            icon.gameObject.SetActive(sprite != null);
        }

        private Image GetOrCreateEffectIcon()
        {
            if (effectIcon == null)
            {
                Transform parent = nameText != null ? nameText.transform.parent : transform;
                Transform child = parent.Find("EffectIcon");
                if (child != null)
                {
                    effectIcon = child.GetComponent<Image>();
                }

                if (effectIcon == null)
                {
                    GameObject iconObject = new GameObject("EffectIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    iconObject.transform.SetParent(parent, false);
                    iconObject.transform.SetAsFirstSibling();

                    RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2(0f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0f, 0.5f);
                    rectTransform.pivot = new Vector2(0f, 0.5f);
                    rectTransform.anchoredPosition = new Vector2(8f, 0f);
                    rectTransform.sizeDelta = new Vector2(EffectIconSize, EffectIconSize);

                    effectIcon = iconObject.GetComponent<Image>();
                    effectIcon.preserveAspect = true;
                    effectIcon.raycastTarget = false;
                    iconObject.AddComponent<LayoutElement>().ignoreLayout = true;
                    effectIcon.gameObject.SetActive(false);
                }
            }

            return effectIcon;
        }

        private void OnDestroy()
        {
            selectButton?.onClick.RemoveListener(OnClicked);
            Clicked = null;
        }
    }
}
