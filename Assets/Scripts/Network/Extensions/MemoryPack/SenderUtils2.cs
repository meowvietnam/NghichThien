using System;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class SenderUtils2
    {
        public static void MemSend<T, V>(this ISender<T> sender, T tag, V value)
            where T : unmanaged, Enum
        {
            var bytes = MemoryPackSerializer.Serialize(value);
            var header = new Header<T>(tag, bytes.ByteSize());
            var writer = new Writer(header.ByteSize() + header.Length);
            writer.Write(header);
            writer.WriteArray(bytes);
            sender.Send(writer);
        }
    }
}