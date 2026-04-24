using CreateCharacter;
using Network.Models;

namespace Network.Callback
{
    public interface IInventoryCallback
    {
        void OnInventoryUpdated(InventoryManager inventoryManager);
    }
}