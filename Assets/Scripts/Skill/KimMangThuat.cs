using UnityEngine;

[CreateAssetMenu(fileName = nameof(KimMangThuat), menuName = "Skill/Thần Thông/" + nameof(KimMangThuat))]
public class KimMangThuat : ThanThong , BaseSkill.LienKich
{
    [SerializeField] private int[] dames;
    [SerializeField] private int[] dameBuffs;
    [SerializeField] private int noiThuong = 1;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster,new System.Collections.Generic.List<int> { dame });
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, noiThuong);


    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dameBuffs[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame },true);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương, bản thân nhận phải  {GetColoredText($"+{noiThuong}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng Thổ hệ thần thông thì tăng lên {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }
   
}
