using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}