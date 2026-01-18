using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [CreateAssetMenu]
    public class StepConfig : ScriptableObject
    {
        [field: SerializeField] public List<StepData> StepDatas { get; private set; }
    }
}
