using MemoryPack;
using Network.Extensions;
namespace Network.Models
{
    public enum ShowAdsType
    {
        Arena,
        MoneyVip,
        ReloadTask,
        ReloadShop
    }
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(ArenaAds))]
    [MemoryPackUnion(1, typeof(MoneyVipAds))]
    [MemoryPackUnion(2, typeof(ReloadTaskAds))]
    [MemoryPackUnion(3, typeof(ReloadShopAds))]
    public abstract partial class ShowAds
    {
		public string Location { get; set; }
        public string ButtonName { get; set; }

        public virtual void OnReward()
        {
            Client.Instance.Send(MessageTag.CompleteAds, GetAdsType());
        }

        public virtual void OnClose()
        {
            Client.Instance.Send(MessageTag.CloseAds, GetAdsType());
        }

        public virtual void OnLoadFail()
        {
            Client.Instance.Send(MessageTag.LoadAdsFail, GetAdsType());
        }
        public abstract ShowAdsType GetAdsType();
        public ShowAds(string location, string buttonName)
        {
            Location = location;
            ButtonName = buttonName;
        }
        public void Show()
        {
            AdsService.Instance.ShowRewardVideoAds(Location, ButtonName, OnReward, OnClose, OnLoadFail);
        }
    }
}