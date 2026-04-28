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
        private InventoryManager Inventory => Client.Instance.CharacterData.InventoryManager;
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
                    var charData = Client.Instance.CharacterData;
                    charData.SetInventory(result.InventoryManager);
                }
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handlerTag = { MessageTag.GetInventoryResult };
        public MessageTag[] HandleTag => handlerTag;
    }
}