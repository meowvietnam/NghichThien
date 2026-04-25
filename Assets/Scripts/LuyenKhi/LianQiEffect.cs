using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class LianQiEffect : IMemorySerializable<LianQiEffect>
    {
        [field: SerializeField] public CultivationElement CultivationElement { get; set; }
        [field: SerializeField] public YinYangType YinYangType { get; set; }
        [field: SerializeField] public ArtifactEffect[] Effects { get; set; }
        public void Deserialize(IReader reader, ref LianQiEffect value)
        {
            value.CultivationElement = reader.Read<CultivationElement>();
            value.YinYangType = reader.Read<YinYangType>();
            value.Effects = reader.ReadArray<ArtifactEffect>();
        }

        public int ByteSize()
        {
            return CultivationElement.ByteSize() + YinYangType.ByteSize() + Effects.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(CultivationElement);
            writer.Write(YinYangType);
            writer.WriteArray(Effects);
        }
    }
}