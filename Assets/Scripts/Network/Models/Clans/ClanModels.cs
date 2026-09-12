using System;
using System.Collections.Generic;
using Base;
using CreateCharacter;
using LuyenDan;
using MemoryPack;

namespace Network.Models.Clans
{
    public enum ClanRole : byte
    {
        Member,      // Đệ tử
        Elder,       // Trưởng lão
        ViceLeader,  // Phó Tông Chủ
        Leader       // Tông Chủ
    }

    [Flags]
    public enum ClanPermission : uint
    {
        None = 0,
        AddMember = 1 << 0,
        KickMember = 1 << 1,
        AddShopItem = 1 << 2,
        RemoveShopItem = 1 << 3,
        AssignRole = 1 << 4,
    }

    public enum CreateClanBy
    {
        MoneyVip,
        Vnd
    }

    public enum ClanRemoveMemberReason
    {
        Kick = 1,
        Leave = 2
    }

    [MemoryPackable]
    public partial class Clan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizeName { get; set; } = string.Empty;
        public int NodeId { get; set; }
        public string ImageUrl { get; set; }
        public long Funds { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Announcement { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public int MaxMembers { get; set; }
        public int Level { get; set; }
        public long Exp { get; set; }
        public int LeaderCharId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ClanMember> Members { get; set; } = new List<ClanMember>();
        public List<ClanJoinRequest> JoinRequests { get; set; } = new List<ClanJoinRequest>();
        public List<ClanShopItem> ShopItems { get; set; } = new List<ClanShopItem>();
    }

    [MemoryPackable]
    public partial class ClanMember
    {
        public int ClanId { get; set; }
        public int CharId { get; set; }
        public long ContribPoints { get; set; }
        public DateTime LastFreeContributionAt { get; set; }
        public int ShopItemsAddedToday { get; set; }
        public DateTime LastShopItemAddedAt { get; set; }
        public ClanRole Role { get; set; }
        public DateTime JoinAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public Gender Gender { get; set; }
        public Appearance Appearance { get; set; }
    }

    [MemoryPackable]
    public partial class ClanJoinRequest
    {
        public int CharId { get; set; }
        public int ClanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public Gender Gender { get; set; }
        public Appearance Appearance { get; set; }
        public DateTime Created { get; set; }
    }

    [MemoryPackable]
    public partial class ClanShopItem
    {
        public int Id { get; set; }
        public int ClanId { get; set; }
        public int OwnerCharId { get; set; }
        public long Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public ItemRef Item { get; set; } = new ItemRef();
    }
}
