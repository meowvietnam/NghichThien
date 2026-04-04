using Network.Models;

namespace Network.Callback
{
    public interface IInventoryCallback
    {
        void GetInventoryResult(GetInventoryResult result);
    }
}