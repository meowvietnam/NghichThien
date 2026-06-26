using Base;
using CreateCharacter;
using MemoryPack;
using System.Collections.Generic;
using IAP;

namespace Network.Models
{
    [MemoryPackable]
    public partial class IapModel
    {
        public Store Store { get; set; }
        public string ProductId { get; set; }
        public string PurchaseToken { get; set; }
    }
}