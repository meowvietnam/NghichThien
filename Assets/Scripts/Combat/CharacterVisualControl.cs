using UnityEngine;
using Spine.Unity;
using CreateCharacter;

public class CharacterVisualControl : MonoBehaviour
{
    [SerializeField] public ETypeVisualChar eTypeVisualChar;
    [SerializeField] public SkeletonGraphic[] characterGenders;
    [SerializeField] public SkeletonGraphic currentCharGender => characterGenders[(int)eTypeVisualChar];
    public string ANIM_DIE => "Die_0";
    public string ANIM_IDLE
    {
        get
        {
            // Truy cập vào SkeletonData để tìm Animation
            var skeletonData = currentCharGender.SkeletonDataAsset.GetSkeletonData(true);

            if (skeletonData.FindAnimation("Idle_0") != null)
            {
                return "Idle_0";
            }
            return "stand_0";
        }
    }
    public string ANIM_HIT
    {
        get
        {
            // Truy cập vào SkeletonData để tìm Animation
            var skeletonData = currentCharGender.SkeletonDataAsset.GetSkeletonData(true);

            if (skeletonData.FindAnimation("Hit_0") != null)
            {
                return "Hit_0";
            }
            return "hit_0";
        }
    }
    public string ANIM_PUNCH
    {
        get
        {
            // Truy cập vào SkeletonData để tìm Animation
            var skeletonData = currentCharGender.SkeletonDataAsset.GetSkeletonData(true);

            if (skeletonData.FindAnimation("Punch_0") != null)
            {
                return "Punch_0";
            }
            return "punch_0";
        }
    }
    public void Init(ETypeVisualChar eTypeVisualChar , CharacterStatsSecure charStat)
    {
        foreach (var item in characterGenders)
        {
            if(item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(false);

            }
        }
        this.eTypeVisualChar = eTypeVisualChar;
        if (eTypeVisualChar == ETypeVisualChar.Character)
        {
            var charGraphic = currentCharGender.GetComponent<CharacterGraphic>();
            charGraphic.SetGender(charStat.Gender);
            charGraphic.SetSkin(charStat.Skins, charStat.Colors);
        }
        currentCharGender.gameObject.SetActive(true);
        AnimIdle();
    }
    public void AnimDie()
    {
        currentCharGender.AnimationState.SetAnimation(0, ANIM_DIE, false);

    }
    public void AnimIdle()
    {
        currentCharGender.AnimationState.SetAnimation(0, ANIM_IDLE, true);

    }
    public void AnimHit()
    {
        currentCharGender.AnimationState.SetAnimation(0, ANIM_HIT, false);

    }
    public void AnimPunch()
    {
        currentCharGender.AnimationState.SetAnimation(0, ANIM_PUNCH, false);

    }
}
