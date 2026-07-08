using System;

namespace SignIn.Google
{
    public class GoogleCredential : ISignInCredential
    {
        public SignInProvider Provider => SignInProvider.Google;
        public string IdToken { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public Uri ImageUrl { get; private set; }

        public GoogleCredential()
        {
        }

        public GoogleCredential(string email, string name, string idToken, Uri imageUrl)
        {
            Email = email;
            Name = name;
            IdToken = idToken;
            ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            return $"IdToken: {IdToken}, Name: {Name}, Email: {Email}, ImageUrl: {ImageUrl}";
        }
    }
}