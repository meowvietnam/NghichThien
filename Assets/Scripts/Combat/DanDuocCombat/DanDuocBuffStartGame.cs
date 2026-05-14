using Newtonsoft.Json.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanDuocBuffStartGame), menuName = "Đan dược/Đan dược Combat/" + nameof(DanDuocBuffStartGame))]

public class DanDuocBuffStartGame : BaseDanDuocCombat
{
    [SerializeField] protected EBuffAndDeBuff buffType;
    [SerializeField] protected int buff = 1; 
   public override void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data)
   {
        
   }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(buffType, buff * GetDanDuocCombat(caster).count );

    }
}
