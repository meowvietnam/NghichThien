using Network.Extensions;

namespace Network.Models
{
    public class MoveToMapResult : IMemorySerializable<MoveToMapResult>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Energy { get; set; }
        public int[] Path { get; set; }
        public void Deserialize(IReader reader, ref MoveToMapResult value)
        {
            value.Success = reader.Read<bool>();
            if (!value.Success)
            {
                value.Message = reader.Read();
            }
            else
            {
                value.Energy = reader.Read<int>();
                value.Path = reader.ReadArray<int>();
            }
        }
        public int ByteSize()
        {
            if (Success)
            {
                return Success.ByteSize() + Energy.ByteSize() + Path.ByteSize();
            }
            else
            {
                return Success.ByteSize() + Message.ByteSize();
            }
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Success);
            if (!Success)
            {
                writer.Write(Message);
            }
            else
            {
                writer.Write(Energy);
                writer.WriteArray(Path);
            }
        }
    }
}