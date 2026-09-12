using System;
using Network.Models.Clans;
using UnityEngine;

namespace Clan
{
    public class ClanInheritanceOptionElement : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button selectButton;
        [SerializeField] private UnityEngine.UI.Text nameText;
        [SerializeField] private UnityEngine.UI.Text valueText;
        [SerializeField] private GameObject selectedRoot;

        public EBuffAndDeBuff BuffType { get; private set; }
        public event Action<ClanInheritanceOptionElement> Clicked;

        private void Awake()
        {
            if (selectButton == null) selectButton = GetComponent<UnityEngine.UI.Button>();
            selectButton?.onClick.AddListener(OnClicked);
        }

        public void Init(EBuffAndDeBuff buffType, int value, bool selected)
        {
            BuffType = buffType;
            if (nameText != null) nameText.text = ClanInheritancePanel.GetEffectName(buffType);
            if (valueText != null) valueText.text = $"+{value:n0}";
            if (selectedRoot != null) selectedRoot.SetActive(selected);
            if (selectButton != null) selectButton.interactable = !selected;
        }

        private void OnClicked() => Clicked?.Invoke(this);

        private void OnDestroy()
        {
            selectButton?.onClick.RemoveListener(OnClicked);
            Clicked = null;
        }
    }
}
