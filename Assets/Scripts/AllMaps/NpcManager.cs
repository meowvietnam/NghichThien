using AllMaps;
using LongDH.LongDHModules;
using Network;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NPCManager : ActionMapManager
{
    [SerializeField] public NPCData[] npcDatas; 
    public override int TimeMonthRandom => 5;
    public override string fileSaveTime => "OldTimeRandomNPC";


    public override void RandomAction()
    {
        var mapManager = Pool.Get<MapManager>();
        foreach (var npc in npcDatas)
        {
            int randomMapId = mapManager.maps[UnityEngine.Random.Range(0, mapManager.maps.Length)].Id;
            npc.charStats.idNode = randomMapId;

        }
        base.RandomAction();
    }
    [Button]
    private void OnUpdateDataNPCData()
    {
#if UNITY_EDITOR
        // Tìm tất cả GUID của các asset có type là CongPhap
        string[] guids = AssetDatabase.FindAssets("t:NPCData");

        npcDatas = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<NPCData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        EditorUtility.SetDirty(this);
        Debug.Log($"Đã cập nhật {npcDatas.Length} NPC vào mảng.");
#endif
    }
}
