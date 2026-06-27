using Base;
using CreateCharacter;
using MemoryPack;
using System.Collections.Generic;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ResultCombat
    {
       
        public List<int> ListIdThanThong { get; set; }
        public ETypeCombat TypeCombat { get; set; }
        public int CurrentHPUser { get; set; }
        public int CurrentLevelEnemy { get; set; }
        public ETypeResultCombat TypeResultCombat { get; set; }

    }
}