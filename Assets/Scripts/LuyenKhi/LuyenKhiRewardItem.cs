using System;
using LuyenDan;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class LuyenKhiRewardItem : IMemorySerializable<LuyenKhiRewardItem>
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public EquipmentTier Tier { get; set; }
        [field: SerializeField] public ArtifactType Type { get; set; }
        [field: SerializeField, PreviewField(75)] public Sprite Icon { get; set; }
        [field: SerializeField, PreviewField(75)] public Sprite IconFight { get; set; }


        public void Deserialize(IReader reader, ref LuyenKhiRewardItem value)
        {
            value.Id = reader.Read<int>();
            value.Tier = reader.Read<EquipmentTier>();
            value.Type = reader.Read<ArtifactType>();
        }
        public int ByteSize()
        {
            return Id.ByteSize() + Tier.ByteSize() + Type.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Tier);
            writer.Write(Type);
        }
    }
}