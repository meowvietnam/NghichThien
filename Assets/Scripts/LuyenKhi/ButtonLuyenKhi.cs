using System;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class ButtonLuyenKhi : BaseBehaviour
    {
        [SerializeField] private Sprite active;
        [SerializeField] private Sprite textActive;
        [SerializeField] private Sprite inactive;
        [SerializeField] private Sprite textInactive;
        [SerializeField] private Image image;
        [SerializeField] private Image imageText;
        [SerializeField] private Button button;

        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        private void OnClick()
        {
            LuyenKhiPanel.Instance.LuyenKhiPage.LuyenKhi();
        }

        public void SetInteractable(bool interactable)
        {
            button.interactable = interactable;
            image.sprite = interactable ? active : inactive;
            imageText.sprite = interactable ? textActive : textInactive;
        }
    }
}