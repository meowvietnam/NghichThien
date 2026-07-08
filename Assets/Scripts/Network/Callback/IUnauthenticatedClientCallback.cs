using Network.Models;

namespace Network.Callback
{
    public interface IUnauthenticatedClientCallback
    {
        void OnConnectResult(ConnectResult connectResult);
        void OnCheckVersionResult(CheckVersionResult result);
        void OnDisconnect();
    }
}