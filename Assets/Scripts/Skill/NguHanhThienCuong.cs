using System.Collections.Generic;
using UnityEngine;
using static CharacterCombat;
[CreateAssetMenu(fileName = nameof(NguHanhThienCuong), menuName = "Skill/Thần Thông/" + nameof(NguHanhThienCuong))]

public class NguHanhThienCuong : KhuVatThuat
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        base.Execute(caster, target, data);
      

    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} "+ base.GetDesc(level);

    }
}

