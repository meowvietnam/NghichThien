using Network.Models;

namespace Network.Callback
{
    public interface IAllMapsCallback
    {
        void OnMoveToMapResult(MoveToMapResult result);
    }
}