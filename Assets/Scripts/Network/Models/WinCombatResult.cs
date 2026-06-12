using Base;
using CreateCharacter;
using MemoryPack;
using Network.Extensions;

namespace Network.Models
{
    public class WinCombatResult : IMemorySerializable<WinCombatResult>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public RewardSnapshot Reward { get; set; }
        public int NewHP { get; set; }
        public CamNgo[] NewCamNgo { get; set; }

        public int ByteSize()
        {
            return Success.ByteSize() + Message.ByteSize() + Reward.ByteSize() + NewCamNgo.ByteSize();
        }

        public void Deserialize(IReader reader, ref WinCombatResult value)
        {
           value.NewCamNgo = reader.ReadArray<CamNgo>();
           value.NewHP = reader.Read<int>();
           value.Reward = reader.Read<RewardSnapshot>();
           value.Message = reader.Read();
           value.Success = reader.Read<bool>();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Success);
            writer.Write(Message);
            writer.Write(Reward);
            writer.WriteArray(NewCamNgo);
            writer.Write(NewHP);

        }
    }
}