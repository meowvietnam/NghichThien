using CreateCharacter;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SetCamNgoResult
    {
        public CamNgo CamNgo { get; set; }
        public int NewNgoDaoDiem { get; set; }
    }
}