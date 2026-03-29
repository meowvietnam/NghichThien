using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class CreateAvatar : CharacterGraphic
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject addGO;
        [SerializeField] private GameObject hasCharGO;
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Image levelImage;
        private CharacterData characterData;
        public void Init(CharacterData characterData, SkinResources skinResources)
        {
            this.characterData = characterData;
            this.skinResources = skinResources;
            var hasChar = characterData != null;
            addGO.SetActive(!hasChar);
            hasCharGO.SetActive(hasChar);
            if (hasChar)
            {
                SetGender(characterData.Gender);
                SetSkin(characterData.Skins,characterData.Colors);
                nameText.text = characterData.Name;
                levelText.text = GetLevelText(characterData.Level);
                levelImage.sprite = GetIconLevel(characterData.Level);
                button.onClick.AddListener(SelectCharacter);
            }
            else
            {
                button.onClick.AddListener(CreateCharacter);
            }
        }

        private Sprite GetIconLevel(int level)
        {
            return skinResources.LevelIcons[level - 1];
        }
        public static Dictionary<int, string> levelWithText = new Dictionary<int, string>
        {
            // Luyện Khí
            { 1, "Luyện Khí Sơ Kì" },
            { 2, "Luyện Khí Trung Kì" },
            { 3, "Luyện Khí Hậu Kì" },

            // Trúc Cơ
            { 4, "Trúc Cơ Sơ Kì" },
            { 5, "Trúc Cơ Trung Kì" },
            { 6, "Trúc Cơ Hậu Kì" },

            // Kim Đan
            { 7, "Kim Đan Sơ Kì" },
            { 8, "Kim Đan Trung Kì" },
            { 9, "Kim Đan Hậu Kì" },

            // Nguyên Anh
            { 10, "Nguyên Anh Sơ Kì" },
            { 11, "Nguyên Anh Trung Kì" },
            { 12, "Nguyên Anh Hậu Kì" },

            // Hóa Thần
            { 13, "Hóa Thần Sơ Kì" },
            { 14, "Hóa Thần Trung Kì" },
            { 15, "Hóa Thần Hậu Kì" },
        };
        private string GetLevelText(int level)
        {
            return levelWithText[level];
        }
        private void SelectCharacter()
        {
            CreateCharacterSystem.Instance.SelectCharacter(characterData.Id);
        }
        [Button]
        private void CreateCharacter()
        {
            Client.Instance.Send(MessageTag.CanCreateCharacter);
        }
        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}