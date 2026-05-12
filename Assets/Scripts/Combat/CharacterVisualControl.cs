using UnityEngine;
using Spine.Unity;
using CreateCharacter;

public class CharacterVisualControl : MonoBehaviour
{
    [SerializeField] public ETypeVisualChar eTypeVisualChar;
    [SerializeField] public SkeletonGraphic[] characterGenders;
    [SerializeField] public SkeletonGraphic currentCharGender => characterGenders[(int)eTypeVisualChar];

    public void Init(ETypeVisualChar eTypeVisualChar , CharacterStatsSecure charStat)
    {
        this.eTypeVisualChar = eTypeVisualChar;
        if (eTypeVisualChar == ETypeVisualChar.Character)
        {
            var charGraphic = currentCharGender.GetComponent<CharacterGraphic>();
            charGraphic.SetGender(charStat.Gender);
            charGraphic.SetSkin(charStat.Skins, charStat.Colors);
        }
        currentCharGender.gameObject.SetActive(true);
    }
}
