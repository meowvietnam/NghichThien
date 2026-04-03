using System;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class MailFilter : BaseBehaviour
    {
        [field: SerializeField] public MailFilterType Type { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [SerializeField] private Button filterButton;
        public event Action<MailFilter> OnFilter;

        private void Start()
        {
            this.SetEventButton(filterButton, OnClick);
        }

        private void OnClick()
        {
            OnFilter?.Invoke(this);
        }
        private void OnDestroy()
        {
            filterButton.onClick.RemoveAllListeners();
        }
    }
}