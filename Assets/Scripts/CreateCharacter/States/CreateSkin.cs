using System.IO;
using Network;
using Network.Extensions;
using Network.Models;
using UnityEngine;

namespace CreateCharacter.States
{
    public class CreateSkin : ICreateCharacterState
    {
        private readonly CreateCharacterPanel createCharacterPanel;
        private AbstractCreateCharacterSystem system;

        public CreateSkin(CreateCharacterPanel createCharacterPanel, AbstractCreateCharacterSystem system)
        {
            this.createCharacterPanel = createCharacterPanel;
            this.system = system;
        }

        public void OnEnter()
        {
            SetPlayerAvatar.Instance.SetActive(true);
        }

        public void Next()
        {
            var currentSelect = SetPlayerAvatar.Instance.CurrentSelect;
            var gender = currentSelect.Gender;
            var skin = currentSelect.Skin;
            var color = currentSelect.Colors;
            var request = new SetSkinRequest()
            {
                Gender = gender,
                Skins = skin,
                Colors = color,
            };
            var newCharacter = system.NewCharacter;
            newCharacter.Gender = gender;
            newCharacter.Appearance.Skins = skin;
            newCharacter.Appearance.Colors = color;
            Client.Instance.Send(MessageTag.SetSkins, request);
        }

        public void Back()
        {
            system.ToSetPlayerNamePanel(createCharacterPanel);
        }

        public void OnExit()
        {
            SetPlayerAvatar.Instance.SetActive(false);
        }
    }
}