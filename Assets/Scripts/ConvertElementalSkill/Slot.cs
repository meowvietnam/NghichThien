using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class Slot : BaseItemUI, ISelectedItemCallback
    {
        [SerializeField] private Button clearButton;

        protected override void Start()
        {
            base.Start();
            SetEventButton(clearButton, Clear);
        }

        public void Clear()
        {
            ClearItem();
            ConvertElementalSkillPanel.Instance.Page.PutItems.OnDeselect(this);
            ClearNew();
        }

        private void ClearItem()
        {
            if (SkillData != null)
            {
                var instance = ConvertElementalSkillPanel.Instance;
                var bag = instance.Bag;
                var myItem = SkillData.ItemRef;
                var itemBag = bag.FindItem(myItem.Id);
                var itemRef = itemBag.SkillData.ItemRef;
                itemRef.SetCount(itemRef.Count + myItem.Count);
                var itemList = bag.ItemList;
                itemList.ApplyFilterOn(itemBag);
            }
        }

        public void ClearNew()
        {
            SetData(null);
            clearButton.gameObject.SetActive(false);
        }

        protected override void OnSelected()
        {
            var instance = ConvertElementalSkillPanel.Instance;
            instance.Bag.OpenWithCallback(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            clearButton.onClick.RemoveAllListeners();
        }

        public void OnSelected(ItemBag item)
        {
            ClearItem();
            var instance = ConvertElementalSkillPanel.Instance;
            var itemRef = item.SkillData.ItemRef;
            var itemRefClone = itemRef.DeepClone();
            itemRefClone.Count = 1;
            itemRef.SetCount(itemRef.Count - itemRefClone.Count);
            var skillData = new SkillData(itemRefClone, item.SkillData.ItemData);
            SetData(skillData);
            var itemList = instance.Bag.ItemList;
            itemList.ApplyFilterOn(item);
            instance.Page.PutItems.OnSelect(this);
            clearButton.gameObject.SetActive(true);
        }
    }
}