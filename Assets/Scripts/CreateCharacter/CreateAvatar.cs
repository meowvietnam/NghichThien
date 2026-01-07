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
        public void Init(bool hasChar)
        {
            this.hasChar = hasChar;
            addGO.SetActive(!hasChar);
            hasCharGO.SetActive(hasChar);
        }
        public object SetSkins(List<(string,string)> combines,Color hairColor)
        {
            var combineSkin = new Skin("character");
            var skeleton = graphic.Skeleton;
            var data = skeleton.Data;
            List<Slot> hairSlots = null;
            foreach (var (skinId,skinName) in combines)
            {
                var skin = data.FindSkin(skinName);
                if (skin == null)
                {
                    Debug.LogError($"Skin not found: {skinName}");
                    continue;
                }
                if(skinId == "hair")
                {
                    hairSlots = skin.Attachments.Select(a => skeleton.Slots.Items[a.SlotIndex]).ToList();
                   
                }
                combineSkin.AddSkin(skin);
            }
            skeleton.SetSkin(combineSkin);
            skeleton.SetSlotsToSetupPose();

            graphic.Update(0);
            foreach (var item in hairSlots)
            {
                item.SetColor(hairColor);
                Debug.Log($"Set hair slot color to black: {item.Data.Name}");
            }
            return null;
        }
    }
}