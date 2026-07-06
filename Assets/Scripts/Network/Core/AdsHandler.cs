using CreateCharacter;
using Network.Callback;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.Core
{
    public class AdsHandler : IClientHandler
    {
        private IAdsCallback adsMoneyVipCallback;

        public AdsHandler(IAdsCallback adsMoneyVipCallback)
        {
            this.adsMoneyVipCallback = adsMoneyVipCallback;
        }
        public async void GetAdsMoneyVipConfig()
        {
            var reader = await Client.Instance.RequestWait(MessageTag.GetAdsMoneyVipConfigRequest);
            var config = reader.Read<AdsMoneyVipConfig>();
            NetworkManager.Instance.Invoke(() =>
            {
                adsMoneyVipCallback.Init(config);
            });
        }
        
        public void ClaimMoneyVip()
        {
            var charData = Client.Instance.CharacterData;
            if (!charData.IsNoAds)
            {
                AdsService.Instance.ShowRewardVideoAds(
                    "MoneyVipAds",
                    "Xem quảng cáo để nhận tiên ngọc",
                    SendWatchAdsMoneyVip,
                    null,
                    () => NotifyGroup.Instance.Show("Không thể tải quảng cáo")
                );
            }
            else
            {
                SendWatchAdsMoneyVip();
            }
        }

        private async void SendWatchAdsMoneyVip()
        {
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.WatchAdsMoneyVipRequest);
            var result = reader.MemRead<WatchAdsMoneyVipResult>();
            if (result.Success)
            {
                var charData = client.CharacterData;
                charData.MoneyVip += result.MoneyVip;
                charData.CurrentFreeAdsMoneyVipCount = result.Count;
            }

            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    adsMoneyVipCallback.SuccessClaim(result);
                }
                else
                {
                    adsMoneyVipCallback.FailureClaim(result.Message);
                }
            });
        }
        public void OnEnter()
        {
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.ActiveAdsMoneyVip)
            {
                var active = reader.Read<bool>();
                NetworkManager.Instance.Invoke(() =>
                {
                    adsMoneyVipCallback.ActiveAds(active);
                });
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static MessageTag[] handler = { MessageTag.ActiveAdsMoneyVip };
        public MessageTag[] HandleTag => handler;
    }
}