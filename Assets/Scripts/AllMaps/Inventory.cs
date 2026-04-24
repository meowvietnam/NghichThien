using CreateCharacter;
using Network;
using Network.Callback;
using Network.Core;
using Network.Models;
using UnityEngine;

namespace AllMaps
{
    public class Inventory : MonoBehaviour, IInventoryCallback
    {
        private void Start()
        {
            var module = Client.Instance.CoreModule;
            module.RemoveHandler<InventoryHandler>();
            var inventoryHandler = new InventoryHandler();
            inventoryHandler.OnInventoryUpdated += OnInventoryUpdated;
            module.AddHandler(inventoryHandler);
        }

        public void OnInventoryUpdated(InventoryManager result)
        {
            
        }

        private void OnDestroy()
        {
            var module = Client.Instance.CoreModule;
            var inventoryHandler = module.GetHandler<InventoryHandler>();
            inventoryHandler.OnInventoryUpdated -= OnInventoryUpdated;
        }
    }
}