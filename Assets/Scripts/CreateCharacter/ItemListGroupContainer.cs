using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    public class ItemListGroupContainer : MonoBehaviour
    {
        [SerializeField] private ItemListContainer itemListContainerPrefab;
        [SerializeField] private Transform content;
        private List<ItemListContainer> itemListContainers = new List<ItemListContainer>();
        private ItemListContainer currentActive;
        public void Init(List<SkinGroup> skinGroups,List<Color> pallete)
        {
            foreach (var skinGroup in skinGroups)
            {
                var t = Instantiate(itemListContainerPrefab, content);
                t.Init(skinGroup, pallete);
                itemListContainers.Add(t);
            }
        }
        public void Active(string id)
        {
            foreach (var item in itemListContainers)
            {
                var isActive = item.Id == id;
                item.SetActive(isActive);
                if (isActive)
                {
                    currentActive = item;
                    currentActive.ActiveCurrent();
                }
            }
        }
        public void ActiveItemList(string id)
        {
            currentActive.Active(id);
        }
        public void RandomSelector()
        {
            foreach (var itemList in itemListContainers)
            {
                itemList.RandomSelector();
            }
        }
        public void SelectFirst()
        {
            foreach (var item in itemListContainers)
            {
                item.SelectFirst();
            }
        }
    }
}
