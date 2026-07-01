using Base;
using CreateCharacter;
using MemoryPack;
using System.Collections.Generic;

namespace Network.Models
{
    [MemoryPackable]
    public partial class GachaModel
    {

        public ETypeGacha TypeGacha { get; set; }
        public bool IsGacha10 { get; set; }



    }
}