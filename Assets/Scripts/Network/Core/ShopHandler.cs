using System.Collections.Generic;
using System.Linq;
using AllMaps;
using LongDH.LongDHModules;
using LuyenDan;
using Network.Extensions;
using Network.Extensions.MemoryPack;

namespace Network.Core
{
    public class ShopHandler : IClientHandler
    {
        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.LOAD_SHOP, LoadShop);
            Observer.AddObserver(EventList.Network.RELOAD_SHOP, ReloadShop);
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
            NetworkManager.Instance.Invoke(() =>
            {
                callback.SuccessLoadShop(shop.Items.ToList());
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