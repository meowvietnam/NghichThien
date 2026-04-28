using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(PhanKhiThuat), menuName = "Skill/Thần Thông/" + nameof(PhanKhiThuat))]

public class PhanKhiThuat : ThanThong
{
    [SerializeField] protected int[] dames;
    [SerializeField] private float delayTimeEffect = 0.5f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        target.CaculateDameByThanThong(caster, new List<int> { dames[GetIndexLevelThanThong(caster)] }, eTypeSkill);
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt  thieuDot);
        if(thieuDot > 0)
        {
            // Lấy linh khí hợp lệ
            ELinhKhi linhKhi = target.GetRandomLinhKhiConLai();

            if (linhKhi == ELinhKhi.None)
                return;

            int count = 1; // hoặc random / scale theo skill
            target.TieuTanLinhKhi(linhKhi, count);

        }
        ShowSkillEffectWorld(target, delayTimeEffect);
    }


    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Nếu đối thủ có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} thì tiêu tán một điểm linh khí của hắn";

    }

   
}
