using MemoryPack;
using SignIn;

namespace Network.Models
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(LoginWithGoogle))]
    [MemoryPackUnion(1, typeof(AdminLoginWithGoogle))]
    [MemoryPackUnion(2, typeof(LoginWithUsernamePassword))]
    [MemoryPackUnion(3, typeof(LoginGuest))]
    [MemoryPackUnion(4, typeof(AdminLoginWithName))]
    public partial interface ILogin
    {
        SignInProvider GetSignInProvider();
    }
}