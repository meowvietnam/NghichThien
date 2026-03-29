using System;
using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AllMaps
{
    [Serializable]
    public class Mail : IMemorySerializable<Mail>
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public string Title { get; set; } = string.Empty;
        [field: SerializeField] public string Content { get; set; } = string.Empty;
        [field: SerializeField] public bool IsRead { get; set; }
        [field: SerializeField] public bool IsClaim { get; set; }
        [field: SerializeField] public List<ItemRef> Rewards { get; set; } = new List<ItemRef>();
        [field: ShowInInspector] public DateTime CreatedAt { get; set; }

        public void Deserialize(IReader reader, ref Mail value)
        {
            value.Id = reader.Read<int>();
            value.Title = reader.Read();
            value.Content = reader.Read();
            value.IsRead = reader.Read<bool>();
            value.IsClaim = reader.Read<bool>();
            value.Rewards = reader.ReadArray<ItemRef>().ToList();
            value.CreatedAt = reader.Read<DateTime>();
        }

        public int ByteSize()
        {
            return Id.ByteSize() + Title.ByteSize() + Content.ByteSize()
                   + IsRead.ByteSize() + IsClaim.ByteSize() + Rewards.ByteSize()
                   + CreatedAt.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Title);
            writer.Write(Content);
            writer.Write(IsRead);
            writer.Write(IsClaim);
            writer.WriteArray(Rewards);
            writer.Write(CreatedAt);
        }
    }
}