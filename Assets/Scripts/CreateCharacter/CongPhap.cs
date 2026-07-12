using System;
using System.Linq;
using Base;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial class CongPhap : IDeepClone<CongPhap>, IEquatable<CongPhap>
    {
        [field: SerializeField]
        public ushort Id { get; set; }
        [field: SerializeField]
        public ushort Level { get; set; }

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
            return gameManager.GetCongPhapWithId((int)Id);
        }
    }
}