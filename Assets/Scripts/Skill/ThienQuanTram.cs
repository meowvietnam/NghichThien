using UnityEngine;
using static CharacterCombat;
[CreateAssetMenu]
public class ThienQuanTram : ThanThong
{
    [SerializeField] private int dame = 25;
    public override void Execute(CharacterCombat caster, CharacterCombat target)
    {
        int hoTrao = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out hoTrao);

        if (hoTrao > 0)
        {
            target.TakeDame(caster, dame * 2);
        }
        else
        {
            target.TakeDame(caster, dame);
        }
    }

    public override string GetDesc()
    {
        return $"Tạo thành {GetColoredText($"{dame}", "red")} điểm sát thương, nếu đối phương có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} thì nhận gấp đôi sát thương";

    }
}
