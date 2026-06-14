using Base;
using CreateCharacter;
using MemoryPack;
using System.Collections.Generic;

namespace Network.Models
{
    [MemoryPackable]
    public partial class IAPModel
    {

        public string ReceiptId { get; set; }
        public string ProductId { get; set; }
      


    }
}