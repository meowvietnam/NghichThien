using AllMaps;
using LongDH.LongDHModules;
using Network;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComboSkillResource.NPCData;
using LongDHModules;
using MemoryPack;
using Snapshot;
using UnityEditor;
using UnityEngine;

public class NPCManager : ActionMapManager
{
    [SerializeField] public NPCData[] npcDatas; 
    public override int TimeMonthRandom => 5;
    public override string fileSaveTime => "OldTimeRandomNPC";

    public override void Init()
    {
        base.Init();
        RandomAction();
    }
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
        // Điền chính xác đường dẫn folder bạn muốn quét tại đây
        string[] searchFolders = new string[] { "Assets/Scripts/ComboSkillResource/NPCData" };

        // Truyền mảng searchFolders vào làm tham số thứ 2 của FindAssets
        string[] guids = AssetDatabase.FindAssets("t:NPCData", searchFolders);

        npcDatas = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<NPCData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null && !(asset is EnemyData))
            .ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        Debug.Log($"Đã cập nhật {npcDatas.Length} NPC từ folder chỉ định.");
#endif
    }

    [Button]
    public void Export()
    {
        var npcDatas = this.npcDatas.Select(x => new NpcData(x)).ToArray();
        var bytes = MemoryPackSerializer.Serialize(npcDatas);
        var path = Path.Combine(Application.dataPath, "Exports", "npc-datas.bin");
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
        Debug.Log(path);
    }
}
