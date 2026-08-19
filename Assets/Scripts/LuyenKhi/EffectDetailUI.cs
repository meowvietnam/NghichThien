using CreateCharacter.CharStats;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class EffectDetailUI : MonoBehaviour , IPointerDownHandler , IPointerExitHandler
    {
        [SerializeField] private Text text;
        public Action<PointerEventData> actionPointerDown;
        public void OnPointerDown(PointerEventData eventData)
        {
            actionPointerDown?.Invoke(eventData);
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.Instance?.SetActive(false);
        }
    }
}