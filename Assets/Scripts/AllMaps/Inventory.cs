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
            var inventoryHandler = new InventoryHandler(this);
            module.AddHandler(inventoryHandler);
        }

        public void GetInventoryResult(GetInventoryResult result)
        {
            
        }
    }
}