using System.Threading.Tasks;
using UnityEngine;

namespace SignIn
{
    public interface ISignInService
    {
        SignInProvider Provider { get; }
        Task<ISignInCredential> SignInAsync();
        Task SignOutAsync();
    }
}