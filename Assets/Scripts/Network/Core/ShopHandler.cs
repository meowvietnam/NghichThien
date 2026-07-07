using System.Collections.Generic;
using System.Linq;
using AllMaps;
using AllMaps.ItemProvider;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.Core
{
    public class ShopHandler : IClientHandler
    {
        private INetworkEvent.IReloadShop reloadShopCallback;

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.LOAD_SHOP, LoadShop);
            Observer.AddObserver(EventList.Network.RELOAD_SHOP, ReloadShop);
            Observer.AddObserver(EventList.Network.BUY_AND_SELL, BuyAndSell);
        }

        private async void BuyAndSell(object[] data)
        {
            var callback = data[0] as INetworkEvent.IBuyAndSell;
            var buyItems = data[1] as List<ItemRef>;
            var sellItems = data[2] as List<ItemRef>;
            var shopType = (EShopType)data[3];
            var client = Client.Instance;
            var request = new BuyAndSell()
            {
                BuyItems = buyItems,
                SellItems = sellItems,
                ShopType = shopType,
            };
            var reader = await client.RequestWait(MessageTag.BuyAndSellRequest, request);
            var result = reader.MemRead<BuyAndSellResult>();
            var shopManager = client.CharacterData.ShopManager;
            var inventory = client.CharacterData.InventoryManager;
            if (result.Success)
            {
                var shop = result.ReloadShop;
                shopManager.SetShop(shop);
                var inventoryTracking = new InventoryTracking()
                {
                    Furnaces = result.Furnaces,
                    Items = result.Items,
                };
                inventoryTracking.Apply(inventory);
                client.CharacterData.Stats.LinhThach = result.LinhThach;
            }

            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    callback.SuccessBuyAndSell(result.Message);
                }
                else
                {
                    Debug.Log(result.Message);
                    callback.FailBuyAndSell(result.Message);
                }
            });
        }

        private async void LoadShop(object[] data)
        {
            var callback = data[0] as INetworkEvent.ILoadShop;
            var shopType = (EShopType)data[1];
            var client = Client.Instance;
            var shopManager = client.CharacterData.ShopManager;
            var reader = await client.RequestWait(MessageTag.LoadShopRequest, shopType);
            var shop = reader.MemRead<Shop>();
            shopManager.SetShop(shop);
            NetworkManager.Instance.Invoke(() => { callback?.SuccessLoadShop(shop.Items.ToList()); });
        }

        private void ReloadShop(object[] data)
        {
            reloadShopCallback = data[0] as INetworkEvent.IReloadShop;
            var shopType = (EShopType)data[1];
            Client.Instance.Send(MessageTag.ReloadShop, shopType);
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            var client = Client.Instance;
            if (tag == MessageTag.ReloadShopResult)
            {
                var result = reader.MemRead<ReloadShopResult>();
                var shopManager = client.CharacterData.ShopManager;
                if (result.Success)
                {
                    shopManager.SetShop(result.Shop);
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        reloadShopCallback?.SuccessReloadShop(result.Shop.Items.ToList());
                    }
                    else
                    {
                        reloadShopCallback?.FailReLoadShop(result.Message);
                    }
                });
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] hanlers = { MessageTag.ReloadShopResult };
        public MessageTag[] HandleTag => hanlers;
    }
}