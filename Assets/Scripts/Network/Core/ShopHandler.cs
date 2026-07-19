using System.Collections.Generic;
using System.Linq;
using AllMaps;
using AllMaps.ItemProvider;
using Base;
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
            var monPhai = (EMonPhai)data[4];
            var client = Client.Instance;
            var request = new BuyAndSell()
            {
                BuyItems = buyItems,
                SellItems = sellItems,
                ShopType = shopType,
                MonPhai = monPhai,
            };
            var reader = await client.RequestWait(MessageTag.BuyAndSellRequest, request);
            var result = reader.MemRead<BuyAndSellResult>();
            var shopManager = client.CharacterData.ShopManager;
            var inventory = client.CharacterData.InventoryManager;
            if (result.Success)
            {
                var shop = result.ReloadShop;
                shopManager.SetShop(shop, result.MonPhai);
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
            var monPhai = (EMonPhai)data[2];
            Debug.Log(monPhai);
            var request = new LoadShopRequest()
            {
                ShopType = shopType,
                MonPhai = monPhai,
            };
            var client = Client.Instance;
            var shopManager = client.CharacterData.ShopManager;
            var reader = await client.MemRequestWait(MessageTag.LoadShopRequest, request);
            var result = reader.MemRead<LoadShopResult>();
            if (result.Success)
            {
                shopManager.SetShop(result.Shop, result.MonPhai);
            }

            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    callback?.SuccessLoadShop(result.Shop.Items.ToList());
                }
                else
                {
                    callback?.FailLoadShop(result.Message);
                }
            });
        }

        private void ReloadShop(object[] data)
        {
            reloadShopCallback = data[0] as INetworkEvent.IReloadShop;
            var shopType = (EShopType)data[1];
            var monPhai = (EMonPhai)data[2];
            var request = new ReloadShopRequest()
            {
                ShopType = shopType,
                MonPhai = monPhai,
            };
            Client.Instance.MemSend(MessageTag.ReloadShop, request);
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
                    shopManager.SetShop(result.Shop, result.MonPhai);
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
            Observer.RemoveObserver(EventList.Network.LOAD_SHOP, LoadShop);
            Observer.RemoveObserver(EventList.Network.RELOAD_SHOP, ReloadShop);
            Observer.RemoveObserver(EventList.Network.BUY_AND_SELL, BuyAndSell);
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] hanlers = { MessageTag.ReloadShopResult };
        public MessageTag[] HandleTag => hanlers;
    }
}