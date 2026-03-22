using System;
using System.Collections.Generic;
using System.IO;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [CreateAssetMenu]
    public class StepConfig : ScriptableObject , IMemorySerializable<StepConfig>
    {
        [field: SerializeField] public List<StepData> StepDatas { get; private set; }
        public int ByteSize()
        {
            return StepDatas.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(StepDatas);
        }

        public void Deserialize(IReader reader, ref StepConfig value)
        {
            var count = reader.Read<int>();
            var stepDatas = new List<StepData>(count);
            value.StepDatas = stepDatas;
            for (int i = 0; i < count; i++)
            {
                var stepType = reader.Read<StepType>();
                if (stepType == StepType.Selection)
                {
                    var step = reader.Read<SelectionStepData>();
                    stepDatas.Add(step);
                }
                else if (stepType == StepType.Elemental)
                {
                    var step = reader.Read<ElementalStepData>();
                    stepDatas.Add(step);
                }
            }
        }

        // [Button]
        // public void ToBinary()
        // {
        //     var path = Path.Combine(Application.dataPath, "Exports", "StepConfig.bin");
        //     var writer = new Writer(ByteSize());
        //     writer.Write(this);
        //     File.WriteAllBytes(path,writer.Buffer);
        // }
    }
}
