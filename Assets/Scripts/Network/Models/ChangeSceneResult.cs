using MemoryPack;
using Network.Extensions;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ChangeSceneResult
    {
        public bool Success { get; set; }
        public string SceneName { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Success} - {SceneName} - {Message}";
        }
    }
}