using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(fileName = nameof(BaoNhien), menuName = "Skill/Thần Thông/" + nameof(BaoNhien))]

public class BaoNhien : ThanThong, BaseSkill.LienKich
{
    [SerializeField]
    private int[] dameValues;

     
    private void ExcuteDiem(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out SecureInt diem);
        target.AddDamageData(new System.Collections.Generic.List<int> { diem });
    }    
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        int dame = dameValues[GetIndexLevelThanThong(caster)];

        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });
        Debug.Log("BaoNhien before" + target.allDamages[0]);

        ExcuteDiem(caster,target);
        Debug.Log("BaoNhien after" + target.allDamages[0]);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        ExcuteDiem(caster, target);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Đồng thời phát động thêm 1 lần {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}. {GetKeyWord(EKeyWord.LienKich)}, nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Moc)} hệ thần thông thì phát động thêm 1 lần {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}";

    }
}
