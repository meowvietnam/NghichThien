using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SignIn.Guest
{
    [CreateAssetMenu(fileName = "GuestSignIn", menuName = "SignIn/Guest/GuestSignIn")]
    public class GuestSignIn : SignInService
    {
        public const string USERNAME_KEY = "guest-username";
        public override SignInProvider Provider => SignInProvider.Guest;
        public override Task<ISignInCredential> SignInAsync()
        {
            string username;
            if (!PlayerPrefs.HasKey(USERNAME_KEY))
            {
                username = Guid.NewGuid().ToString("N");
                PlayerPrefs.SetString(USERNAME_KEY, username);
            }
            else
            {
                username = PlayerPrefs.GetString(USERNAME_KEY);
            }
            return Task.FromResult<ISignInCredential>(new GuestCredential()
            {
                Username = username,
            });
        }

        public override Task SignOutAsync()
        {
            PlayerPrefs.DeleteKey(USERNAME_KEY);
            return Task.CompletedTask;
        }
    }
}