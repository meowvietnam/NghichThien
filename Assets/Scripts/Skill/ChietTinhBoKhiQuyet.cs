using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChietTinhBoKhiQuyet), menuName = "Skill/Thần Thông/" + nameof(ChietTinhBoKhiQuyet))]

public class ChietTinhBoKhiQuyet : ThanThong
{
    
    [SerializeField]
    private int[] buffValues = new int[]
    {
        8,12,16,20,24
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NgungThan, out SecureInt ngungThan);
        caster.Healing(ngungThan * buffValues[GetIndexLevelThanThong(caster)]);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.NgungThan, ngungThan);

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NgungThan, out SecureInt ngungThan);

        if (ngungThan > 0)
        {
            return true;
        }
        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Gỡ ra toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NgungThan)} Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NgungThan)} hồi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";

    }
}
