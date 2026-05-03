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

public class NpcManager : MonoBehaviour
{
    [SerializeField] public NPCData[] npcDatas;
    [SerializeField] public EnemyData[] enemyDatas;
    [SerializeField] public List<EnemyData> currentEnemy;
    [SerializeField] public List<Map> currentMap;
    public int timeMonthRandomEnemy => 3;
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
        Dictionary<int, int> dict = new Dictionary<int, int>();
        foreach (var enemy in enemyDataSave)
        {
            dict.Add(enemy.charStatsCreateRef.idChar.Value, enemy.charStats.idNode);
        }
        string json = JsonConvert.SerializeObject(dict);
        File.WriteAllText(Application.persistentDataPath + "/saveEnemyMap.json", json);
    }
    private List<EnemyData> EnemyRandomOnMap(int maxCount)
    {
        // 1. Lấy level hiện tại của người chơi
        int playerLevel = Client.Instance.CharacterData.Progression.Level;

        // 2. Tính toán Target Level của Enemy dựa trên tỉ lệ: 50% bằng, 40% dưới 1, 10% trên 1
        int targetLevel = playerLevel;
        float rand = UnityEngine.Random.value;
        if (rand <= 0.4f) // 40% (0.0 -> 0.4)
        {
            targetLevel = Mathf.Max(1, playerLevel - 1); // Không để level thấp hơn 1
        }
        else if (rand <= 0.9f) // 50% tiếp theo (0.4 -> 0.9)
        {
            targetLevel = playerLevel;
        }
        else // 10% còn lại (0.9 -> 1.0)
        {
            targetLevel = playerLevel + 1;
        }

        // 3. Lọc danh sách Enemy theo targetLevel đã tính
        var groupedEnemies = enemyDatas
            .Where(e => e != null &&
                        e.charStatsCreateRef != null &&
                        e.charStatsCreateRef.level.Value == targetLevel)
            .GroupBy(e => e.comboChar.typeCombo)
            .Select(group => group.First()) // Đảm bảo mỗi loại combo chỉ xuất hiện 1 lần
            .ToList();
        if (groupedEnemies.Count == 0)
        {
            groupedEnemies = enemyDatas
                .Where(e => e != null &&
                            e.charStatsCreateRef != null &&
                            e.charStatsCreateRef.level.Value == playerLevel)
                .GroupBy(e => e.comboChar.typeCombo)
                .Select(group => group.First())
                .ToList();
        }
        // 3. Xáo trộn ngẫu nhiên danh sách các loại combo đã lọc được
        int n = groupedEnemies.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = groupedEnemies[i];
            groupedEnemies[i] = groupedEnemies[j];
            groupedEnemies[j] = temp;
        }

        // 4. Xác định số lượng cần lấy (từ 2 đến maxCount)
        // Phải đảm bảo không lấy quá số lượng loại combo thực tế đang có
        int finalCount = UnityEngine.Random.Range(2, maxCount + 1);
        finalCount = Mathf.Min(finalCount, groupedEnemies.Count);

        // 5. Trả về danh sách kết quả
        if (finalCount < 2 && groupedEnemies.Count >= 2) finalCount = 2; // Bảo hiểm ít nhất là 2 nếu có đủ dữ liệu
        var listEnemy = groupedEnemies.Take(finalCount).ToList();
        currentEnemy = listEnemy;
        foreach (var item in currentMap)
        {
            if(item == null) continue;
            item.ResetData();
        }
        currentMap.Clear();
        SetEnemyOnMap();
        SaveEnemyMap(listEnemy);
        PlayerPrefs.SetInt(SaveOldTimeRandomEnemy, GameManager.GetTimeWorldInt());
        PlayerPrefs.Save();

        return listEnemy;
    }
    private void SetEnemyOnMap()
    {
        var mapManager = Pool.Get<MapManager>();
        var mapExcept = mapManager.maps.Where(x => x.iconVisualAction == null && x.enemyDataCurrent == null && !(x is Stronghold)).ToList();

        foreach (var enemy in currentEnemy)
        {
            int randomMapId = mapExcept[UnityEngine.Random.Range(0, mapExcept.Count)].Id;
            var map = mapManager.FindMap(randomMapId);
            enemy.charStats.idNode = randomMapId;
            mapManager.SetEnemyOnMap(enemy,randomMapId);
            mapExcept.Remove(map);
            currentMap.Add(map);
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
            var dictLoadEnemyMap = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);

            List<int> npcExcept = new List<int>();
            foreach (var item in dictLoadEnemyMap)
            {
                var enemy = enemyDatas.FirstOrDefault(x => x.charStatsCreateRef.idChar == item.Key);
                currentEnemy.Add(enemy);
                enemy.charStats.idNode = item.Value;
                
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
