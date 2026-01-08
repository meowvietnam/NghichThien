using Base;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class PlayerAvatar : MonoBehaviour
    {
        private SkinResources skinResource;
        [SerializeField] private SkeletonGraphic graphic;
        [SerializeField] private Dictionary<string, string> currentSkins =  new Dictionary<string, string>();
        [SerializeField] private Dictionary<string, Color> currentColors = new Dictionary<string,Color>();
        public void Init(SkinResources skinResources)
        {
            this.skinResource = skinResources;
        }
        public void SetGender(Gender gender)
        {
            var skeletonDataAsset = skinResource.GenderSkinGroups.FirstOrDefault(skinResource => skinResource.Gender == gender).SkeletonDataAsset;
            graphic.skeletonDataAsset = skeletonDataAsset;
            graphic.Initialize(true);
            graphic.AnimationState.SetAnimation(0, "Idle_0", true);
        }
        public void UpdateSkin(Dictionary<string, string> combines, Dictionary<string, Color> colors = null)
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
