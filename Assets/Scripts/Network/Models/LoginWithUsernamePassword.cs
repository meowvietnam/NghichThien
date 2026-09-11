using MemoryPack;
using SignIn;

namespace Network.Models
{
    [MemoryPackable]
    public partial class LoginWithUsernamePassword : ILogin, ISignInCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public SignInProvider GetSignInProvider()
        {
            return SignInProvider.UsernamePassword;
        }

        [MemoryPackIgnore] public SignInProvider Provider => GetSignInProvider();
    }
}