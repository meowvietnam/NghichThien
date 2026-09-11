using System.Linq;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenDan
{
    public class ItemResult : BaseItemUI
    {
        [SerializeField] private Button showInfoButton;
        private IItemData itemData;
        private ItemRef itemRef;

        protected virtual void Init(IItemData data)
        {
            SetEventButton(showInfoButton, ShowInfo);
            itemData = data;
            SetData(data);
        }

        private void ShowInfo()
        {
            ToolTipItems.Instance.ShowInfoItem(itemData, itemRef);
        }

        public virtual void Init(ItemRef itemRef)
        {
            SetCount(itemRef.Count);
            var data = LuyenDanPanel.Instance.Resources.FindItem(itemRef);
            this.itemRef = itemRef;
            Init(data);
        }

        public void DestroySelf()
        {
            Destroy(selfObject);
        }
    }
}