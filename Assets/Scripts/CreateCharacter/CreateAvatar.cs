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

        private string GetLevelText(int level)
        {
            return level switch
            {
                <= 0 => "Phàm Nhân",
                1 => "Luyện Khí tầng 1 – Cảm Khí",
                2 => "Luyện Khí tầng 2 – Dẫn Khí",
                3 => "Luyện Khí tầng 3 – Thông Mạch",
                4 => "Luyện Khí tầng 4 – Ổn Khí",
                5 => "Luyện Khí tầng 5 – Khí Hải",
                6 => "Luyện Khí tầng 6 – Tăng Linh",
                7 => "Luyện Khí tầng 7 – Hóa Khí",
                8 => "Luyện Khí tầng 8 – Ngưng Khí",
                9 => "Luyện Khí tầng 9 – Đại Viên Mãn",
                10 => "Luyện Khí tầng 10 – Hậu Kỳ",
                11 => "Luyện Khí tầng 11 – Hậu Kỳ",
                12 => "Luyện Khí tầng 12 – Hậu Kỳ",
                13 => "Luyện Khí tầng 13 – Cực Cảnh",
                _ => "Trúc Cơ"
            };
        }
        private void SelectCharacter()
        {
            
        }
        private void CreateCharacter()
        {
            CreateCharacterSystem.Instance.BeginCreateCharacter();
        }
        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}