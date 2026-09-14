using UnityEngine;

namespace AllMaps.DauTruong
{
    [CreateAssetMenu(fileName = "ArenaResources")]
    public class ArenaResources : ScriptableObject
    {
        [SerializeField] public Sprite[] spriteRefRank;
        [SerializeField] public Sprite[] spriteRefRankName;
        private static ArenaResources instance;

        public static ArenaResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ArenaResources>("ArenaResources");
                }
                return instance;
            }
        }
    }
}