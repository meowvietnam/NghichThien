using UnityEngine;

namespace CreateCharacter.States
{
    public class CreateSkin : ICreateCharacterState
    {
        private readonly CreateCharacterPanel createCharacterPanel;
        public CreateSkin(CreateCharacterPanel createCharacterPanel)
        {
            this.createCharacterPanel = createCharacterPanel;
        }
        public void OnEnter()
        {
            SetPlayerAvatar.Instance.SetActive(true);
        }
        public void Next()
        {
            var currentSelect = SetPlayerAvatar.Instance.CurrentSelect;
            var skin = currentSelect.Skin;
            var color = currentSelect.Colors;
            var newCharacter = CreateCharacterSystem.Instance.NewCharacter;
            newCharacter.Gender = currentSelect.Gender;
            newCharacter.Skins = skin;
            newCharacter.Colors = color;
            createCharacterPanel.SetState(new SelectStatsState(createCharacterPanel));
        }
        public void Back()
        {
            CreateCharacterSystem.Instance.ToSetPlayerNamePanel(createCharacterPanel);
        }

        public void OnExit()
        {
            SetPlayerAvatar.Instance.SetActive(false);
        }
    }
}