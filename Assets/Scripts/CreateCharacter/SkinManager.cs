using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CreateCharacter
{
    public class SkinManager : MonoBehaviour
    {
        [SerializeField] private SkinResources skinResources;

        [SerializeField] private SkeletonGraphic graphic;
        [SerializeField] private CreateAvatar createAvatar;
        private void Start()
        {
            RandomSkin();
        }
        [Button]
        private void RandomSkin()
        {
            var genderSkinGroups = skinResources.GenderSkinGroups;
            var gender = SkinGender.Male;
            var genderSkinGroup = genderSkinGroups.Find(g => g.Gender == gender);
            var combines = new List<(string, string)>();
            foreach (var skinGroup in genderSkinGroup.SkinGroups)
            {
                foreach (var category in skinGroup.Categories)
                {
                    var skins = category.Skins;
                    var randomIndex = Random.Range(0, skins.Count);
                    var skin = skins[randomIndex];
                    combines.Add(new(category.Id, skin.Name));
                }
            }
            createAvatar.SetSkins(combines, skinResources.Colors[0]);
            EditorUtility.SetDirty(graphic);
        }
    }
}
