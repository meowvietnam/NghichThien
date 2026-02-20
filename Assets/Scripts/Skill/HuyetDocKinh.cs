using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyetDocKinh))]

public class HuyetDocKinh : CongPhap
{
    [SerializeField] private int[] healValues;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(healValues[CurrentLevelCongPhapInCombat(caster)-1]);
    }

    public override string GetDesc(int level)
    {
        return $"Khi đối phương bị kích phát hiệu quả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}, bản thân khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm Hp";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.TRUNG_DOC, ExecuteParams);

    }

}
