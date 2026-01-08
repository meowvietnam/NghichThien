using Base;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class GenderSkinGroup
    {
        [field: SerializeField] public Gender Gender { get; private set; }
        [field: SerializeField] public SkeletonDataAsset SkeletonDataAsset { get; private set; }
        [field: SerializeField] public List<SkinGroup> SkinGroups { get; private set; }
    }
}
