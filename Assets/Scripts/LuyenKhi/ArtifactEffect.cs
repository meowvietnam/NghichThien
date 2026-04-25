using System;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class ArtifactEffect : IMemorySerializable<ArtifactEffect>
    {
        [ShowInInspector]
        public EquipmentType EquipmentType
        {
            get
            {
                switch (ArtifactType)
                {
                    case ArtifactType.PhiKiem:
                    case ArtifactType.BaoChung:
                    case ArtifactType.ChiHoan:
                    case ArtifactType.PhiCham:
                    case ArtifactType.BaoHap:
                        return EquipmentType.Weapon;
                    case ArtifactType.TruongBao:
                    case ArtifactType.GiapTru:
                        return EquipmentType.Armor;
                    default:
                        return EquipmentType.Jewelry;
                }
            }
        }
        [field: SerializeField] public ArtifactType ArtifactType { get; set; }
        [field: SerializeField] public BaseEffect Effect { get; set; }
        public void Deserialize(IReader reader, ref ArtifactEffect value)
        {
           value.ArtifactType = reader.Read<ArtifactType>();
           value.Effect = reader.Read<BaseEffect>();
        }

        public int ByteSize()
        {
            return ArtifactType.ByteSize() + Effect.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(ArtifactType);
            writer.Write(Effect);
        }
    }
}