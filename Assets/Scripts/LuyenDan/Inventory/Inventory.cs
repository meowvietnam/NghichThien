using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [field: SerializeField] public ItemListUI ItemListUI { get; private set; }
        [SerializeField] private List<FilterGroup> filterGroups;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject thisObject;
        
        public void Init(IEnumerable<ItemRef> data)
        {
            closeButton.onClick.AddListener(Close);
            ItemListUI.Init(data);
            foreach (var filterGroup in filterGroups)
            {
                filterGroup.Init();
            }
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