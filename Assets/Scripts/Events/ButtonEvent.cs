using System;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class ButtonEvent : BaseBehaviour
    {
        [field: SerializeField] public ushort EventId { get; private set; }
        [SerializeField] private Button button;
        protected RectTransform containerPanel;

        public void Init(RectTransform containerPanel)
        {
            this.containerPanel = containerPanel;
        }

        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        protected virtual void OnClick()
        {
        }
    }
}