using System;
using System.Collections.Generic;
using MemoryPack;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial class Appearance
    {
        [ShowInInspector] public Dictionary<string, string> Skins { get; set; } = new();
        [ShowInInspector] public Dictionary<string, Color> Colors { get; set; } = new();
    }
}