using Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    public class GenderSkinGroupUI : MonoBehaviour
    {
        [SerializeField] private ToggleGroupSkinGroups toggleGroupSkinGroups;
        [SerializeField] private ToggleGroupSkinCategoriesContainer toggleGroupSkinCategoriesContainer;
        [SerializeField] private ItemListGroupContainer itemListGroupContainer;
        [SerializeField] private GameObject thisObject;
        public Dictionary<string, string> Skin { get; private set; } = new();
        public Dictionary<string, Color> Colors { get; private set; } = new();
        public Gender Gender { get; private set; }
        public bool IsUsedToSelect { get; private set; }

        public void Init(GenderSkinGroup genderSkinGroup, List<Color> pallete)
        {
            Gender = genderSkinGroup.Gender;
            toggleGroupSkinGroups.Init(genderSkinGroup.SkinGroups);
            toggleGroupSkinCategoriesContainer.Init(genderSkinGroup.SkinGroups);
            itemListGroupContainer.Init(genderSkinGroup.SkinGroups, pallete);
        }

        public void SetSkin(string key, string value)
        {
            Skin[key] = value;
        }

        public void SetColor(string key, Color value)
        {
            Colors[key] = value;
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }

        private bool IsSame(Color a, Color b, float tolerance = 0.001f)
        {
            return Mathf.Abs(a.r - b.r) < tolerance &&
                   Mathf.Abs(a.g - b.g) < tolerance &&
                   Mathf.Abs(a.b - b.b) < tolerance &&
                   Mathf.Abs(a.a - b.a) < tolerance;
        }

        public void RandomSelector()
        {
            itemListGroupContainer.RandomSelector();
        }

        public void DefaultSelector()
        {
            IsUsedToSelect = true;
            itemListGroupContainer.DefaultSelector();
        }
    }
}