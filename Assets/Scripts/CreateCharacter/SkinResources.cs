using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    [CreateAssetMenu(menuName = "Create Character/Skin Resources", fileName = "SkinResources")]
    public class SkinResources : ScriptableObject, IMemorySerializable<SkinResources>
    {
        [field: SerializeField] public List<Sprite> LevelIcons { get; private set; }
        [field: SerializeField] public List<Color> Colors { get; private set; }
        [field: SerializeField] public List<GenderSkinGroup> GenderSkinGroups { get; private set; }
        private static SkinResources instance;

        public static SkinResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<SkinResources>("Skins/SkinResources");
                }

                return instance;
            }
        }

        public int ByteSize()
        {
            return Colors.ByteSize() + GenderSkinGroups.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(Colors);
            writer.WriteArray(GenderSkinGroups);
        }

        public void Deserialize(IReader reader, ref SkinResources value)
        {
            value.Colors = reader.ReadArray<Color>().ToList();
            value.GenderSkinGroups = reader.ReadArray<GenderSkinGroup>().ToList();
        }
#if UNITY_EDITOR
        [Button]
        public void ShowSpriteSize()
        {
            var sprites = GenderSkinGroups.SelectMany(x => x.SkinGroups)
                .SelectMany(x => x.Categories)
                .SelectMany(x => x.Skins)
                .Select(x => x.Icon)
                .Concat(LevelIcons)
                .ToHashSet();
            var size = AssetSize.CalculateSize(sprites);
            Debug.Log(size);
        }
#endif
    }
}