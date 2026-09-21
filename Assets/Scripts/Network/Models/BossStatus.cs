using MemoryPack;

namespace Network.Models
{
[MemoryPackable]
public partial class BossStatus
{
    public bool Success { get; set; }
    public int RequestNodeId { get; set; }
    public long SpawnId { get; set; }
    public int NodeId { get; set; }
    public ETypeVisualChar SkinId { get; set; }
    public CharacterSnapshot Boss { get; set; }
    public bool IsAlive { get; set; }
    public int FightingCount { get; set; }
    public int RemainingAttempts { get; set; }
}
}
