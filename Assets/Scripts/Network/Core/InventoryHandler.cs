using Network.Callback;
using Network.Extensions;
using Network.Models;
using UnityEngine;

namespace Network.Core
{
    public class InventoryHandler : IClientHandler
    {
        private IInventoryCallback callback;
        public InventoryHandler(IInventoryCallback callback)
        {
            this.callback = callback;
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
                Client.Instance.CharacterData.InventoryManager = result.InventoryManager;
                callback.GetInventoryResult(result);
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