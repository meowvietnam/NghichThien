using System.IO;
using MemoryPack;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Boss
{
    [CreateAssetMenu(fileName = "BossResources")]
    public class BossResources : ScriptableObject
    {
        [field: SerializeField] public BossData[] BossDatas { get; set; }
        private static BossResources instance;

        public static BossResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<BossResources>("BossResources");
                }
                return instance;
            }
        }

        [Button]
        public void Sync()
        {
            foreach (var bossData in BossDatas)
            {
                bossData.Sync();
            }
        }
        [Button]
        public void Export()
        {
            var bytes = MemoryPackSerializer.Serialize(BossDatas);
            var path = Path.Combine(Application.dataPath, "Exports", "BossResources.bin");
            File.WriteAllBytes(path, bytes);
            Debug.Log(path);
        }
    }
}