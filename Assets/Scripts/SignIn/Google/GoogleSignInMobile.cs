using System.Threading.Tasks;
using UnityEngine;

namespace SignIn.Google
{
    [CreateAssetMenu(fileName = "GoogleSignInMobile", menuName = "SignIn/Google/GoogleSignInMobile")]
    public class GoogleSignInMobile : GoogleSignInPlatform
    {
        [SerializeField] private string clientId = "YOUR_CLIENT_ID.apps.googleusercontent.com";

        public override async Task<GoogleCredential> SignInAsync()
        {
            return null;
        }

        public override Task SignOutAsync()
        {
            return Task.CompletedTask;
        }
    }
}