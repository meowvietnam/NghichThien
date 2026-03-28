using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class FurnaceData : IMemorySerializable<FurnaceData>
    {
        [field: SerializeField]
        public int SelfId { get; set; }
        [field: SerializeField]
        public ItemRef Ref { get; set; }
        public int ByteSize()
        {
            return SelfId.ByteSize() + Ref.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(SelfId);
            writer.Write(Ref);
        }

        public void Deserialize(IReader reader, ref FurnaceData value)
        {
            value.SelfId = reader.Read<int>();
            value.Ref = reader.Read<ItemRef>();
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(SelfId, Ref);
        }
    }
}