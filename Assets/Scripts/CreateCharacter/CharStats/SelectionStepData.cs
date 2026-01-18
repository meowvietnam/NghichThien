using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [CreateAssetMenu(menuName =  "Create Character/CharStats/Selection Step Data", fileName =  "SelectionStepData")]
    public class SelectionStepData : StepData
    {
        [field: SerializeField] public List<StatOptionGroup> Groups { get; private set; }
        [SerializeField] private SelectionStep selectionStepPrefab;
        public override Step CreateUI(SelectStats selectStats, Transform parent)
        {
            var selectionStep = Instantiate(selectionStepPrefab, parent);
            selectionStep.Init(selectStats,this);
            return selectionStep;
        }
    }
}
