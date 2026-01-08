using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class ItemList : MonoBehaviour
    {
        [SerializeField] private Item itemPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private GameObject thisObject;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private SelectColor selectColor;
        private List<Item> items = new List<Item>();
        public string Id { get; private set; }
        public void Init(SkinCategory category, List<Color> pallete, ColorGroup colorGroup)
        {
            if (colorGroup != null)
            {
                selectColor.Init(pallete, colorGroup);
                selectColor.SetActive(true);
            }
            else
            {
                selectColor.SetActive(false);
            }
            Id = category.Id;
            var skins = category.Skins;
            foreach (var skin in skins)
            {
                var item = Instantiate(itemPrefab, content);
                item.Init(skin.Name, skin.Icon, toggleGroup, OnToggleItem);
                items.Add(item);
            }
        }
        private void OnToggleItem(Item item)
        {
            var itemName = item.SkinName;
            SetPlayerAvatar.Instance.SetSkinImmediate(Id, itemName);
        }
        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }
        public void RandomSelector()
        {
            var index = UnityEngine.Random.Range(0, items.Count);
            var item = items[index];
            item.ToggleOnWithoutNotify();
            SetPlayerAvatar.Instance.SetSkin(Id,item.SkinName);
        }
        public void SelectFirst()
        {
            var item = items[0];
            item.ToggleOnWithoutNotify();
            SetPlayerAvatar.Instance.SetSkin(Id,item.SkinName);
            selectColor.SelectFirst();
        }
    }
}