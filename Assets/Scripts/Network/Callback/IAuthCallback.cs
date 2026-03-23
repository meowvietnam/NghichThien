using Network.Models;

namespace Network.Callback
{
    public interface IAuthCallback
    {
        void OnLoginResult(LoginResult result);
    }
}