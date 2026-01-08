using Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class SetPlayerAvatar : MonoBehaviour
    {
        [SerializeField] private PlayerAvatar avatar;
        [SerializeField] private GenderSkinGroupUI genderSkinGroupUIPrefab;
        [SerializeField] private Transform content;
        private List<GenderSkinGroupUI> genderSkinGroupUIs = new List<GenderSkinGroupUI>();
        public GenderSkinGroupUI currentSelect { get; private set; }
        public static SetPlayerAvatar Instance { get; private set; }
        [SerializeField] private Button randomBtn;
        public void Init(SkinResources skinResources)
        {
            Instance = this;
            randomBtn.onClick.AddListener(Random);
            var colors = skinResources.Colors;
            var genderSkinGroups = skinResources.GenderSkinGroups;
            foreach (var genderSkinGroup in genderSkinGroups)
            {
                var genderSkinGroupUI = Instantiate(genderSkinGroupUIPrefab, content);
                genderSkinGroupUI.Init(genderSkinGroup, colors);
                genderSkinGroupUIs.Add(genderSkinGroupUI);
            }
        }
        public void SetGender(Gender gender)
        {
            foreach (var item in genderSkinGroupUIs)
            {
                var active = item.Gender == gender;
                item.SetActive(active);
                if(active)
                {
                    currentSelect = item;
                }
            }
            avatar.SetGender(gender);
            if(!currentSelect.IsUsedToSelect)
            {
                currentSelect.SelectFirst();
            }
            UpdateAvatar();
        }
        private void Random()
        {
            currentSelect.RandomSelector();
            UpdateAvatar();
        }
        public void SetSkin(string key,string value)
        {
            currentSelect.SetSkin(key, value);
        }
        public void SetSkinImmediate(string key,string value)
        {
            currentSelect.SetSkin(key, value);
            UpdateAvatar();
        }
        public void SetColor(string key, Color value)
        {
            currentSelect.SetColor(key, value);
        }
        private void OnDestroy()
        {
            randomBtn.onClick.RemoveAllListeners();
        }
        public void UpdateAvatar()
        {
            avatar.UpdateSkin(currentSelect.Skin, currentSelect.Colors);
        }
    }
}
