using System;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class ColorItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image image;

        public void Init(Color color, ToggleGroup toggleGroup, System.Action<Color> onSelectColor)
        {
            toggle.group = toggleGroup;
            SetColor(color);
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    onSelectColor?.Invoke(color);
                }
            });
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveAllListeners();
        }

        private void SetColor(Color color)
        {
            image.color = color;
        }

        public Color GetColor()
        {
            return image.color;
        }

        public void ToggleOn()
        {
            toggle.isOn = true;
        }

        public Color SetIsOnWithoutNotify()
        {
            toggle.SetIsOnWithoutNotify(true);
            return image.color;
        }
    }
}