using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class CreateAvatar : CharacterGraphic
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject addGO;
        [SerializeField] private GameObject hasCharGO;
        private bool hasChar;
        public void Init(bool hasChar, SkinResources skinResources)
        {
            this.hasChar = hasChar;
            addGO.SetActive(!hasChar);
            hasCharGO.SetActive(hasChar);
            this.skinResources = skinResources;
        }
    }
}