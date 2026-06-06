using MemoryPack;

namespace Network.Extensions
{
    public static class ReaderUtils5 
    {
        public static T[] ReadArray<T>(this IReader @this) where T : IMemoryPackable<T>
        {
            var bytes = @this.ReadArray<byte>();
            return MemoryPackSerializer.Deserialize<T[]>(bytes);
        }
    }
}