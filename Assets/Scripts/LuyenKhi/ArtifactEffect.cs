using System;
using System.Linq;
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

        private EffectTypeConfig Config
        {
            get
            {
                return LuyenKhiResources.Instance.EffectTypeNameConfigs.First(x => x.Type == EffectType);
            }
        }
        [field: SerializeField] public EffectType EffectType { get; set; }
        [ShowInInspector] public string Name => Config.Name;
        [ShowInInspector] public int Value => Config.Value;
        public void Deserialize(IReader reader, ref ArtifactEffect value)
        {
           value.ArtifactType = reader.Read<ArtifactType>();
           value.EffectType = reader.Read<EffectType>();
        }
        public int ByteSize()
        {
            return ArtifactType.ByteSize() + EffectType.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(ArtifactType);
            writer.Write(EffectType);
        }
    }
}