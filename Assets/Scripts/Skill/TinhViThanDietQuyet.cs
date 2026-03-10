using UnityEngine;
[CreateAssetMenu(fileName = nameof(TinhViThanDietQuyet), menuName = "Skill/Thần Thông/" + nameof(TinhViThanDietQuyet))]
public class TinhViThanDietQuyet : ThanNiemQuyet
{
   
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThanThuong, out SecureInt thanThuong);
        int damebuff = thanThuong * buffValues[GetIndexLevelThanThong(caster)];
        Debug.Log("ddamebuff " + damebuff);

        target.TakeDameByThanThong(caster,new System.Collections.Generic.List<int> { damebuff }, eTypeSkill);
        target.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, thanThuong);

    }

 
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out SecureInt anNac);
        if(anNac>0)
        {
            return true;
        }    
        return false;


    }

    public override string GetDesc(int level)
    {
        return $"Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)} mới có thể sử dụng. Gỡ ra toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)} của đối thủ mỗi một tầng thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương. " + base.GetDesc(level);

    }
}
