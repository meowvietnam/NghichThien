using System.Collections.Generic;
using System.Linq;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class WriterUtils1
    {
        public static void Write<T>(this IWriter @this, T value) where T : IMemoryPackable<T>
        {
            var bytes = MemoryPackSerializer.Serialize(value);
            @this.WriteArray(bytes);
        }

        public static void WriteArray<T>(this IWriter @this, IEnumerable<T> value) where T : IMemoryPackable<T>
        {
            var memoryPackables = value as T[] ?? value.ToArray();
            var bytes = MemoryPackSerializer.Serialize(memoryPackables);
            @this.WriteArray(bytes);
        }
    }
}