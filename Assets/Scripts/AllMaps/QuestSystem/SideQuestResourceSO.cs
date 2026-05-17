using AllMaps;
using LongDH.LongDHModules;
using System;
using UnityEngine;
public abstract class SideQuestResourceSO : ScriptableObject , IActionMap
{
    public enum ETypeSideQuest
    {
        Quiz,
        Combat,
        Delivery
    }
    public abstract void EnterMap(Map map);
   
}
