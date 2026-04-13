using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using UnityEngine;

namespace Base
{
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
    }
}
