using MemoryPack;
using UnityEngine;

namespace MainMenu
{
    [MemoryPackable]
    public partial class LocalAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}