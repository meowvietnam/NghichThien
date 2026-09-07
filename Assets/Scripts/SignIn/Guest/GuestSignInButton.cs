using System.Threading.Tasks;
using Network.Models;
using UnityEngine;

namespace SignIn.Guest
{
    public class GuestSignInButton : SignInButton
    {
        [SerializeField] private GuestSignIn guestSignIn;

        protected override async Task<ILogin> CreateRequest()
        {
            var credental = await guestSignIn.SignInAsync() as GuestCredential;
            return new LoginGuest()
            {
                Username = credental.Username,
                DeviceId = credental.DeviceId
            };
        }

        public override ISignInService GetSignInService()
        {
            return guestSignIn;
        }
    }
}