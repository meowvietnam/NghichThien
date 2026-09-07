using System.Collections.Generic;
using System.Linq;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class MemoryUtils1
    {
        public static int ByteSize<T>(this T @this) where T : IMemoryPackable<T>
        {
            var bytes = MemoryPackSerializer.Serialize(@this);
            return bytes.ByteSize();
        }

        public static int ByteSize<T>(this IEnumerable<T> @this) where T : IMemoryPackable<T>
        {
            var arr = @this as T[] ?? @this.ToArray();
            var bytes = MemoryPackSerializer.Serialize(arr);
            return bytes.ByteSize();
        }
    }
}