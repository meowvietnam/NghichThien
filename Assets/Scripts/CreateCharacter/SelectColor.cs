using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class SelectColor : MonoBehaviour
    {
        [SerializeField] private Button selectColor;
        [SerializeField] private Image colorImage;
        [SerializeField] private ColorList colorList;
        [SerializeField] private GameObject thisObject;
        private ColorGroup colorGroup;
        public void Init(List<Color> pallete, ColorGroup colorGroup)
        {
            colorGroup.OnChanged += OnColorChange;
            this.colorGroup = colorGroup;
            selectColor.onClick.AddListener(ShowColorList);
            colorList.Init(pallete, OnSelectColor);
        }
        private void ShowColorList()
        {
            colorList.SetActive(true);
            selectColor.gameObject.SetActive(false);
        }
        private void HideColorList()
        {
            colorList.SetActive(false);
            selectColor.gameObject.SetActive(true);
        }
        public void OnSelectColor(Color color)
        {
            HideColorList();
            colorGroup.OnChanged?.Invoke(color);
            foreach (var item in colorGroup.CategoryIds)
            {
                SetPlayerAvatar.Instance.SetColor(item, color);   
            }
            SetPlayerAvatar.Instance.UpdateAvatar();
        }
        private void OnColorChange(Color color)
        {
            var currentColor = colorImage.color;
            if (Vector4.Distance(currentColor, color) < 0.001f)
            {
                return;
            }
            SetColor(color);
        }
        private void SetColor(Color color)
        {
            colorImage.color = color;
            colorList.SetSelect(color);
        }
        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }
        private void OnDestroy()
        {
            selectColor.onClick.RemoveAllListeners();
            if (colorGroup != null)
            {
                colorGroup.OnChanged -= OnColorChange;
            }
        }
        public void SelectFirst()
        {
            if(colorList.HasColorPallet())
            {
                var color = colorList.SelectFirst();
                colorImage.color = color;
                foreach (var item in colorGroup.CategoryIds)
                {
                    SetPlayerAvatar.Instance.SetColor(item, color);
                }
            }
        }
    }
}
