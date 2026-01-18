using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [CreateAssetMenu(menuName =  "Create Character/CharStats/Elemental Step Data", fileName =  "ElementalStepData")]
    public class ElementalStepData : StepData
    {
        [field: SerializeField] public int GroupId { get; private set; }
        [field: SerializeField] public List<Elemental> Elementals { get; private set; }
        [SerializeField] private ElementalStep elementalStepPrefab;
        public override Step CreateUI(SelectStats selectStats, Transform parent)
        {
            var elementalStep = Instantiate(elementalStepPrefab, parent);
            elementalStep.Init(selectStats,this);
            return elementalStep;
        }
    }
}