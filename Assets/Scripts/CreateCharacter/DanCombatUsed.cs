using Network;
using Network.Extensions;

namespace CreateCharacter
{
    public record DanCombatUsed : IMemorySerializable<DanCombatUsed>
    {
        public ushort Id { get; set; }
        public int count { get; set; }

        public void Deserialize(IReader reader, ref DanCombatUsed value)
        {
            value.Id = reader.Read<ushort>();
        }
        public int ByteSize()
        {
            return Id.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
        }
    }
}