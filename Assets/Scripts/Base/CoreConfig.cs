using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuyenDan;
using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base
{
    [MemoryPackable]
    public partial class CoreResource
    {
        public List<Quality> Qualities { get; private set; }

        public CoreResource(List<Quality> qualities)
        {
            Qualities = qualities;
        }
    }
    [CreateAssetMenu]
    public class CoreConfig : ScriptableObject
    {
        [field: SerializeField] public List<Quality> Qualities { get; private set; }
        private static CoreConfig instance;
        public static CoreConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<CoreConfig>("Core/CoreConfig");
                }
                return instance;
            }
        }
        public Quality FindQuality(Tier tier)
        {
            return Qualities.FirstOrDefault(x => x.Tier == tier);
        }

        [Button]
        public void Export()
        {
            var coreResource = new CoreResource(Qualities);
            var bytes = MemoryPackSerializer.Serialize(coreResource);
            var path = Path.Combine(Application.dataPath, "Exports", "core-config.bin");
            File.WriteAllBytes(path, bytes);
            Debug.Log(path);
        }
    }
}
