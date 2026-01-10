using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    public class ItemListContainer : MonoBehaviour
    {
        [SerializeField] private ItemList itemListPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private GameObject thisObject;
        private List<ItemList> itemLists = new List<ItemList>();
        public string Id { get; private set; }
        private ItemList current;
        public void Init(SkinGroup skinGroup, List<Color> pallete)
        {
            Id = skinGroup.Id;
            foreach (var category in skinGroup.Categories)
            {
                var itemList = Instantiate(itemListPrefab, content);
                ColorGroup cg = null;
                if(skinGroup.ColorGroups != null) 
                {
                    foreach(var colorGroup in skinGroup.ColorGroups) 
                    {
                        if(colorGroup.CategoryIds.Contains(category.Id)) 
                        {
                            cg = colorGroup;
                            break;
                        }
                    }
                }
                itemList.Init(category, pallete, cg);
                itemLists.Add(itemList);
            }
        }
        public void SetActive(bool isActive)
        {
            thisObject.SetActive(isActive);
        }
        public void Active(string id)
        {
            foreach (var item in itemLists)
            {
                var active = item.Id == id;
                item.SetActive(active);
                if (active)
                {
                    current = item;
                }
            }
        }
        public void ActiveCurrent() 
        {
            if(current == null) 
            {
                current = itemLists[0];
            }
            itemLists.ForEach(item => item.SetActive(item == current));
        }
        public void RandomSelector()
        {
            foreach (var item in itemLists)
            {
                item.RandomSelector();
            }
        }
        public void DefaultSelector()
        {
            foreach (var item in itemLists)
            {
                item.DefaultSelector();
            }
        }
    }
}