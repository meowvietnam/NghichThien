using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class EquipmentIcon
    {
        [field: SerializeField] public ArtifactType Type { get; private set; }
        [field: SerializeField, PreviewField(height: 50)] public Sprite Icon { get; private set; }
    }
}