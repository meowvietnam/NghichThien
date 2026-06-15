using System.Collections.Generic;
using System.Linq;
using MemoryPack;

namespace Network.Extensions
{
    public static class MemoryUtils3
    {
        public static int ByteSize<T>(this IMemoryPackable<T> @this)
        {
            var bytes = MemoryPackSerializer.Serialize(@this);
            return bytes.ByteSize();
        }
        public static int ByteSize<T>(this IEnumerable<T> @this) where T : IMemoryPackable<T>
        {
            var bytes = MemoryPackSerializer.Serialize(@this);
            return bytes.ByteSize();
        }
    }
}