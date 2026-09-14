using System;
using Combat;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;

namespace Clan
{
    public class ClanInheritanceSlotElement : MonoBehaviour
    {
        private const float EffectIconSize = 28f;

        [SerializeField] private Button selectButton;
        [SerializeField] private Text slotText;
        [SerializeField] private Text effectText;
        [SerializeField] private Text valueText;
        [SerializeField] private Image effectIcon;
        [SerializeField] private GameObject lockedRoot;
        [SerializeField] private Text lockedText;
        [SerializeField] private BuffAndDeBuffIconResources iconResources;
        public ClanInheritanceTrigger Trigger { get; private set; }
        public int SlotIndex { get; private set; }
        public ClanInheritanceEffect Effect { get; private set; }
        public bool IsUnlocked { get; private set; }

        public event Action<ClanInheritanceSlotElement> Clicked;

        private void Awake()
        {
            if (selectButton == null) selectButton = GetComponent<Button>();
            selectButton?.onClick.AddListener(OnClicked);
        }

        public void Init(
            ClanInheritanceTrigger trigger,
            int slotIndex,
            ClanInheritanceEffect effect,
            int displayValue,
            bool unlocked,
            int unlockLevel)
        {
            Trigger = trigger;
            SlotIndex = slotIndex;
            Effect = effect;
            IsUnlocked = unlocked;

            if (slotText != null) slotText.text = $"Ô {slotIndex + 1}";
            if (effectText != null)
                effectText.text = effect == null ? "Chưa chọn truyền thừa" : ClanInheritancePanel.GetEffectName(effect.EffectType);
            SetEffectIcon(effect);
            if (valueText != null)
            {
                valueText.text = effect == null ? "+—" : $"+{displayValue:n0}";
                valueText.gameObject.SetActive(unlocked);
            }
            if (lockedRoot != null) lockedRoot.SetActive(!unlocked);
            if (lockedText != null) lockedText.text = $"Mở ở cấp {unlockLevel}";
            if (selectButton != null) selectButton.interactable = unlocked;
        }

        private void OnClicked()
        {
            if (IsUnlocked) Clicked?.Invoke(this);
        }

        private void SetEffectIcon(ClanInheritanceEffect effect)
        {
            Image icon = GetOrCreateEffectIcon();
            if (icon == null)
            {
                return;
            }

            Sprite sprite = effect == null || iconResources == null ? null : iconResources.GetSprite(effect.EffectType);
            icon.sprite = sprite;
            icon.gameObject.SetActive(sprite != null);
        }

        private Image GetOrCreateEffectIcon()
        {
            if (effectIcon == null)
            {
                Transform parent = effectText != null ? effectText.transform.parent : transform;
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
