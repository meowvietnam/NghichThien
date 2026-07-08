using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}