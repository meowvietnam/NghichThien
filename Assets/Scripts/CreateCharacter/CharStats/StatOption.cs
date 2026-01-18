using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [Serializable]
    public class StatOption
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<StatModifier> Modifiers { get; private set; }
        [field: SerializeField, TextArea] public string Effective { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
    }
}