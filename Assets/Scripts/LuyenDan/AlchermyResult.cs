using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class AlchermyResult : MonoBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private ItemResult itemPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button okBtn;
        private List<ItemResult> items = new List<ItemResult>();

        private void Start()
        {
            okBtn.onClick.AddListener(OnOk);
        }

        private void OnOk()
        {
            
        }
        private void OnDestroy()
        {
            okBtn.onClick.RemoveAllListeners();
        }

        public void SetDescription(string description)
        {
            descriptionText.text = description;
        }
        public void SetData(List<ItemRef> itemRefs)
        {
            foreach (var item in items)
            {
                item.DestroySelf();
            }
            items = new List<ItemResult>();
            foreach (var itemRef in itemRefs)
            {
                var item = Instantiate(itemPrefab, content);
                item.Init(itemRef);
                items.Add(item);
            }
        }
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
    }
}
