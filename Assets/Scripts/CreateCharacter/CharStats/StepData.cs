using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [Serializable]
    public abstract class StepData : ScriptableObject, IMemorySerializable
    {
        [field: SerializeField] public StepType StepType { get; protected set; }
        [field: SerializeField] public int Id { get; protected set; }
        [field: SerializeField] public string Title { get; protected set; }
        [field: SerializeField, TextArea] public string Description { get; protected set; }
        public abstract Step CreateUI(SelectStats selectStats, Transform parent);
        public abstract int ByteSize();
        public abstract void Serialize(IWriter writer);
    }
}