using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;
namespace CreateCharacter.CharStats
{
    [Serializable]
    public class StatOptionGroup : IMemorySerializable<StatOptionGroup>
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public List<StatOption> StatOptions { get; private set; } 
        [field: SerializeField] public List<int> Exclusives { get; private set; }
        public int ByteSize()
        {
            return Id.ByteSize() + StatOptions.ByteSize() + Exclusives.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.WriteArray(StatOptions);
            writer.WriteArray(Exclusives);
        }

        public void Deserialize(IReader reader, ref StatOptionGroup value)
        {
            value.Id = reader.Read<int>();
            value.StatOptions = reader.ReadArray<StatOption>().ToList();
            value.Exclusives = reader.ReadArray<int>().ToList();
        }
    }
}
