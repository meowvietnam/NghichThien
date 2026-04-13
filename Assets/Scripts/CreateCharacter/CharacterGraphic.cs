using Base;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    public class CharacterGraphic : MonoBehaviour
    {
        [SerializeField] protected SkeletonGraphic graphic;
        [SerializeField] protected SkinResources skinResources;
        private Dictionary<string, string> currentSkins = new Dictionary<string, string>();
        private Dictionary<string, Color> currentColors = new Dictionary<string, Color>();
        public void SetGender(Gender gender)
        {
            var skeletonDataAsset = skinResources.GenderSkinGroups.FirstOrDefault(skinResource => skinResource.Gender == gender)?.SkeletonDataAsset;
            graphic.skeletonDataAsset = skeletonDataAsset;
            currentSkins.Clear();
            currentColors.Clear();
            graphic.Initialize(true);
            graphic.AnimationState.SetAnimation(0, "Idle_0", true);
        }
        public void SetSkin(Dictionary<string, string> combines, Dictionary<string, Color> colors = null)
        {
            var combineSkin = new Skin("character");
            var skeleton = graphic.Skeleton;
            var data = skeleton.Data;
            if(combines == null)
            {
                Debug.Log("data set skin null");
                return;
            }    
            foreach (var (skinId, skinName) in combines)
            {
                var skin = data.FindSkin(skinName);
                if (skin == null)
                {
                    Debug.LogError($"Skin not found: {skinName}");
                    continue;
                }
                currentSkins[skinId] = skinName;
                combineSkin.AddSkin(skin);
            }
            skeleton.SetSkin(combineSkin);
            skeleton.SetSlotsToSetupPose();
            graphic.Update(0);
            if (colors == null) return;
            foreach (var (skinId, color) in colors)
            {
                var skinName = currentSkins[skinId];
                var skin = data.FindSkin(skinName);
                foreach (var attachment in skin.Attachments)
                {
                    var slot = skeleton.Slots.Items[attachment.SlotIndex];
                    slot.SetColor(color);
                }
                currentColors[skinId] = color;
            }
        }
        [Button]
        public (Dictionary<string, string> skins, Dictionary<string, Color> colors) RandomSkin(Gender gender, bool useDefaultColors)
        {
            var skins = new Dictionary<string, string>();
            var colors = new Dictionary<string, Color>();
            var palette = skinResources.Colors;
            var group = skinResources.GenderSkinGroups.First(x => x.Gender == gender);
            foreach (var groupSkinGroup in group.SkinGroups)
            {
                foreach (var skinCategory in groupSkinGroup.Categories)
                {
                    var skin = skinCategory.Skins[Random.Range(0, skinCategory.Skins.Count)];
                    skins[skinCategory.Id] = skin.Name;
                }

                foreach (var colorGroup in groupSkinGroup.ColorGroups)
                {
                    foreach (var colorGroupCategoryId in colorGroup.CategoryIds)
                    {
                        var color = useDefaultColors ? colorGroup.DefaultColor : palette[Random.Range(0, palette.Count)];
                        colors[colorGroupCategoryId] = color;
                    }
                }
            }
            SetGender(gender);
            SetSkin(skins, colors);
            return (skins,colors);
        }
        public static (Dictionary<string, string> skins, Dictionary<string, Color> colors) RandomSkinStatic(Gender gender, bool useDefaultColors)
        {
            var skins = new Dictionary<string, string>();
            var colors = new Dictionary<string, Color>();
            var palette = SkinResources.Instance.Colors;
            var group = SkinResources.Instance.GenderSkinGroups.First(x => x.Gender == gender);
            foreach (var groupSkinGroup in group.SkinGroups)
            {
                foreach (var skinCategory in groupSkinGroup.Categories)
                {
                    var skin = skinCategory.Skins[Random.Range(0, skinCategory.Skins.Count)];
                    skins[skinCategory.Id] = skin.Name;
                }

                foreach (var colorGroup in groupSkinGroup.ColorGroups)
                {
                    foreach (var colorGroupCategoryId in colorGroup.CategoryIds)
                    {
                        var color = useDefaultColors ? colorGroup.DefaultColor : palette[Random.Range(0, palette.Count)];
                        colors[colorGroupCategoryId] = color;
                    }
                }
            }
           
            return (skins, colors);
        }
    }
}