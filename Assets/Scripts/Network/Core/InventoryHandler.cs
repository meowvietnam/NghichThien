using System;
using CreateCharacter;
using Network.Callback;
using Network.Extensions;
using Network.Models;
using UnityEngine;

namespace Network.Core
{
    public class InventoryHandler : IClientHandler
    {
        private Action<GetInventoryResult> getInventoryCallback;
        public event Action<InventoryManager> OnInventoryUpdated;

        public InventoryHandler()
        {
        }

        public void OnEnter()
        {
            Client.Instance.Send(MessageTag.GetInventory);
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.GetInventoryResult)
            {
                var result = reader.Read<GetInventoryResult>();
                if (result.Success)
                {
                    SetInventory(result.InventoryManager);
                }
            }
        }

        public void SetInventory(InventoryManager inventoryManager)
        {
            Client.Instance.CharacterData.InventoryManager = inventoryManager;
            OnInventoryUpdated?.Invoke(inventoryManager);
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handlerTag = { MessageTag.GetInventoryResult };
        public MessageTag[] HandleTag => handlerTag;

        public void GetInventory()
        {
            Client.Instance.Send(MessageTag.GetInventory);
        }
    }
}