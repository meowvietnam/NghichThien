using Network.Models;

namespace Network.Callback
{
    public interface IUnauthenticatedClientCallback
    {
        void OnConnectResult(ConnectResult connectResult);
        void OnCheckVersionResult(bool result);
        void OnDisconnect();
    }
}