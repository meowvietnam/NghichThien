using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [CreateAssetMenu(menuName = "Create Character/CharStats/Selection Step Data", fileName = "SelectionStepData")]
    public class SelectionStepData : StepData, IMemorySerializable<SelectionStepData>
    {
        [field: SerializeField] public List<StatOptionGroup> Groups { get; private set; }
        [SerializeField] private SelectionStep selectionStepPrefab;

        public override Step CreateUI(SelectStats selectStats, Transform parent)
        {
            var selectionStep = Instantiate(selectionStepPrefab, parent);
            selectionStep.Init(selectStats, this);
            return selectionStep;
        }

        public override int ByteSize()
        {
            return StepType.ByteSize() + Id.ByteSize() + Title.ByteSize() + Description.ByteSize() + Groups.ByteSize();
        }

        public override void Serialize(IWriter writer)
        {
            writer.Write(StepType);
            writer.Write(Id);
            writer.Write(Title);
            writer.Write(Description);
            writer.WriteArray(Groups);
        }

        public void Deserialize(IReader reader, ref SelectionStepData value)
        {
            value.StepType = StepType.Selection;
            value.Id = reader.Read<int>();
            value.Title = reader.Read();
            value.Description = reader.Read();
            value.Groups = reader.ReadArray<StatOptionGroup>().ToList();
        }
    }
}