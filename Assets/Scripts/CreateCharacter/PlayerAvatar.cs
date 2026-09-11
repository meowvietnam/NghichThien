using Base;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class PlayerAvatar : CharacterGraphic
    {
        public void Init(SkinResources skinResources, CharacterData charData)
        {
            this.skinResources = skinResources;
            if (charData == null)
            {
                return;
            }

            var appearance = charData.Appearance;
            SetGender(charData.Gender);
            SetSkin(appearance.Skins, appearance.Colors);
        }
    }
}