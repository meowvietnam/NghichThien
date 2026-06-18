using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class ReaderUtils3
    {
        public static T MemRead<T>(this IReader @this)
        {
            var bytes = @this.ReadArray<byte>();
            return MemoryPackSerializer.Deserialize<T>(bytes);
        }
    }
}