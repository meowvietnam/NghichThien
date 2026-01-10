using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class ColorList : MonoBehaviour
    {
        [SerializeField] private ColorItem colorItemPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private GameObject thisObject;
        [SerializeField] private ToggleGroup toggleGroup;
        private List<ColorItem> colorItems = new List<ColorItem>();
        public void Init(List<Color> colors, Action<Color> onSelectColor)
        {
            for (int i = 0; i < colors.Count; i++)
            {
                var color = colors[i];
                var colorItem = Instantiate(colorItemPrefab, content);
                colorItem.Init(color, toggleGroup, onSelectColor);
                colorItems.Add(colorItem);
            }
        }
        public void SetActive(bool isActive)
        {
            thisObject.SetActive(isActive);
        }
        public void SetSelect(Color color)
        {
            foreach (var item in colorItems)
            {
                if (Vector4.Distance(item.GetColor(),color) < 0.001f)
                {
                    item.SetIsOnWithoutNotify();
                    break;
                }
            }
        }
        public bool HasColorPallet()
        {
            return colorItems.Count > 0;
        }
        public Color SelectFirst()
        {
            return colorItems[0].SetIsOnWithoutNotify();
        }
        public void SelectColor(Color color)
        {
            foreach (var item in colorItems)
            {
                if(Vector4.Distance(item.GetColor(),color) < 0.001f)
                {
                    item.SetIsOnWithoutNotify();
                    break;
                }   
            }
        }
    }
}