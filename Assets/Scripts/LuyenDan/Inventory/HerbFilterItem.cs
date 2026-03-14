using System;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public abstract class HerbFilterItem : FilterItem
    {
        [SerializeField] private Button btn;
        private void Awake()
        {
            btn.onClick.AddListener(OnClick);
        }

        protected abstract void OnClick();

        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }

        public override void ForceSelect()
        {
        }
    }
}