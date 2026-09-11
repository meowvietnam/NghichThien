using System;

namespace Network.Extensions.MemoryPack
{
    public static class RequesterUtils4
    {
        public static void Request<T, V>(this IRequester<T> @this, T tag, V body, Action<IReader> response,
            int timeoutMilliseconds = 20000)
            where T : unmanaged, Enum
        {
            @this.AddRequest(tag, response, timeoutMilliseconds);
            @this.MemSend(tag, body);
        }
    }
}