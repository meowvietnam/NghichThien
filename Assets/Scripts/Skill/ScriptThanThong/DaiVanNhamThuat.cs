using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DaiVanNhamThuat), menuName = "Skill/Thần Thông/" + nameof(DaiVanNhamThuat))]

public class DaiVanNhamThuat : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
   {
        38,54,85,150,300
   };
    [SerializeField] protected float delayTimeEffect = 0.5f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out SecureInt danLuc);
        if(danLuc>0)
        {
            dame *= 2;
        }    
       

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu đối thủ có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} thì tổn thương gấp đôi";

    }
}
