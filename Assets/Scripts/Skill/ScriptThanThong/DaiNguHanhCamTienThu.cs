using UnityEngine;
[CreateAssetMenu(fileName = nameof(DaiNguHanhCamTienThu), menuName = "Skill/Thần Thông/" + nameof(DaiNguHanhCamTienThu))]

public class DaiNguHanhCamTienThu : ThanThong
{
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { (int)target.allDameTaked/2 }, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Tạo thành {GetColoredText($"một nửa", "red")} của tổng lượng sát thương từ thần thông mà đối thủ đã phải chịu trong trận chiến này";
    }


}
