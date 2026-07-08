using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SignIn.Google
{
    [CreateAssetMenu(fileName = "GoogleSignIn", menuName = "SignIn/Google/GoogleSignIn")]
    public class GoogleSignIn : SignInService
    {
        [SerializeField] private GoogleSignInPlatform[] signInPlatforms;
        private GoogleSignInPlatform signInPlatform;
        public event Action<GoogleCredential> OnSignIn;
        public event Action OnSignOut;

        private GoogleSignInPlatform SignInPlatform
        {
            get
            {
                if (signInPlatform == null)
                {
                    var platform = PlatformManager.GetPlatform();
                    signInPlatform = signInPlatforms.FirstOrDefault(x => x.Platform == platform);
                }

                return signInPlatform;
            }
        }

        public override SignInProvider Provider => SignInProvider.Google;

        [ContextMenu(nameof(SignInAsync))]
        public override async Task<ISignInCredential> SignInAsync()
        {
            var credential = await SignInPlatform.SignInAsync();
            OnSignIn?.Invoke(credential);
            return credential;
        }

        [ContextMenu(nameof(SignOutAsync))]
        public override async Task SignOutAsync()
        {
            await SignInPlatform.SignOutAsync();
            OnSignOut?.Invoke();
        }
    }
}