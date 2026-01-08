using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    public class ToggleGroupSkinCategoriesContainer : MonoBehaviour
    {
        [SerializeField] private ToggleGroupSkinCategories toggleGroupSkinCategoriesPrefab;
        [SerializeField] private Transform content;
        private List<ToggleGroupSkinCategories> toggleGroups = new List<ToggleGroupSkinCategories>();
        [SerializeField] private ItemListGroupContainer itemListGroupContainer;
        public void Init(List<SkinGroup> skinGroups)
        {
            foreach (var skinGroup in skinGroups)
            {
                var t = Instantiate(toggleGroupSkinCategoriesPrefab, content);
                t.Init(skinGroup, itemListGroupContainer);
                toggleGroups.Add(t);
            }
        }
        public void Active(string id)
        {
            foreach (var item in toggleGroups)
            {
                var active = item.Id == id;
                item.SetActive(active);
            }
        }
    }
}
