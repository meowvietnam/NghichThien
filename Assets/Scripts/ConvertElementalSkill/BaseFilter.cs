using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public abstract class BaseFilter : BaseBehaviour, IFilter
    {
        [SerializeField] private Button button;
        [SerializeField] private string text;
        [SerializeField] private FilterGroup filterGroup;
        public string GetText()
        {
            return text;
        }
        public abstract bool Filter(ItemBag item);

        public void Init()
        {
            SetEventButton(button, OnClick);
        }

        private void OnClick()
        {
            filterGroup.SelectFilter(this);
            filterGroup.UpdateFilter();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}