using CreateCharacter;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class OwnerIapProduct
    {
        public IapProduct Product { get; set; }
        public bool HasPurchased { get; set; }
    }
}