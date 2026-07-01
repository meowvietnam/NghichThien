using System.Collections.Generic;
using System.Linq;
using AllMaps;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.Core
{
    public class ShopHandler : IClientHandler
    {
        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.LOAD_SHOP, LoadShop);
            Observer.AddObserver(EventList.Network.RELOAD_SHOP, ReloadShop);
            Observer.AddObserver(EventList.Network.BUY_AND_SELL, BuyAndSell);
        }

        private async void BuyAndSell(object[] data)
        {
            var callback = data[0] as INetworkEvent.IBuyAndSell;
            var sellItems = data[1] as List<ItemRef>;
            var buyItems = data[2] as List<ItemRef>;
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
                    callback.SuccessBuyAndSell();
                }
                else
                {
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
            var result = reader.MemRead<ReloadShopResult>();
            if (result.Success)
            {
                shopManager.SetShop(result.Shop);
            }
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    callback.SuccessLoadShop(result.Shop.Items.ToList());
                }
                else
                {
                    callback.FailLoadShop(result.Message);
                }
            });
        }
        private async void ReloadShop(object[] data)
        {
            var callback = data[0] as INetworkEvent.IReloadShop;
            var shopType = (EShopType)data[1];
            var client = Client.Instance;
            var shopManager = client.CharacterData.ShopManager;
            var reader = await client.RequestWait(MessageTag.ReloadShopRequest, shopType);
            var shop = reader.MemRead<Shop>();
            shopManager.SetShop(shop);
            NetworkManager.Instance.Invoke(() =>
            {
                callback.SuccessReloadShop(shop.Items.ToList());
            });
        }
        
        public void Handle(Message<MessageTag> message)
        {
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] hanlers = new MessageTag[0];
        public MessageTag[] HandleTag => hanlers;
    }
}