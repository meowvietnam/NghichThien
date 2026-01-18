using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [Serializable]
    public class StatModifier
    {
        public StatModifier(StatType type, Action action, float value, int cost)
        {
            Type = type;
            Action = action;
            Value = value;
            Cost = cost;
        }
        [field: SerializeField] public StatType Type { get; private set; }
        [field: SerializeField] public Action Action { get; private set; }
        [field: SerializeField] public float Value { get; set; }
        [field: SerializeField] public int Cost { get; private set; }
        [ShowInInspector, ShowIf(nameof(IsLinhCan))] public LinhCan LinhCan => (LinhCan)((int)Value);
        [ShowInInspector, ShowIf(nameof(IsAbility))] public Ability Ability => (Ability)((int)Value);

        public bool IsLinhCan()
        {
            return Type == StatType.LinhCan;
        }
        public bool IsAbility()
        {
            return Type == StatType.Ability;
        }
    }
}