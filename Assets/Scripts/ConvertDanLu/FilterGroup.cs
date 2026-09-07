using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Groups filter buttons (e.g. tier selection) for the Bag scroll list.
    /// Mirrors ConvertElementalSkill.FilterGroup.
    /// </summary>
    public class FilterGroup : BaseBehaviour
    {
        [SerializeField] private Button openButton;
        [SerializeField] private Text filterText;
        [SerializeField] private RectTransform arrow;
        [SerializeField] private GameObject selectObject;
        [SerializeField] private string nameGroup;
        [SerializeField] private BaseFilter[] filters;

        private BaseFilter currentFilter;
        private ItemList itemList;
        private bool open;

        public void Init(ItemList list)
        {
            itemList = list;
            SetEventButton(openButton, OnOpenClick);
            foreach (var filter in filters)
            {
                filter.Init();
            }

            SelectFilter(null);
        }

        private void OnOpenClick()
        {
            open = !open;
            selectObject.SetActive(open);
            UpdateArrow();
        }

        private void UpdateArrow()
        {
            arrow.localEulerAngles = new Vector3(0, 0, open ? 180f : 0f);
        }

        public void SelectFilter(BaseFilter filter)
        {
            open = false;
            selectObject.SetActive(false);
            UpdateArrow();

            if (currentFilter != null)
                itemList.RemoveFilter(currentFilter);

            currentFilter = filter;

            if (currentFilter != null)
            {
                itemList.AddFilter(currentFilter);
                filterText.text = currentFilter.GetText();
            }
            else
            {
                filterText.text = nameGroup;
            }
        }

        public void UpdateFilter()
        {
            itemList.UpdateFilter();
        }

        private void OnDestroy()
        {
            openButton.onClick.RemoveAllListeners();
        }
    }
}
