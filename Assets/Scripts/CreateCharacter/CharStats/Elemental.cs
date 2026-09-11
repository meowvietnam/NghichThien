using System;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [Serializable]
    public class Elemental : IMemorySerializable<Elemental>
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public ElementalType ElementalType { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite SelectSprite { get; private set; }
        [ShowInInspector] public StatType StatType => (StatType)(ElementalType + 1);

        public int ByteSize()
        {
            return Id.ByteSize() + ElementalType.ByteSize() + Name.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(ElementalType);
            writer.Write(Name);
        }

        public void Deserialize(IReader reader, ref Elemental value)
        {
            value.Id = reader.Read<int>();
            value.ElementalType = reader.Read<ElementalType>();
            value.Name = reader.Read();
        }
    }
}