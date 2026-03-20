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
        protected virtual void Init(IItemData data)
        {
            SetData(data);
        }
        public virtual void Init(ItemRef itemRef)
        {
            SetCount(itemRef.Count);
            var data = LuyenDanPanel.Instance.Resources.FindItem(itemRef);
            Init(data);
        }

        public void DestroySelf()
        {
            Destroy(selfObject);
        }
    }
}