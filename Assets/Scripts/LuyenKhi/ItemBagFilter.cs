using System;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public abstract class ItemBagFilter : BaseBehaviour, IItemFilter<ItemBag>
    {
        [SerializeField] private Button button;
        [SerializeField] private FilterBase filterBase;
        [field: SerializeField] public string FilterName { get; private set; }
        private void Start()
        {
            SetEventButton(button,OnSelect);
        }
        private void OnSelect()
        {
            filterBase.OnFilter(this);
        }
        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
        public abstract bool Filter(ItemBag item);
    }
}