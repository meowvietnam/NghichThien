using MemoryPack;
using SignIn;

namespace Network.Models
{
    [MemoryPackable]
    public partial class AdminLoginWithName : ILogin
    {
        public string Name { get; set; }
        public string SecretKey { get; set; }
        public SignInProvider GetSignInProvider()
        {
            return SignInProvider.AdminName;
        }
    }
}