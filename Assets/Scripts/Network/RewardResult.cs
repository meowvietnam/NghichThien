using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using MemoryPack;
using Network.Extensions;

namespace Network
{
    [MemoryPackable]
    public partial class RewardResult
    {
        public List<long> Mails { get; set; }
        public List<IMailReward> Rewards { get; set; }
    }
}