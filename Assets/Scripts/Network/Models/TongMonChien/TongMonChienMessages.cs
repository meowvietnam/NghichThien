using System;
using Base;
using CreateCharacter;
using MemoryPack;

namespace Network.Models
{
    // Bản sao hợp đồng message Tông môn chiến của server (MTS/Network/Models/TongMonChienMessages.cs).
    // MemoryPack serialize theo thứ tự field: KHÔNG đổi thứ tự/kiểu nếu không đổi đồng thời ở server.

    public enum WarUnitState : byte
    {
        Idle,
        Moving,
        Fighting,
        Holding
    }

    public enum WarUnitKind : byte
    {
        Player,
        Puppet
    }

    public enum WarSideResult : byte
    {
        Win,
        LoseDied,
        LoseRun,
        Draw
    }

    [MemoryPackable]
    public partial class WarClanView
    {
        public int ClanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; }
        public int MainNodeId { get; set; }
        /// <summary>Số thành đã chiếm: node Stronghold tông đang giữ (không gồm thành chính).</summary>
        public int StrongholdCount { get; set; }
        /// <summary>Số điểm đã chiếm: node thường tông đang giữ.</summary>
        public int NodeCount { get; set; }
    }

    [MemoryPackable]
    public partial class WarNodeView
    {
        public int NodeId { get; set; }
        public int? OwnerClanId { get; set; }
        public bool IsFighting { get; set; }
    }

    [MemoryPackable]
    public partial class WarUnitRef
    {
        public WarUnitKind Kind { get; set; }
        public int Id { get; set; }
    }

    [MemoryPackable]
    public partial class WarUnitView
    {
        public WarUnitKind Kind { get; set; }
        public int Id { get; set; }
        public int ClanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int NodeId { get; set; }
        public int Hp { get; set; }
        public int HpMax { get; set; }
        public float RegenPerMinute { get; set; }
        public DateTime HpUpdatedAt { get; set; }
        public WarUnitState State { get; set; }
        public int[] Route { get; set; } = new int[0];
        public DateTime? NextArrivalAt { get; set; }
        public bool IsActive { get; set; }
        public Gender Gender { get; set; }
        public Appearance Appearance { get; set; }
        /// <summary>Độ bền còn lại của rối (số lần còn được chết). 0 với người chơi.</summary>
        public int Durability { get; set; }
        public int MaxDurability { get; set; }

        /// <summary>HP hiện tại ngoại suy theo nhịp hồi (serverNow = giờ server ước lượng).</summary>
        public int GetHp(DateTime serverNow)
        {
            if (Hp >= HpMax || RegenPerMinute <= 0) return Hp;
            var minutes = (serverNow - HpUpdatedAt).TotalMinutes;
            if (minutes <= 0) return Hp;
            return Math.Min(HpMax, Hp + (int)Math.Floor(minutes * RegenPerMinute));
        }
    }

    [MemoryPackable]
    public partial class WarGetSnapshotRequest
    {
    }

    [MemoryPackable]
    public partial class WarSnapshot : Result
    {
        public long Version { get; set; }
        public float SecondsPerEdge { get; set; }
        public DateTime ServerTime { get; set; }
        public int MyCharId { get; set; }
        public int MyClanId { get; set; }
        public bool IsLeader { get; set; }
        public int MaxPuppetCount { get; set; }
        public int MaxPuppetLevel { get; set; }
        public WarClanView[] Clans { get; set; } = new WarClanView[0];
        public WarNodeView[] Nodes { get; set; } = new WarNodeView[0];
        public WarUnitView[] Units { get; set; } = new WarUnitView[0];
        /// <summary>Tiên ngọc/ngày cho mỗi thành (Stronghold) tông đang giữ.</summary>
        public int StrongholdMoneyVip { get; set; }
        /// <summary>Tiên ngọc/ngày cho mỗi điểm (node thường) tông đang giữ.</summary>
        public int NodeMoneyVip { get; set; }
        /// <summary>Thời điểm chốt phát tiên ngọc hằng ngày kế tiếp (UTC).</summary>
        public DateTime NextSettlementAt { get; set; }
        /// <summary>% tổng tiên ngọc/ngày của tông mà bản thân nhận theo chức vụ.</summary>
        public int MyRewardPercent { get; set; }
        /// <summary>Thời điểm kết thúc mùa hiện tại (UTC); hết mùa làm mới đất đã chiếm.</summary>
        public DateTime SeasonEndAt { get; set; }
        /// <summary>Số thành viên cùng nhóm chức vụ với bản thân (phần % của nhóm chia đều cho nhóm).</summary>
        public int MyRoleMemberCount { get; set; }
    }

    [MemoryPackable]
    public partial class WarDelta
    {
        public long Version { get; set; }
        public DateTime ServerTime { get; set; }
        public WarClanView[] Clans { get; set; } = new WarClanView[0];
        public WarNodeView[] Nodes { get; set; } = new WarNodeView[0];
        public WarUnitView[] Units { get; set; } = new WarUnitView[0];
        public WarUnitRef[] RemovedUnits { get; set; } = new WarUnitRef[0];
    }

    [MemoryPackable]
    public partial class WarMoveRequest
    {
        public int TargetNodeId { get; set; }
    }

    [MemoryPackable]
    public partial class WarMoveResult : Result
    {
        public int[] Route { get; set; } = new int[0];
    }

    [MemoryPackable]
    public partial class WarDeployPuppetRequest
    {
        public int PuppetId { get; set; }
        public int TargetNodeId { get; set; }
    }

    [MemoryPackable]
    public partial class WarUseHpPillRequest
    {
        public int PuppetId { get; set; }
        public ushort DanId { get; set; }
        public ushort Count { get; set; }
    }

    [MemoryPackable]
    public partial class WarUseHpPillResult : Result
    {
        public int PuppetId { get; set; }
        public int Hp { get; set; }
        public int HpMax { get; set; }
        public ushort DanId { get; set; }
        public int RemainingCount { get; set; }
    }

    [MemoryPackable]
    public partial class WarCreatePuppetRequest
    {
        /// <summary>Chi phí đã xác nhận; -1 = chỉ hỏi giá (server trả RequiresConfirmation).</summary>
        public long ExpectedContribPointsCost { get; set; } = -1;
        public long ExpectedMoneyVipCost { get; set; } = -1;
    }

    [MemoryPackable]
    public partial class WarUpgradePuppetRequest
    {
        public int PuppetId { get; set; }
        public long ExpectedContribPointsCost { get; set; } = -1;
        public long ExpectedMoneyVipCost { get; set; } = -1;
    }

    [MemoryPackable]
    public partial class WarPuppetResult : Result
    {
        public WarUnitView Puppet { get; set; }
        public bool IsUpgrade { get; set; }
        public int PuppetId { get; set; }
        /// <summary>Level rối sau khi nâng (nâng cấp) hoặc level rối mới (tạo).</summary>
        public int TargetLevel { get; set; }
        /// <summary>Cần xác nhận chi phí (lần hỏi giá đầu tiên hoặc giá đã đổi); client hiện popup rồi gửi lại.</summary>
        public bool RequiresConfirmation { get; set; }
        public long ContribPointsCost { get; set; }
        public long MoneyVipCost { get; set; }
        /// <summary>Điểm cống hiến còn lại sau khi trừ.</summary>
        public long ContribPoints { get; set; }
        public int NewMoneyVip { get; set; }
    }

    [MemoryPackable]
    public partial class WarBattleMatched
    {
        public long BattleId { get; set; }
        public int NodeId { get; set; }
        public bool IsPvp { get; set; }
        public bool EnemyIsPuppet { get; set; }
        public int EnemyPuppetId { get; set; }
        public CharacterSnapshot Me { get; set; } = new CharacterSnapshot();
        public CharacterSnapshot Enemy { get; set; } = new CharacterSnapshot();
    }

    [MemoryPackable]
    public partial class WarBattleReport
    {
        public long BattleId { get; set; }
        public ResultCombat Result { get; set; } = new ResultCombat();
        public int EnemyHp { get; set; }
    }

    [MemoryPackable]
    public partial class WarBattleEnded
    {
        public long BattleId { get; set; }
        public WarSideResult Result { get; set; }
        public int Hp { get; set; }
        public int HpMax { get; set; }
        public int NodeId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    [MemoryPackable]
    public partial class WarKicked
    {
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>Thông báo chung từ Tông môn chiến (vd. hết mùa, lãnh thổ được làm mới).</summary>
    [MemoryPackable]
    public partial class WarNotice
    {
        public string Message { get; set; } = string.Empty;
    }
}
