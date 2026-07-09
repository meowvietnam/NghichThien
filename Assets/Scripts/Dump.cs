#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base;
using CreateCharacter;
using LuyenDan;
using LuyenKhi;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace DumpGame
{
    public class SpriteInfo
    {
        public string name;
        public string texturePath;
    }

    public class SpriteRes
    {
        public string name;
        public string path;
        public Dictionary<string, SpriteInfo[]> sprites = new Dictionary<string, SpriteInfo[]>();
    }

    public class Dump : MonoBehaviour
    {
        [SerializeField] private List<BaseData> datas;
        [SerializeField] private SkinResources skinResources;
        [SerializeField] private CoreConfig coreConfig;
        [SerializeField] private TextAsset textAsset;

        [Button]
        public void GetDatas()
        {
            var guids = AssetDatabase.FindAssets("t:BaseData");
            datas = guids.Select(x => AssetDatabase.LoadAssetAtPath<BaseData>(AssetDatabase.GUIDToAssetPath(x)))
                .ToList();
        }

        public SpriteInfo GetSpriteInfo(Sprite sprite)
        {
            return new SpriteInfo()
            {
                name = sprite.name,
                texturePath = AssetDatabase.GetAssetPath(sprite.texture)
            };
        }

        public Sprite GetOriginalSprite(SpriteInfo info)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(info.texturePath);
            var sprites = AssetOptimize.GetOriginalSprites(texture);
            return sprites.FirstOrDefault(x => x.name == info.name);
        }

        [Button]
        public void Get()
        {
            var list = JsonConvert.DeserializeObject<List<SpriteRes>>(textAsset.text);
            foreach (var baseData in datas)
            {
                var res = list.FirstOrDefault(x => x.name == baseData.name);
                var spriteItem = res.sprites["spriteItem"].First();
                var sprite = GetOriginalSprite(spriteItem);
                baseData.spriteItem = sprite;
                EditorUtility.SetDirty(baseData);
            }

            var coreRes = list.FirstOrDefault(x => x.name == "CoreConfig");
            for (var i = 0; i < coreConfig.Qualities.Count; i++)
            {
                var quality = coreConfig.Qualities[i];
                var backgroundIcon = GetOriginalSprite(coreRes.sprites[$"{i}.Background"].First());
                var iconUp = GetOriginalSprite(coreRes.sprites[$"{i}.IconUp"].First());
                quality.Background = backgroundIcon;
                quality.IconUp = iconUp;
            }

            EditorUtility.SetDirty(coreConfig);
            var skinRes = list.FirstOrDefault(x => x.name == "SkinResources");
            foreach (var s in skinResources.GenderSkinGroups)
            {
                var gender = s.Gender.ToString();
                var skins = s.SkinGroups
                    .SelectMany(x => x.Categories)
                    .SelectMany(x => x.Skins)
                    .ToList();
                foreach (var skinData in skins)
                {
                    var key = $"{gender}.{skinData.Name}";
                    var icon = GetOriginalSprite(skinRes.sprites[key].First());
                    skinData.Icon =  icon;
                }
            }
            EditorUtility.SetDirty(skinResources);
            AssetDatabase.SaveAssets();
            Debug.Log("Completed");
        }
    }
}
#endif