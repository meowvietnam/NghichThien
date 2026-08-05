using System;
using System.Threading.Tasks;
using Google;
using UnityEngine;

namespace SignIn.Google
{
    [CreateAssetMenu(fileName = "GoogleSignInMobile", menuName = "SignIn/Google/GoogleSignInMobile")]
    public class GoogleSignInMobile : GoogleSignInPlatform
    {
        [SerializeField] private string clientId = "YOUR_CLIENT_ID.apps.googleusercontent.com";

        public override async Task<GoogleCredential> SignInAsync()
        {
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
        }

        public override Task SignOutAsync()
        {
            global::Google.GoogleSignIn.DefaultInstance.SignOut();
            return Task.CompletedTask;
        }
    }
}