using UnityEngine;
using LongDH.LongDHModules;
using System.Collections.Generic;
[CreateAssetMenu(fileName = nameof(HoaHuyetDocChuong), menuName = "Skill/Thần Thông/" + nameof(HoaHuyetDocChuong))]

public class HoaHuyetDocChuong : ThanThong
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt trungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
        HauKeVoLuc(caster);

        CombatStatic.TrungDoc(caster, target);

        caster.Healing(trungDoc);
       



    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt trungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
        return trungDoc > 0;
    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Kích phát hiệu ứng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} của đối thủ. Đồng thời, hồi một lượng Hp bằng với lượng sát thương gây ra";

    }
}
