using AllMaps;
using LongDH.LongDHModules;
using System;
using UnityEngine;
public abstract class ActionMapResourceSO : ScriptableObject , IActionMap
{
    public GameObject prefabIcon;
    public abstract void ActiveEventMap(Map map);
   
}
