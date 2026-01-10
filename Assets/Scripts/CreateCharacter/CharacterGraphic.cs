using Base;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
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
            var skeletonDataAsset = skinResources.GenderSkinGroups.FirstOrDefault(skinResource => skinResource.Gender == gender).SkeletonDataAsset;
            graphic.skeletonDataAsset = skeletonDataAsset;
            graphic.Initialize(true);
            graphic.AnimationState.SetAnimation(0, "Idle_0", true);
        }
        public void SetSkin(Dictionary<string, string> combines, Dictionary<string, Color> colors = null)
        {
            var combineSkin = new Skin("character");
            var skeleton = graphic.Skeleton;
            var data = skeleton.Data;
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
    }
}