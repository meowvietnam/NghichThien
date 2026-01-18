using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [Serializable]
    public class Elemental
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public ElementalType ElementalType { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite SelectSprite { get; private set; }
        [ShowInInspector] public StatType StatType => (StatType)(ElementalType + 1);
        
    }
}