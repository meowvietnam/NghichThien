using CreateCharacter;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class FilterBase : BaseBehaviour
    {
        [SerializeField] private Text currentSelect;
        [SerializeField] public GameObject select;
        [SerializeField] public Button selectButton;
        [SerializeField] private RectTransform direct;
        [SerializeField] public ItemListUI itemListUI;
        private ItemBagFilter currentFilter;
        private bool isOpen;

        private void Start()
        {
            SetEventButton(selectButton, OnClick);
        }

        private void OnClick()
        {
            if (!isOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        private void Open()
        {
            isOpen = true;
            select.SetActive(true);
            direct.eulerAngles = new Vector3(0, 0, 180);
        }

        private void Close()
        {
            isOpen = false;
            select.SetActive(false);
            direct.eulerAngles = Vector3.zero;
        }

        public void OnFilter(ItemBagFilter itemBagFilter)
        {
            if (currentFilter != null)
            {
                itemListUI.RemoveFilter(currentFilter);
            }

            currentFilter = itemBagFilter;
            currentSelect.text = itemBagFilter.FilterName;
            itemListUI.AddFilter(currentFilter);
            itemListUI.Refresh();
            Close();
        }

        private void OnDestroy()
        {
            selectButton.onClick.RemoveAllListeners();
        }
    }
}