using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [CreateAssetMenu(menuName = "Create Character/CharStats/Elemental Step Data", fileName = "ElementalStepData")]
    public class ElementalStepData : StepData, IMemorySerializable<ElementalStepData>
    {
        [field: SerializeField] public int GroupId { get; private set; }
        [field: SerializeField] public List<Elemental> Elementals { get; private set; }
        [SerializeField] private ElementalStep elementalStepPrefab;

        public override Step CreateUI(SelectStats selectStats, Transform parent)
        {
            var elementalStep = Instantiate(elementalStepPrefab, parent);
            elementalStep.Init(selectStats, this);
            return elementalStep;
        }

        public override int ByteSize()
        {
            return StepType.ByteSize() + Id.ByteSize() + Title.ByteSize() + Description.ByteSize() +
                   GroupId.ByteSize() + Elementals.ByteSize();
        }

        public override void Serialize(IWriter writer)
        {
            writer.Write(StepType);
            writer.Write(Id);
            writer.Write(Title);
            writer.Write(Description);
            writer.Write(GroupId);
            writer.WriteArray(Elementals);
        }

        public void Deserialize(IReader reader, ref ElementalStepData value)
        {
            value.StepType = StepType.Elemental;
            value.Id = reader.Read<int>();
            value.Title = reader.Read();
            value.Description = reader.Read();
            value.GroupId = reader.Read<int>();
            value.Elementals = reader.ReadArray<Elemental>().ToList();
        }
    }
}