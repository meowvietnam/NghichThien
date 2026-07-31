using CreateCharacter;
using MemoryPack;
using Network.Extensions;
using SignIn;

namespace Network.Models
{
    [MemoryPackable]
    public partial class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserWrapper UserData { get; set; }
        public string AccountAlias { get; set; }
        public SignInProvider Provider { get; set; }
    }
}