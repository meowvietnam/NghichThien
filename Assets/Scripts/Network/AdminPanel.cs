using System.Collections.Generic;
using LuyenDan;
using Network.Core;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Network
{
    public class AdminPanel : MonoBehaviour, IClientHandler
    {
        private Client Client => Client.Instance;
        private static AdminPanel instance;

        public static AdminPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<AdminPanel>();
                }
                return instance;
            }
        }
        private bool IsReady()
        {
            if (Client == null)
            {
                return false;
            }

            return Client.CharacterData != null;
        }
        [Button]
        [ShowIf(nameof(IsReady))]
        public void AddItems(List<ItemRef> items) 
        {
            if (items == null || items.Count == 0)
            {
                Debug.LogWarning("AddItems called with no items");
                return;
            }
            Client.Send(MessageTag.AddItems, items);
        }

        public void StartListen()
        {
            var module = Client.CoreModule;
            module.RemoveHandler<AdminPanel>();
            module.AddHandler(this);
        }

        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.AddItemsResult)
            {
                var rewards = reader.Read<RewardItems>();
                var inventoryManager = Client.CharacterData.InventoryManager;
                inventoryManager.AddToInventory(rewards);
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handler = { MessageTag.AddItemsResult };
        public MessageTag[] HandleTag => handler;
    }
}