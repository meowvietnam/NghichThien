using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class Reconnect
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;

        [MemoryPackConstructor]
        public Reconnect()
        {
        }

        public Reconnect(int id, string key)
        {
            Id = id;
            Key = key;
        }
    }
}