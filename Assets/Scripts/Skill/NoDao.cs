using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NoDao), menuName = "Skill/Thần Thông/" + nameof(NoDao))]

public class NoDao : ThanThong , BaseSkill.LienKich
{
    [SerializeField] protected int countAttack = 3;
    [SerializeField] protected int countAttackLienKich = 2;

    [SerializeField] protected int[] damePerCountAttack;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttack; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }
        target.TakeDameByThanThong(caster, dames, false);


    }
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{countAttack} lần {damePerCountAttack[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Kim)} hệ thần thông thì tăng thêm {GetColoredText($"{countAttackLienKich} lần", "red")}";
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttackLienKich; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }
        target.TakeDameByThanThong(caster, dames, false);
    }
}
