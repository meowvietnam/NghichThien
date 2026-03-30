using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [CreateAssetMenu(menuName =  "Create Character/Skin Resources",fileName = "SkinResources")]
    public class SkinResources : ScriptableObject, IMemorySerializable<SkinResources>
    {
        [field: SerializeField] public List<Sprite> LevelIcons { get; private set; }
        [field: SerializeField] public List<Color> Colors { get; private set; }
        [field: SerializeField] public List<GenderSkinGroup> GenderSkinGroups { get; private set; }
        public static SkinResources Instance
        {
            get
            {
                return Resources.FindObjectsOfTypeAll<SkinResources>().FirstOrDefault();
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
    }
}
