using Base;
using MemoryPack;
using System.Collections;
using UnityEngine;

namespace Network.Models
{
    [MemoryPackable]
    public partial class JoinMonPhaiResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public EMonPhai TypeMonPhai { get; set; }
    }
}