using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class SenderUtils1
    {
        public static void Send<T, V>(this ISender<T> sender, T tag, V value)
            where T : unmanaged, Enum
            where V : IMemoryPackable<V>
        {
            var bytes = MemoryPackSerializer.Serialize(value);
            var header = new Header<T>(tag, bytes.ByteSize());
            var writer = new Writer(header.ByteSize() + header.Length);
            writer.Write(header);
            writer.WriteArray(bytes);
            sender.Send(writer);
        }

        public static void Send<T, V>(this ISender<T> sender, T tag, IEnumerable<V> values)
            where T : unmanaged, Enum
            where V : IMemoryPackable<V>
        {
            var memoryPackables = values as V[] ?? values.ToArray();
            var bytes = MemoryPackSerializer.Serialize(memoryPackables);
            var header = new Header<T>(tag, bytes.ByteSize());
            var writer = new Writer(header.ByteSize() + header.Length);
            writer.Write(header);
            writer.WriteArray(bytes);
            sender.Send(writer);
        }
    }
}