using System.Collections.Generic;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class MemoryUtils1
    {
        public static int ByteSize<T>(this T @this) where T: IMemoryPackable<T>
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