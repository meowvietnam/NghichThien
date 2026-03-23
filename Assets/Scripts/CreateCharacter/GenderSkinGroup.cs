using Base;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class GenderSkinGroup : IMemorySerializable<GenderSkinGroup>
    {
        [field: SerializeField] public Gender Gender { get; private set; }
        [field: SerializeField] public SkeletonDataAsset SkeletonDataAsset { get; private set; }
        [field: SerializeField] public List<SkinGroup> SkinGroups { get; private set; }

        public int ByteSize()
        {
            return Gender.ByteSize() + SkinGroups.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Gender);
            writer.WriteArray(SkinGroups);
        }

        public void Deserialize(IReader reader, ref GenderSkinGroup value)
        {
            value.Gender = reader.Read<Gender>();
            value.SkinGroups = reader.ReadArray<SkinGroup>().ToList();
        }
    }
}