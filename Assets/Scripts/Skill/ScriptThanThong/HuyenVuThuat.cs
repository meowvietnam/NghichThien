using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyenVuThuat), menuName = "Skill/Thần Thông/" + nameof(HuyenVuThuat))]
public class HuyenVuThuat : ThanThong
{
    [SerializeField]
    private int[] huyenVus = new int[]
    {
        1,1,1,1,1
    };
    [SerializeField] private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HuyenVu, huyenVus[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }


    public override string GetDesc(int level)
    {
        return $"Thu hoạch được {GetColoredText($"+{huyenVus[level-1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)}";
    }
}
