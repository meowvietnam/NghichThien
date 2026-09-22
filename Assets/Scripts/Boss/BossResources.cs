using System;
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
        [field: SerializeField] public BossData[] BossDatas { get; private set; }
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
        public void Export()
        {
            if (BossDatas == null || BossDatas.Length == 0 ||
                Array.Exists(BossDatas, boss => boss?.Character == null || boss.Character.SinhMenh <= 0))
                throw new InvalidOperationException("BossDatas cần ít nhất một boss có Character và SinhMenh hợp lệ.");
            var bytes = MemoryPackSerializer.Serialize(BossDatas);
            var path = Path.Combine(Application.dataPath, "Exports", "BossResources.bin");
            File.WriteAllBytes(path, bytes);
            Debug.Log(path);
        }
    }
}