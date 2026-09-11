using System;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image image;
        public string SkinName { get; private set; }

        public void Init(string skinName, Sprite icon, ToggleGroup toggleGroup, Action<Item> onToggle)
        {
            SkinName = skinName;
            image.sprite = icon;
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    onToggle?.Invoke(this);
                }
            });
            toggle.group = toggleGroup;
        }

        public void ToggleOnWithoutNotify()
        {
            toggle.SetIsOnWithoutNotify(true);
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }
}