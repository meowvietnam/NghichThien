using System;
using CreateCharacter;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace Rank
{
    [MemoryPackable]
    [Serializable]
    public partial class CharacterWrapper
    {
        [MemoryPackIgnore]
        [field: SerializeField]
        public CharacterData Character { get; private set; }
        [field: SerializeField]
        public byte[] CharacterBytes { get; private set; }
        public CharacterWrapper(CharacterData character)
        {
            Character = character;
            var writer = new Writer(character.ByteSize());
            writer.Write(Character);
            CharacterBytes = writer.Buffer;
        }

        [MemoryPackConstructor]
        public CharacterWrapper(byte[] characterBytes)
        {
            CharacterBytes = characterBytes;
            var reader = new Reader(characterBytes);
            Character = reader.Read<CharacterData>();
        }
    }
}