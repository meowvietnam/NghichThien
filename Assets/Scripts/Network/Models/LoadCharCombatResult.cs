using MemoryPack;
using Network.Extensions;
using System;

namespace Network.Models
{
    [MemoryPackable]
    public partial class LoadCharCombatResult 
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public CharacterSnapshot Client { get; set; }
        public CharacterSnapshot Enemy { get; set; }

    }
}