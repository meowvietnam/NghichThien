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
        // Tìm tất cả GUID của các asset có type là NPCData (bao gồm cả class con kế thừa nó)
        string[] guids = AssetDatabase.FindAssets("t:NPCData");

        npcDatas = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<NPCData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null && !(asset is EnemyData)) // KIỂM TRA: Nếu KHÔNG PHẢI EnemyData thì mới lấy
            .ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets(); // Nên thêm dòng này để Unity lưu hẳn thay đổi xuống ổ cứng
        Debug.Log($"Đã cập nhật {npcDatas.Length} NPC thuần túy vào mảng (Đã loại bỏ Enemy).");
#endif
    }
}
