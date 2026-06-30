using System;
using MemoryPack;

namespace CreateCharacter
{
    [MemoryPackable]
    [Serializable]
    public partial class ArenaManager
    {
        public int CurrentArenaCount { get; set; }
        public int Points { get; set; }
    }
}