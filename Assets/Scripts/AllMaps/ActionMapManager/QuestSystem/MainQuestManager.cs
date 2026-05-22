using AllMaps;
using Base;
using LongDH.LongDHModules;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainQuestManager : BaseBehaviour , IUISingleton
{
    public List<MainQuestResource> mainQuestResources;
    public SecureInt currentMainQuest = new SecureInt(0);
    public MainQuestResource CurrentMainQuest => currentMainQuest.Value < mainQuestResources.Count ?mainQuestResources[currentMainQuest.Value] : null;
   
    public Map currentMapMainQuest = null;
    public void CheckSpawnQuest()
    {
        if (CurrentMainQuest == null) return;
        if(currentMapMainQuest == null)
        {
            currentMapMainQuest = Pool.Get<MapManager>().SetDataOnMap(CurrentMainQuest, CurrentMainQuest.idNode);

        }
        else
        {
            if(currentMapMainQuest.actionMap is MainQuestResource mainQuestResource)
            {
                if(mainQuestResource.id != CurrentMainQuest.id)
                {
                    ResetDataMap();
                    CheckSpawnQuest();
                }    
            }    
        }

    }
    private void ResetDataMap()
    {
        currentMapMainQuest.ResetDataAction();
        currentMapMainQuest = null;
    }    
    private void RemoveMap(params object[] data)
    {
        Map map = (Map)data[0];
        if (currentMapMainQuest == null) return;
        if(map.Id == currentMapMainQuest.Id)
        {
            ResetDataMap();
        }
    }
    public void QuestDoneCombat()
    {
        Pool.Get<QuestManager>().currentQuest.QuestDoneData();
        Pool.Get<QuestManager>().currentQuest = null;
        currentMapMainQuest = null;
        currentMainQuest++;
    }

    public void Init()
    {
        Pool.Set(this);
        Observer.AddObserver(EventList.GamePlay.MOVE_MAP_SUCCESS, CheckSpawnQuest);
        Observer.AddObserver(EventList.GamePlay.USER_ENTER_MAP, RemoveMap);
        currentMainQuest = Client.Instance.CharacterData.QuestManager.CurrentMainQuest;
        CheckSpawnQuest();

    }
}
