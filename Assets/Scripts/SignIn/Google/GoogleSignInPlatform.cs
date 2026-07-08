using System.Threading.Tasks;
using UnityEngine;

namespace SignIn.Google
{
    public abstract class GoogleSignInPlatform : ScriptableObject
    {
        [field: SerializeField] public Platform Platform { get; private set; }
        public abstract Task<GoogleCredential> SignInAsync();
        public abstract Task SignOutAsync();
    }
}