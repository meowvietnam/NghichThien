using System.Linq;
using UnityEngine;

public class EnemyActionEnterMapManager : ActionEnterMapManager
{
    public override int TimeMonthRandom => 10;

    public override string fileSaveTime => "OldTimeRandomEnemy";

    public override string fileSaveMapNode => "saveEnemyMap";

    public override int CountRandom => 7;

    public override ActionMapResourceSO GetActionMapResourceSO()
    {
        return actionMapResourceSOs.First();
    }

 
}
