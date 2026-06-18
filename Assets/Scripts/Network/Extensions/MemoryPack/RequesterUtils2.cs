using System;
using System.Collections.Generic;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class RequesterUtils2
    {
        public static void Request<T, V>(this IRequester<T> @this, T tag, V body, Action<IReader> response, int timeoutMilliseconds = 20000)
            where T : unmanaged, Enum
            where V : IMemoryPackable<V>
        {
            @this.AddRequest(tag, response, timeoutMilliseconds);
            @this.Send(tag, body);
        }
        public static void Request<T, V>(this IRequester<T> @this, T tag, IEnumerable<V> body, Action<IReader> response, int timeoutMilliseconds = 20000)
            where T : unmanaged, Enum
            where V : IMemoryPackable<V>
        {
            @this.AddRequest(tag, response, timeoutMilliseconds);
            @this.Send(tag, body);
        }
    }
}