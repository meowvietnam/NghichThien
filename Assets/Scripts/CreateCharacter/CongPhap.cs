using System;
using System.Linq;
using Base;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class CongPhap : IMemorySerializable<CongPhap>, IDeepClone<CongPhap>
    {
        [field: SerializeField]
        public ushort Id { get; set; }
        [field: SerializeField]
        public ushort Level { get; set; }
        public void Deserialize(IReader reader, ref CongPhap value)
        {
            value.Id = reader.Read<ushort>();
            value.Level = reader.Read<ushort>();
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Equals(CongPhap? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
        public int ByteSize()
        {
            return Id.ByteSize() + Level.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Level);
        }
        public CongPhap DeepClone()
        {
            return new CongPhap()
            {
                Id = Id,
                Level = Level
            };
        }

        public Skill.CongPhap GetCongPhap()
        {
            var gameManager = LongDHModules.Pool.Get<GameManager>();
            return gameManager.congPhaps.FirstOrDefault(x => x.Id == Id);
        }
    }
}