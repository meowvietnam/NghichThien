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
[System.Serializable]
public class EnemySaveEntry
{
    public int idChar;
    public int idNode;
}
public class NpcManager : MonoBehaviour
{
    [SerializeField] public NPCData[] npcDatas;
    [SerializeField] public EnemyData[] enemyDatas;
    [SerializeField] public List<EnemyData> currentEnemy;
    [SerializeField] public List<Map> currentMapEnemy;
    public int timeMonthRandomEnemy => 5;
    public int timeMonthRandomNPC => 5;

    public static string SaveOldTimeRandomNPC => "OldTimeRandomNPC";
    public static string SaveOldTimeRandomEnemy => "OldTimeRandomEnemy";

    private void Awake()
    {
        Pool.Set(this);
        Observer.AddObserver(EventList.GamePlay.TIME_ADJUST, OnTimeAdjust);

    }
    private void Start()
    {
        LoadNPCMap();
        LoadEnemyMap();
        OnTimeAdjust();
    }
    [Button]
    public void ClearDataTime()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Đảm bảo dữ liệu được ghi xuống đĩa ngay lập tức
    }    
    public void OnTimeAdjust()
    {
        // 1. Lấy tổng số tháng hiện tại của thế giới
        int timeWordCurrent = GameManager.GetTimeWorldInt();

        // 2. Lấy thời điểm thực hiện random lần cuối từ PlayerPrefs
        // Nếu là lần đầu tiên (giá trị -1), ta coi như cần random ngay lập tức
        int oldTimeRandomNpcInt = PlayerPrefs.GetInt("OldTimeRandomNPC", -1);
        int oldTimeRandomEnemyInt = PlayerPrefs.GetInt("OldTimeRandomEnemy", -1);

        // --- XỬ LÝ RANDOM NPC (Mỗi 5 tháng) ---
        if (oldTimeRandomNpcInt == -1 || (timeWordCurrent - oldTimeRandomNpcInt) >= timeMonthRandomNPC)
        {
            NPCRandomOnMap(new List<int> {  });
            // Lưu lại mốc thời gian mới
            Debug.Log($"<color=cyan>NPC Spawned</color> at month: {timeWordCurrent}");
        }

        // --- XỬ LÝ RANDOM ENEMY (Mỗi 3 tháng) ---
        if (oldTimeRandomEnemyInt == -1 || (timeWordCurrent - oldTimeRandomEnemyInt) >= timeMonthRandomEnemy)
        {
            EnemyRandomOnMap(5); // Truyền maxCount ví dụ là 5
            Debug.Log($"<color=red>Enemy Spawned</color> at month: {timeWordCurrent}");
        }


    }    
    private void SaveNPCMap()
    {
        Dictionary<int, int> dict = new Dictionary<int, int>();
        foreach (var npc in npcDatas)
        {
            dict.Add(npc.charStatsCreateRef.idChar.Value, npc.charStats.idNode);
        }
        string json = JsonConvert.SerializeObject(dict);
        File.WriteAllText(Application.persistentDataPath + "/saveNPCMap.json", json);
    }
    public void OnGoToEnemy(EnemyData enemyData)
    {
        currentEnemy.Remove(enemyData);
        SaveEnemyMap(currentEnemy);
    }
    private void SaveEnemyMap(List<EnemyData> enemyDataSave)
    {
        List<EnemySaveEntry> saveList = new List<EnemySaveEntry>();

        foreach (var enemy in enemyDataSave)
        {
            saveList.Add(new EnemySaveEntry
            {
                idChar = enemy.charStatsCreateRef.idChar.Value,
                idNode = enemy.charStats.idNode
            });
        }

        string json = JsonConvert.SerializeObject(saveList);
        File.WriteAllText(Application.persistentDataPath + "/saveEnemyMap.json", json);
    }
    private List<EnemyData> EnemyRandomOnMap(int maxCount)
    {
        // 1. Lấy level player
        int playerLevel = Client.Instance.CharacterData.Progression.Level;
        List<EnemyData> listEnemy = new List<EnemyData>();

        // 2. Chạy vòng lặp đúng số lượng maxCount để chọn từng enemy
        for (int i = 0; i < maxCount; i++)
        {
            int targetLevel = playerLevel;
            float rand = UnityEngine.Random.value;

            // Xác định level mục tiêu cho vị trí này
            if (rand <= 0.25f)
            {
                // 25% vượt cấp (+1 hoặc +2)
                targetLevel = UnityEngine.Random.Range(playerLevel + 1, playerLevel + 3);
            }
            else
            {
                // 75% cùng hoặc dưới cảnh giới (1 -> playerLevel)
                targetLevel = UnityEngine.Random.Range(1, playerLevel + 1);
            }

            // 3. Tìm danh sách enemy phù hợp với level này
            var possibleEnemies = enemyDatas
                .Where(e => e != null &&
                            e.charStatsCreateRef != null &&
                            e.charStatsCreateRef.level.Value == targetLevel)
                .ToList();

            // Bảo hiểm: Nếu level đó không có quái, lấy quái bằng level player
            if (possibleEnemies.Count == 0)
            {
                possibleEnemies = enemyDatas
                    .Where(e => e != null && e.charStatsCreateRef.level.Value == playerLevel)
                    .ToList();
            }

            // Chọn ngẫu nhiên 1 con từ danh sách phù hợp
            if (possibleEnemies.Count > 0)
            {
                EnemyData randomEnemy = possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Count)];
                listEnemy.Add(randomEnemy);
            }
        }

        // 4. Cập nhật dữ liệu Map (Reset và Lưu)
        currentEnemy = listEnemy;
        foreach (var item in currentMapEnemy)
        {
            if (item != null) item.ResetDataAction();
        }
        currentMapEnemy.Clear();

        SetEnemyOnMap();
        SaveEnemyMap(listEnemy);

        // Lưu thời gian random
        PlayerPrefs.SetInt(SaveOldTimeRandomEnemy, GameManager.GetTimeWorldInt());
        PlayerPrefs.Save();

        return listEnemy;
    }
    private void SetEnemyOnMap()
    {
        var mapManager = Pool.Get<MapManager>();
        var mapExcept = mapManager.maps.Where(x => x.IsEmptyAction && !(x is Stronghold)).ToList();

        foreach (var enemy in currentEnemy)
        {
            int randomMapId = mapExcept[UnityEngine.Random.Range(0, mapExcept.Count)].Id;
            var map = mapManager.FindMap(randomMapId);
            enemy.charStats.idNode = randomMapId;
            mapManager.SetDataOnMap(enemy,randomMapId);
            mapExcept.Remove(map);
            currentMapEnemy.Add(map);
        }
    }    
    private void NPCRandomOnMap(List<int> idNpcExcept)
    {
        var mapManager = Pool.Get<MapManager>();
        foreach (var npc in npcDatas)
        {
            if (!idNpcExcept.Contains(npc.charStatsCreateRef.idChar))
            {
                int randomMapId = mapManager.maps[UnityEngine.Random.Range(0, mapManager.maps.Length)].Id;
                npc.charStats.idNode = randomMapId;
            }

        }

        SaveNPCMap();
        PlayerPrefs.SetInt(SaveOldTimeRandomNPC, GameManager.GetTimeWorldInt());
        PlayerPrefs.Save();

    }

    private void LoadNPCMap()
    {

        string path = Application.persistentDataPath + "/saveNPCMap.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            // Tiếp tục xử lý dữ liệu (JsonUtility.FromJson...)
            var dictLoadNPCMap = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
            if(dictLoadNPCMap != null)
            {

                List<int> npcExcept = new List<int>();
                foreach (var item in dictLoadNPCMap)
                {

                    npcDatas.FirstOrDefault(x => x.charStatsCreateRef.idChar == item.Key).charStats.idNode = item.Value;
                    npcExcept.Add(item.Key);
                }
                NPCRandomOnMap(npcExcept);

            }
            else
            {
                NPCRandomOnMap(new List<int> { });
                Debug.LogWarning("Không tìm thấy file save. Khởi tạo dữ liệu mới.");
            }
            Debug.Log("Đã load dữ liệu thành công!");
        }
        else
        {
            NPCRandomOnMap(new List<int> { });
            Debug.LogWarning("Không tìm thấy file save. Khởi tạo dữ liệu mới.");
            return;
        }


    }
    private void LoadEnemyMap()
    {

        string path = Application.persistentDataPath + "/saveEnemyMap.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            // Tiếp tục xử lý dữ liệu (JsonUtility.FromJson...)
            var data = JsonConvert.DeserializeObject<List<EnemySaveEntry>>(json);
            List<int> npcExcept = new List<int>();
            foreach (var item in data)
            {
                var enemy = enemyDatas.FirstOrDefault(x => x.charStatsCreateRef.idChar == item.idChar);
                currentEnemy.Add(enemy);
                enemy.charStats.idNode = item.idNode;
                
            }
            SetEnemyOnMap();
            Debug.Log("Đã load dữ liệu thành công!");
        }
        else
        {
            EnemyRandomOnMap(5);
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
