
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LacAnh), menuName = "Skill/Thần Thông/" + nameof(LacAnh))]

public class LacAnh : ThanThong, BaseSkill.LienKich
{

    [SerializeField] private int[] dames;
    [SerializeField] private int[] percentBuff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);


    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int countDebuff = 0;
        foreach (var item in target.buffAndDebuffCount)
        {
            if(BaseSkill.eDebuffs.Contains(item.Key))
            {
                countDebuff++;
            }
        }
        int dame = dames[GetIndexLevelThanThong(caster)];
        float percent = (float)percentBuff[GetIndexLevelThanThong(caster)]/100f;

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { (int)(dame * (percent* countDebuff)) }, eTypeSkill, true);

    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)}. Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Thuy)} hệ thần thông, đối phương mỗi có một loại trạng thái tiêu cực thì tổn thương tăng lên {GetColoredText($"{percentBuff[level-1]}%", "red")}";

    }
}
