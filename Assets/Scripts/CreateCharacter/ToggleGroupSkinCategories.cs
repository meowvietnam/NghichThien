using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class ToggleGroupSkinCategories : MonoBehaviour
    {
        [SerializeField] private ToggleSkinCategory toggleSkinCategoryPrefab;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private Transform content;
        [SerializeField] private GameObject thisObject;
        private List<ToggleSkinCategory> toggleSkinCategories = new List<ToggleSkinCategory>();
        public string Id { get; private set; }
        private ItemListGroupContainer itemListGroupContainer;

        public void Init(SkinGroup skinGroup, ItemListGroupContainer itemListGroupContainer)
        {
            Id = skinGroup.Id;
            this.itemListGroupContainer = itemListGroupContainer;
            foreach (var skinCategory in skinGroup.Categories)
            {
                var toggleSkinCategory = Instantiate(toggleSkinCategoryPrefab, content);
                toggleSkinCategory.Init(skinCategory, toggleGroup, OnToggleOn);
                toggleSkinCategories.Add(toggleSkinCategory);
            }
        }

        public void SetActive(bool isActive)
        {
            thisObject.SetActive(isActive);
        }

        private void OnToggleOn(ToggleSkinCategory toggleCategory)
        {
            Active(toggleCategory.Id);
        }

        public void Active(string id)
        {
            itemListGroupContainer.ActiveItemList(id);
        }

        public void ActiveFirst()
        {
            //toggleSkinCategories[0].ToggleOn();
            //Debug.Log(toggleSkinCategories[0].SelfId);
        }
    }
}