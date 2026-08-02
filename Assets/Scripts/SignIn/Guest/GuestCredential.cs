namespace SignIn.Guest
{
    public class GuestCredential : ISignInCredential
    {
        public string Username { get; set; }
        public string DeviceId { get; set; }
        public SignInProvider Provider => SignInProvider.Guest;
    }
}