using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class WriterUtils2
    {
        public static void MemWrite<T>(this IWriter @this, T value)
        {
            var bytes = MemoryPackSerializer.Serialize(value);
            @this.WriteArray(bytes);
        }
    }
}