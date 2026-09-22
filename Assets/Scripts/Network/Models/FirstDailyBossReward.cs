using System.Collections.Generic;
using CreateCharacter;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class FirstDailyBossReward
    {
        public List<IMailReward> Rewards { get; private set; }
    }
}