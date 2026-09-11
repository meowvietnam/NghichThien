using MemoryPack;
using SignIn;

namespace Network.Models
{
    [MemoryPackable]
    public partial class LoginGuest : ILogin
    {
        public string Username { get; set; }
        public string DeviceId { get; set; }

        public SignInProvider GetSignInProvider()
        {
            return SignInProvider.Guest;
        }
    }
}