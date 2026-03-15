using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SaTang), menuName = "Skill/Thần Thông/" + nameof(SaTang))]

public class SaTang : ThanThong , BaseSkill.LienKich
{
    [SerializeField] private HoaCat hoaCat;
    [SerializeField]
    private int[] dames = new int[]
    {
        16,24,38,70,140
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,2,4,8,16
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

        

    }

   
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}. Đồng thời, kích hoạt thần thông {GetColoredText($"[Hóa Cát]", "red")}";

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, buffValues[GetIndexLevelThanThong(caster)]);
        caster.AddThanThongCoolDown(hoaCat, -1);
        Observer.Notify(EventList.GamePlay.PLAY_SKILL, caster, target, hoaCat, new List<LinhKhiTieuTan>());
    }
}
