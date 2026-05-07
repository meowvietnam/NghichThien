using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TuQuyThuat), menuName = "Skill/Thần Thông/" + nameof(TuQuyThuat))]

public class TuQuyThuat : ThanThong
{

    [SerializeField]
    private int[] dames = new int[]
    {
        9,12,20,36,72
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        8,10,13,21,36
    };
    [SerializeField] private float delayTimeEffect = 1f;
    [SerializeField] private float cameraSize = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int> { dame }, false , null , false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, buffValues[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectParticleBase(caster, delayTimeEffect, cameraSize);

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out SecureInt hon);
        if(hon>0 && base.IsCanUse(caster, target))
        {
            return true;
        }    
        return false;
    }


    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương cho bản thân. Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)}";

    }
}
