using System;

namespace Network.Extensions.MemoryPack
{
    public static class SendActionUtils1
    {
        private class MemSendAction<T, V> : ISendActionResult
            where V : unmanaged, Enum
        {
            public ISender<V> Sender { get; private set; }
            public V Tag { get; private set; }
            public T Result { get; private set; }

            public MemSendAction(ISender<V> sender, V tag, T result)
            {
                Sender = sender;
                Tag = tag;
                Result = result;
            }

            public void Execute()
            {
                Sender.MemSend(Tag, Result);
            }
        }

        public static ISendActionResult MemSend<T, V>(this IMessageController<V> @this, V tag, T result)
            where V : unmanaged, Enum
        {
            var sender = @this.GetSender();
            return new MemSendAction<T, V>(sender, tag, result);
        }
    }
}