using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhiLieu), menuName = "Skill/Thần Thông/" + nameof(KhiLieu))]

public class KhiLieu : ThanThong
{
    
    [SerializeField]
    private int[] buffValues = new int[]
    {
        4,6,8,12,17
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(buffValues[GetIndexLevelThanThong(caster)]);
    }



    public override string GetDesc(int level)
    {
        return $"Khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";

    }
}
