using AllMaps;
using LongDH.LongDHModules;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    [SerializeField] public NPCData[] npcDatas;
    private void Awake()
    {
        Pool.Set(this);
    }
    private void Start()
    {
        LoadNPCMap();
    }
    private void SaveNPCMap()
    {
        Dictionary<int, int> dict = new Dictionary<int, int>();
        foreach (var npc in npcDatas)
        {
            dict.Add(npc.charStatsCreateRef.idChar.Value, npc.charStats.idNode);
        }
        string json = JsonConvert.SerializeObject(dict);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }
    private void NPCRandomOnMap(List<int> idNpcExcept)
    {
        var mapManager = Pool.Get<MapManager>();
        foreach (var npc in npcDatas)
        {
            if (!idNpcExcept.Contains(npc.charStatsCreateRef.idChar))
            {
                int randomMapId = mapManager.maps[Random.Range(0, mapManager.maps.Length)].Id;
                npc.charStats.idNode = randomMapId;
            }

        }

        SaveNPCMap();

    }

    private void LoadNPCMap()
    {

        string path = Application.persistentDataPath + "/save.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            // Tiếp tục xử lý dữ liệu (JsonUtility.FromJson...)
            var dictLoadNPCMap = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);

            List<int> npcExcept = new List<int>();
            foreach (var item in dictLoadNPCMap)
            {

                npcDatas.FirstOrDefault(x => x.charStatsCreateRef.idChar == item.Key).charStats.idNode = item.Value;
                npcExcept.Add(item.Key);
            }
            NPCRandomOnMap(npcExcept);
            Debug.Log("Đã load dữ liệu thành công!");
        }
        else
        {
            NPCRandomOnMap(new List<int> { });
            Debug.LogWarning("Không tìm thấy file save. Khởi tạo dữ liệu mới.");
            return;
        }


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
