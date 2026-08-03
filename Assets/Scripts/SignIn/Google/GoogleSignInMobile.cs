using System.Threading.Tasks;
#if !UNITY_IPHONE
using Google;
#endif
using UnityEngine;

namespace SignIn.Google
{
    [CreateAssetMenu(fileName = "GoogleSignInMobile", menuName = "SignIn/Google/GoogleSignInMobile")]
    public class GoogleSignInMobile : GoogleSignInPlatform
    {
        [SerializeField] private string clientId = "YOUR_CLIENT_ID.apps.googleusercontent.com";

        public override async Task<GoogleCredential> SignInAsync()
        {
#if !UNITY_IPHONE
            global::Google.GoogleSignIn.Configuration = new GoogleSignInConfiguration()
            {
                RequestIdToken = true,
                RequestEmail = true,
                WebClientId = clientId,
            };
            try
            {
                var googleUser = await global::Google.GoogleSignIn.DefaultInstance.SignIn();
                return new GoogleCredential(email: googleUser.Email, idToken: googleUser.IdToken,
                    imageUrl: googleUser.ImageUrl, name: googleUser.DisplayName);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
#else
            return null;
#endif
        }

        public override Task SignOutAsync()
        {
#if !UNITY_IPHONE
global::Google.GoogleSignIn.DefaultInstance.SignOut();
#endif
            return Task.CompletedTask;
        }
    }
}