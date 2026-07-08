namespace SignIn
{
    public interface ISignInCredential
    {
        SignInProvider Provider { get; }
    }
}