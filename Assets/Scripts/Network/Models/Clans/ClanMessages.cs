using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models.Clans
{
    [MemoryPackable]
    public partial class GetClanRequest
    {
    }

    [MemoryPackable]
    public partial class GetClanResult : Result
    {
        public Clans.Clan Clan { get; set; }
        public ClanConfig Config { get; set; } = new ClanConfig();
    }

    [MemoryPackable]
    public partial class ClanConfig
    {
        public int CreateClanMoneyVipPrice { get; set; }
        public int CreateClanVndPrice { get; set; }
        public int CreateMaxMember { get; set; }
        public int ContributionMoneyVipPerTime { get; set; }
        public int MaxContributionTimesPerRequest { get; set; }
        public int MaxShopQuantityPerRequest { get; set; } = 100;
        public int MaxShopItemsAddedPerDay { get; set; } = 100;
        public Dictionary<ClanRole, ClanPermission> RolePermissions { get; set; } =
            new Dictionary<ClanRole, ClanPermission>();
    }

    [MemoryPackable]
    public partial class CreateClanRequest
    {
        public const int MaxNameLength = 15;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Announcement { get; set; } = string.Empty;
        public int NodeId { get; set; }
        public string ImageUrl { get; set; }
        public CreateClanBy CreateBy { get; set; }
    }

    [MemoryPackable]
    public partial class CreateClanResult : Result
    {
        public CreateClanBy By { get; set; }
        public int? NewMoneyVip { get; set; }
        public string Limk { get; set; }
    }

    [MemoryPackable]
    public partial class UploadImageRequest
    {
        public byte[] Data { get; set; }
    }

    [MemoryPackable]
    public partial class UploadImageResult : Result
    {
        public string Url { get; set; }
    }

    [MemoryPackable]
    public partial class AddClanMemberRequest
    {
        public int CharId { get; set; }
        public ClanRole Role { get; set; } = ClanRole.Member;
    }

    [MemoryPackable]
    public partial class AddClanMemberResult : Result
    {
        public Clans.ClanMember Member { get; set; }
    }

    [MemoryPackable]
    public partial class ClanAddMember
    {
        public Clans.ClanMember Member { get; set; }
    }

    [MemoryPackable]
    public partial class KickClanMemberRequest
    {
        public int CharId { get; set; }
    }

    [MemoryPackable]
    public partial class KickClanMemberResult : Result
    {
        public int CharId { get; set; }
    }

    [MemoryPackable]
    public partial class LeaveClanRequest
    {
    }

    [MemoryPackable]
    public partial class LeaveClanResult : Result
    {
        public int ClanId { get; set; }
    }

    [MemoryPackable]
    public partial class ClanRemoveMember
    {
        public int ClanId { get; set; }
        public int CharId { get; set; }
        public ClanRemoveMemberReason Reason { get; set; }
    }

    [MemoryPackable]
    public partial class ClanContribution
    {
        public int ClanId { get; set; }
        public int CharId { get; set; }
        public int Times { get; set; }
        public int FreeTimes { get; set; }
        public int PaidTimes { get; set; }
        public long Amount { get; set; }
        public long ClanFunds { get; set; }
        public long MemberContribPoints { get; set; }
    }

    [MemoryPackable]
    public partial class ClanContributionRequest
    {
        public int Times { get; set; }
    }

    [MemoryPackable]
    public partial class ClanContributionResult : Result
    {
        public ClanContribution Contribution { get; set; }
        public int NewMoneyVip { get; set; }
    }

    [MemoryPackable]
    public partial class SearchClanRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Limit { get; set; } = 20;
    }

    [MemoryPackable]
    public partial class SearchClanResult : Result
    {
        public Clans.Clan[] Clans { get; set; } = new Clans.Clan[0];
    }

    [MemoryPackable]
    public partial class GetClanShopRequest
    {
    }

    [MemoryPackable]
    public partial class GetClanShopResult : Result
    {
        public Clans.ClanShopItem[] Items { get; set; } = new Clans.ClanShopItem[0];
        public long ContribPoints { get; set; }
        public int MaxQuantityPerRequest { get; set; }
        public int MaxItemsAddedPerDay { get; set; }
        public int ShopItemsAddedToday { get; set; }
    }

    [MemoryPackable]
    public partial class BuyClanShopRequest
    {
        public int ShopItemId { get; set; }
        public int Quantity { get; set; }
    }

    [MemoryPackable]
    public partial class BuyClanShopResult : Result
    {
        public int ShopItemId { get; set; }
        public int Quantity { get; set; }
        public long Cost { get; set; }
        public long ContribPoints { get; set; }
        public ItemRef InventoryItem { get; set; }
    }

    [MemoryPackable]
    public partial class AddClanShopItemRequest
    {
        public ItemRef Item { get; set; } = new ItemRef();
        public long Price { get; set; }
    }

    [MemoryPackable]
    public partial class AddClanShopItemResult : Result
    {
        public Clans.ClanShopItem ShopItem { get; set; }
        public ItemRef InventoryItem { get; set; }
        public int MaxItemsAddedPerDay { get; set; }
        public int ShopItemsAddedToday { get; set; }
    }

    [MemoryPackable]
    public partial class RemoveClanShopItemRequest
    {
        public int ShopItemId { get; set; }
    }

    [MemoryPackable]
    public partial class RemoveClanShopItemResult : Result
    {
        public int ShopItemId { get; set; }
        public ItemRef InventoryItem { get; set; }
    }

    [MemoryPackable]
    public partial class ClanShopItemAdded
    {
        public Clans.ClanShopItem ShopItem { get; set; } = new Clans.ClanShopItem();
    }

    [MemoryPackable]
    public partial class ClanShopItemRemoved
    {
        public int ClanId { get; set; }
        public int ShopItemId { get; set; }
    }

    [MemoryPackable]
    public partial class ClanShopItemChanged
    {
        public Clans.ClanShopItem ShopItem { get; set; } = new Clans.ClanShopItem();
    }

    [MemoryPackable]
    public partial class ClanMemberPointsChanged
    {
        public int ClanId { get; set; }
        public int CharId { get; set; }
        public long ContribPoints { get; set; }
    }
}
