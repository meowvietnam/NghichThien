using UnityEngine;
[CreateAssetMenu(fileName = nameof(TichDietChi), menuName = "Skill/Thần Thông/" + nameof(TichDietChi))]

public class TichDietChi : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
   {
        50,100,200,400,800
   };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        30,60,120,240,480
    };
    [SerializeField] private float delayDelete = 1.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        if(caster.CurrentLinhKhi <= 0)
        {
            dame += buffValues[GetIndexLevelThanThong(caster)];
        }    
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });
        ShowSkillEffectWorld(target, delayDelete);
    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu sử dụng thần thông này mà bản thân không còn thừa linh khí nào thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
