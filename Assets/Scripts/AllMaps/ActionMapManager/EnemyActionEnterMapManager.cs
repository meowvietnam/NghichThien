using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;

public class EnemyActionEnterMapManager : ActionEnterMapManager
{
    public override int TimeMonthRandom => 1;

    public override string fileSaveTime => "OldTimeRandomEnemy";

    public override string fileSaveMapNode => "saveEnemyMap";

    public override int CountRandom => 7;
    public override void Init()
    {
        base.Init();
        Observer.AddObserver(EventList.GamePlay.MOVE_MAP_SUCCESS,()=> {

            SetActionOnMap(156);

        });
    }
    public override ActionMapResourceSO GetActionMapResourceSO()
    {
        return actionMapResourceSOs.First();
    }
    public override void LoadAfterBackAllMaps()
    {
        base.LoadAfterBackAllMaps();
        SetActionOnMap(156);
    }
}