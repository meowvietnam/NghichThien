using Base;
using CreateCharacter;
using MemoryPack;
using Network.Extensions;
using System.Collections.Generic;

namespace Network.Models
{
    public class IAPResult : IMemorySerializable<IAPResult>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ProductId { get; set; }

        public RewardSnapshot RewardSnapshot { get; set; }

        public int ByteSize()
        {
            return Success.ByteSize() + Message.ByteSize() + ProductId.ByteSize() + RewardSnapshot.ByteSize();
        }

        public void Deserialize(IReader reader, ref IAPResult value)
        {
            value.Success = reader.Read<bool>();
            value.Message = reader.Read();
            value.ProductId = reader.Read();
            value.RewardSnapshot = reader.Read<RewardSnapshot>();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Success);
            writer.Write(Message);
            writer.Write(ProductId);
            writer.Write(RewardSnapshot);
        }
    }
}