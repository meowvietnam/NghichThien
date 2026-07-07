using CreateCharacter;
using Network.Callback;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.Core
{
    public class AdsHandler : IClientHandler
    {
        private IAdsCallback adsMoneyVipCallback;

        public AdsHandler()
        {
        }

        public void SetCallback(IAdsCallback adsCallback)
        {
            adsMoneyVipCallback = adsCallback;
        }
        public async void GetAdsMoneyVipConfig()
        {
            var reader = await Client.Instance.RequestWait(MessageTag.GetAdsMoneyVipConfigRequest);
            var config = reader.Read<AdsMoneyVipConfig>();
            NetworkManager.Instance.Invoke(() =>
            {
                adsMoneyVipCallback?.Init(config);
            });
        }
        
        public void ClaimMoneyVip()
        {
            Client.Instance.Send(MessageTag.WatchAdsMoneyVip);
        }
        public void OnEnter()
        {
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            var client = Client.Instance;
            if (tag == MessageTag.ActiveAdsMoneyVip)
            {
                var active = reader.Read<bool>();
                NetworkManager.Instance.Invoke(() =>
                {
                    adsMoneyVipCallback?.ActiveAds(active);
                });
            }
            else if (tag == MessageTag.WatchAdsMoneyVipResult)
            {
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
                        adsMoneyVipCallback?.SuccessClaim(result);
                    }
                    else
                    {
                        adsMoneyVipCallback?.FailureClaim(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.WatchAds)
            {
                var showAds = reader.MemRead<ShowAds>();
                NetworkManager.Instance.Invoke(() =>
                {
                    showAds.Show();
                });
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static MessageTag[] handler = { 
            MessageTag.ActiveAdsMoneyVip, 
            MessageTag.WatchAdsMoneyVipResult,
            MessageTag.WatchAds
        };
        public MessageTag[] HandleTag => handler;
    }
}