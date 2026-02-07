using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [field: SerializeField] public ItemListUI ItemListUI { get; private set; }
        [SerializeField] private FilterGroup filterGroup;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject thisObject;
        public void Init()
        {
            closeButton.onClick.AddListener(Close);
            ItemListUI.Init(new List<IInventoryItemData>());
            filterGroup.Init();
        }
        
        private void Close()
        {
            thisObject.SetActive(false);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(Close);
        }
    }
}