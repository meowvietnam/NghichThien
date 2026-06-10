using System;
using Network;
using Network.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class QuestManager : IMemorySerializable<QuestManager>
    {
        [field: SerializeField]
        public int CurrentMainQuest { get; set; } = 0;
        [field: SerializeField]
        public int CurrentCountSideQuest { get; private set; } = 0;
        public void Deserialize(IReader reader, ref QuestManager value)
        {
            value.CurrentMainQuest = reader.Read<int>();
            value.CurrentCountSideQuest = reader.Read<int>();
        }
        public int ByteSize()
        {
            return CurrentMainQuest.ByteSize() + CurrentCountSideQuest.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(CurrentMainQuest);
            writer.Write(CurrentCountSideQuest);
        }
    }
}