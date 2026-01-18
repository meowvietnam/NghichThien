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
        public void Init(SkinResources skinResources)
        {
            this.skinResources = skinResources;
        }
    }
}
