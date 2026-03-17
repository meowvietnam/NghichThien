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
        private void Start()
        {
            closeButton.onClick.AddListener(Close);
        }
        public void Init(IEnumerable<ItemRef> data)
        {
            ItemListUI.Init(data);
            foreach (var filterGroup in filterGroups)
            {
                filterGroup.Init();
            }
        }
        public void Close()
        {
            thisObject.SetActive(false);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(Close);
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }
    }
}