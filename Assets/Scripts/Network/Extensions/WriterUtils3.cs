using System.Collections.Generic;
using MemoryPack;

namespace Network.Extensions
{
    public static class WriterUtils3
    {
        public static void Write<T>(this IWriter @this, T value) where T : IMemoryPackable<T>
        {
            var bytes = MemoryPackSerializer.Serialize(value);
            @this.WriteArray(bytes);
        }

        public static void WriteArray<T>(this IWriter @this, IEnumerable<T> value) where T : IMemoryPackable<T>
        {
            var bytes = MemoryPackSerializer.Serialize(value);
            @this.WriteArray(bytes);
        }
    }
}