using System;
using System.Threading.Tasks;
using Network;
using Network.ClientState;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace SignIn.Google
{
    public class GoogleSignInButton : SignInButton
    {
        [SerializeField] private GoogleSignIn googleSignIn;

        protected override async Task<ILogin> CreateRequest()
        {
            try
            {
                var credential = await googleSignIn.SignInAsync() as GoogleCredential;
                if (credential == null)
                {
                    return null;
                }

                return new LoginWithGoogle()
                {
                    IdToken = credential.IdToken,
                };
            }
            catch (NewGoogleSignInSessionException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override ISignInService GetSignInService()
        {
            return googleSignIn;
        }
    }
}