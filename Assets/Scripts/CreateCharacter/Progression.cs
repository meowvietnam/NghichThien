using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class Progression : IMemorySerializable<Progression>
    {
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public int CurrentExp { get; set; }
        [field: SerializeField] public int CurrentEnergy { get; set; }
        [field: SerializeField] public int NgoDaoDiem { get; set; }
        [field: SerializeField] public int IdCurrentNode { get; set; }
        [field: SerializeField] public int MaxLevel { get; set; }

        public void Deserialize(IReader reader, ref Progression value)
        {
            value.Level = reader.Read<int>();
            value.CurrentExp = reader.Read<int>();
            value.CurrentEnergy = reader.Read<int>();
            value.NgoDaoDiem = reader.Read<int>();
            value.IdCurrentNode = reader.Read<int>();
            value.MaxLevel = reader.Read<int>();
        }

        public int ByteSize()
        {
            return Level.ByteSize() + CurrentExp.ByteSize() + CurrentEnergy.ByteSize() + NgoDaoDiem.ByteSize() +
                   IdCurrentNode.ByteSize() + MaxLevel.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Level);
            writer.Write(CurrentExp);
            writer.Write(CurrentEnergy);
            writer.Write(NgoDaoDiem);
            writer.Write(IdCurrentNode);
            writer.Write(MaxLevel);
        }
    }
}
