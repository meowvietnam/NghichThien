using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class RequesterUtils1
    {
        public static async Task<IReader> RequestWait<T, V>(this IRequester<T> @this, T tag, IEnumerable<V> body,
            int timeoutMilliseconds = 20000)
            where T : unmanaged, Enum
            where V : IMemoryPackable<V>
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
            @this.Send(tag, body);
            return await task.Task;
        }

        public static async Task<IReader> RequestWait<T, V>(this IRequester<T> @this, T tag, V body,
            int timeoutMilliseconds = 20000)
            where T : unmanaged, Enum
            where V : IMemoryPackable<V>
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
            @this.Send(tag, body);
            return await task.Task;
        }
    }
}