using System;
using System.Collections.Generic;
using UnityEngine;
namespace CreateCharacter.CharStats
{
    [Serializable]
    public class StatOptionGroup
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public List<StatOption> StatOptions { get; private set; } 
        [field: SerializeField] public List<int> Exclusives { get; private set; }
    }
}
