using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ToiHuyetChuong), menuName = "Skill/Thần Thông/" + nameof(ToiHuyetChuong))]

public class ToiHuyetChuong : ThanThong , BaseSkill.TuongSinh
{
    [SerializeField]
    private int[] dames = new int[]
    {
        11,15,25,45,90
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        3,4,6,7,9
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        Action(caster,target,dames);
    }
    private void Action(CharacterCombat caster , CharacterCombat target , int[] dames)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });
        if (caster.listAmKinhTT.Count > 0)
        {
            caster.Healing(dame);

        }
    }    
    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        Action(caster, target, buffValues);

    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu bản thân đang có {GetKeyWord(EKeyWord.AmKinh)} thì hồi một lượng HP bằng sát thương gây ra. {GetKeyWord(EKeyWord.TuongSinh)} thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
