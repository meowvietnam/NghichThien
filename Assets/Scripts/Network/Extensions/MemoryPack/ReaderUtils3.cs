using System;
using MemoryPack;

namespace Network.Extensions.MemoryPack
{
    public static class ReaderUtils3
    {
        public static T MemRead<T>(this IReader @this)
        {
            var bytes = @this.ReadArray<byte>();
            try
            {
                return MemoryPackSerializer.Deserialize<T>(bytes);
            }
            catch (MemoryPackSerializationException e)
            {
                // Chẩn đoán lệch dữ liệu client/server: in nguyên payload (base64) để giải mã lại ngoài game.
                const int maxDump = 256 * 1024;
                var dump = bytes.Length <= maxDump ? bytes : bytes.AsSpan(0, maxDump).ToArray();
                UnityEngine.Debug.LogError(
                    $"[MemRead] {typeof(T).FullName} lỗi ({e.Message}). Payload {bytes.Length} bytes" +
                    $"{(bytes.Length > maxDump ? $" (cắt còn {maxDump})" : string.Empty)}:\n" +
                    Convert.ToBase64String(dump));
                throw;
            }
        }
    }
}
