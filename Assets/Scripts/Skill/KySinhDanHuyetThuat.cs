using UnityEngine;
[CreateAssetMenu(fileName = nameof(KySinhDanHuyetThuat), menuName = "Skill/Thần Thông/" + nameof(KySinhDanHuyetThuat))]

public class KySinhDanHuyetThuat : ThanThong
{
    [SerializeField] private int[]dames;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });
        int trienNhieu = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out trienNhieu);
        if(trienNhieu> 0)
        {
            caster.Healing(dame);
        }    

    }


    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level-1]} điểm", "red")} sát thương, nếu là {GetTextLinhKhi(ELinhKhi.Moc)} hệ linh khí thì hấp thụ gấp đôi";
    }
}
