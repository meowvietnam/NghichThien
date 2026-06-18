using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class ReaderUtils1
    {
        public static T Read<T>(this IReader @this) where T : IMemoryPackable<T>
        {
            var bytes = @this.ReadArray<byte>();
            return MemoryPackSerializer.Deserialize<T>(bytes);
        }
    }
}