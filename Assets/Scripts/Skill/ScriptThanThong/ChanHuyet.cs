using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChanHuyet), menuName = "Skill/Thần Thông/" + nameof(ChanHuyet))]

public class ChanHuyet : ThongHuyet
{
    protected override void ExecuteAction(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKim, buff[GetIndexLevelThanThong(caster)]);
    }
    protected override string TextThuHoach()
    {
        return GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKim);
    }
}
