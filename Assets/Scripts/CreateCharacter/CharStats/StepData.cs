using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [Serializable]
    public abstract class StepData : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Title { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        public abstract Step CreateUI(SelectStats selectStats,Transform parent);
    }
}