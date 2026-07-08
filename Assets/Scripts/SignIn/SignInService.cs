using System.Threading.Tasks;
using UnityEngine;

namespace SignIn
{
    public abstract class SignInService : ScriptableObject, ISignInService
    {
        public abstract SignInProvider Provider { get; }
        public abstract Task<ISignInCredential> SignInAsync();
        public abstract Task SignOutAsync();
    }
}