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
            Client.Instance.CharacterData.OnInventoryUpdated += OnInventoryUpdated;
            module.AddHandler(inventoryHandler);
        }

        public void OnInventoryUpdated(InventoryManager result)
        {
            
        }

        private void OnDestroy()
        {
            var client = Client.Instance;
            if (client == null) return;
            var module = client.CoreModule;
            var inventoryHandler = module.GetHandler<InventoryHandler>();
            if (inventoryHandler != null)
            {
                module.RemoveHandler(inventoryHandler);
            }
            var charData = client.CharacterData;
            if (charData != null)
            {
                charData.OnInventoryUpdated -= OnInventoryUpdated;
            }
        }
    }
}