using Base;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CreateCharacter
{
    public class CreateCharacterPanel : MonoBehaviour
    {
        [SerializeField] private Gender initializeGender;
        [SerializeField] private PlayerAvatar avatar;
        [SerializeField] private SetPlayerAvatar setPlayerAvatar;
        [SerializeField] private List<SelectGender> selectGenders;
        public void Init(SkinResources skinResources)
        {
            avatar.Init(skinResources);
            setPlayerAvatar.Init(skinResources);
            foreach (var item in selectGenders)
            {
                item.Init();
            }
            var selectGender = selectGenders.FirstOrDefault(s => s.Gender == initializeGender);
            selectGender.ToggleOn();
        }
    }
}
