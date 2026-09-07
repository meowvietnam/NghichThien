using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class RequesterUtils3
    {
        public static async Task<IReader> MemRequestWait<T, V>(this IRequester<T> @this, T tag, V body,
            int timeoutMilliseconds = 20000)
            where T : unmanaged, Enum
        {
            var task = new TaskCompletionSource<IReader>();

            void OnResponse(IReader reader)
            {
                task.TrySetResult(reader);
            }

            void OnError(Exception ex)
            {
                task.TrySetException(ex);
            }

            @this.AddRequest(tag, OnResponse, timeoutMilliseconds, OnError);
            @this.MemSend(tag, body);
            return await task.Task;
        }
    }
}