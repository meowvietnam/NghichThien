using Base;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    public class SkinManager : MonoBehaviour
    {
        [SerializeField] private SkinResources skinResources;

        private void Start()
        {
            RandomSkin();
        }

        [Button]
        private void RandomSkin()
        {
            var genderSkinGroups = skinResources.GenderSkinGroups;
            var gender = Gender.Male;
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
        }
    }
}