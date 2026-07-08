using MemoryPack;
using SignIn;

namespace Network.Models
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(LoginWithGoogle))]
    public partial interface ILogin
    {
        SignInProvider GetSignInProvider();
    }
}