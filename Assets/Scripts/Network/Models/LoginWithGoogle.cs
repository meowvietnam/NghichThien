using MemoryPack;
using SignIn;

namespace Network.Models
{
    [MemoryPackable]
    public partial class LoginWithGoogle : ILogin
    {
        public string IdToken { get; set; }

        public SignInProvider GetSignInProvider()
        {
            return SignInProvider.Google;
        }
    }
}