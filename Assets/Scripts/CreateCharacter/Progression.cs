using System;
using MemoryPack;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial class Progression
    {
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public int CurrentExp { get; set; }
        [field: SerializeField] public int CurrentEnergy { get; set; }
        [field: SerializeField] public int NgoDaoDiem { get; set; }
        [field: SerializeField] public int IdCurrentNode { get; set; }
        [field: SerializeField] public int MaxLevel { get; set; }

    }
}
