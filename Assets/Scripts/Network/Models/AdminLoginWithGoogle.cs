using MemoryPack;
using SignIn;

namespace Network.Models
{
    [MemoryPackable]
    public partial class AdminLoginWithGoogle : ILogin
    {
        public string Email { get; set; }
        public string SecretKey { get; set; }
        public SignInProvider GetSignInProvider()
        {
            return SignInProvider.AdminGoogle;
        }
    }
}