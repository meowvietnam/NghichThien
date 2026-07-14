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
       
            var charData = Client.Instance.CharacterData;
            charData.OnInventoryUpdated += OnInventoryUpdated;
            charData.GetInventory();
        }

        public void OnInventoryUpdated(InventoryManager result)
        {
            
        }

        private void OnDestroy()
        {
            var client = Client.Instance;
            if (client == null) return;
            var charData = client.CharacterData;
            if (charData != null)
            {
                charData.OnInventoryUpdated -= OnInventoryUpdated;
            }
        }
    }
}