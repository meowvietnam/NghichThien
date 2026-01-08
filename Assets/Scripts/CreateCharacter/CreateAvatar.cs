using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class CreateAvatar : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject addGO;
        [SerializeField] private GameObject hasCharGO;
        [SerializeField] private SkeletonGraphic graphic;
        private bool hasChar;
        private Dictionary<string, string> currentSkins = new Dictionary<string, string>();
        private Dictionary<string, Color> currentColors = new Dictionary<string, Color>();
        public void Init(bool hasChar)
        {
            this.hasChar = hasChar;
            addGO.SetActive(!hasChar);
            hasCharGO.SetActive(hasChar);
        }
        public void SetSkin(Dictionary<string,string> combines,Dictionary<string,Color> colors = null)
        {
            var combineSkin = new Skin("character");
            var skeleton = graphic.Skeleton;
            var data = skeleton.Data;
            foreach (var (skinId,skinName) in combines)
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
        public void SetSkin(string skinId,string skinName)
        {
            currentSkins[skinId] = skinName;
            SetSkin(currentSkins);
        }
        public void SetColor(List<string> skinIds, Color color)
        {
            var skeleton = graphic.Skeleton;
            var data = skeleton.Data;
            foreach (var skinId in skinIds)
            {
                var skinName = currentSkins[skinId];
                var skin = data.FindSkin(skinName);
                foreach (var attachment in skin.Attachments)
                {
                    var slot = skeleton.Slots.Items[attachment.SlotIndex];
                    slot.SetColor(color);
                }
            }
        }
    }
}