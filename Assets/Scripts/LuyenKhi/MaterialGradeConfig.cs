using System;
using Base;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class MaterialGradeConfig
    {
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public int Grade { get; private set; }
    }
}