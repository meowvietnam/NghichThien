using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class GenderSkinGroup
    {
        [field: SerializeField] public SkinGender Gender { get; private set; }
        [field: SerializeField] public List<SkinGroup> SkinGroups { get; private set; }
        public GenderSkinGroup(SkinGender gender, List<SkinGroup> skinGroups)
        {
            Gender = gender;
            SkinGroups = skinGroups;
        }
    }
}
