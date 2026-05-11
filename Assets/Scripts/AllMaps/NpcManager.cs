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
        // 1. Lấy level player
        int playerLevel = Client.Instance.CharacterData.Progression.Level;

        // 2. Xác định Đại Cảnh Giới (1-3: Luyện Khí, 4-6: Trúc Cơ...)
        // Công thức: (level - 1) / 3 giúp xác định nhóm cảnh giới (0: LK, 1: TC, 2: KD...)
        int currentGreatRealmIndex = (playerLevel - 1) / 3;
        int currentGreatRealmMax = (currentGreatRealmIndex + 1) * 3;

        int targetLevel = playerLevel;
        float rand = UnityEngine.Random.value;

        if (rand <= 0.25f)
        {
            // --- 25% RANDOM ENEMY CÓ LEVEL LỚN HƠN +2 LEVEL CỦA PLAYER ---
            // Min là level tiếp theo (playerLevel + 1), Max là +2 (playerLevel + 2)
            // Nhưng theo ví dụ của bạn (lv10 -> lấy 11 hoặc 12), ta dùng:
            int minLvl = playerLevel + 1;
            int maxLvl = playerLevel + 2;
            targetLevel = UnityEngine.Random.Range(minLvl, maxLvl + 1);
        }
        else
        {
            // --- 75% RANDOM TRONG ĐẠI CẢNH GIỚI (TỪ LV 1 ĐẾN MAX CỦA CẢNH GIỚI HIỆN TẠI) ---
            // Ví dụ: Player lv 10 (Nguyên Anh sơ kỳ), Max cảnh giới này là 12.
            // Nhưng theo yêu cầu của bạn: "Min là 1, Max là 10 (level hiện tại)"
            targetLevel = UnityEngine.Random.Range(1, playerLevel + 1);
        }

        // 3. Lọc danh sách Enemy theo targetLevel
        // GroupBy theo typeCombo để đa dạng hóa loại kẻ địch trên map
        var groupedEnemies = enemyDatas
            .Where(e => e != null &&
                        e.charStatsCreateRef != null &&
                        e.charStatsCreateRef.level.Value == targetLevel)
            .GroupBy(e => e.comboChar.typeCombo)
            .Select(group => group.ElementAt(UnityEngine.Random.Range(0, group.Count()))) // Lấy ngẫu nhiên 1 con trong nhóm combo đó
            .ToList();

        // Bảo hiểm: Nếu level target không có enemy nào, lấy enemy cùng level player
        if (groupedEnemies.Count == 0)
        {
            groupedEnemies = enemyDatas
                .Where(e => e != null && e.charStatsCreateRef.level.Value == playerLevel)
                .GroupBy(e => e.comboChar.typeCombo)
                .Select(group => group.First())
                .ToList();
        }

        // 4. Xáo trộn (Shuffle) danh sách
        for (int i = groupedEnemies.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = groupedEnemies[i];
            groupedEnemies[i] = groupedEnemies[j];
            groupedEnemies[j] = temp;
        }

        // 5. Lấy số lượng ngẫu nhiên từ 2 đến maxCount
        int finalCount = UnityEngine.Random.Range(2, maxCount + 1);
        finalCount = Mathf.Min(finalCount, groupedEnemies.Count);

        var listEnemy = groupedEnemies.Take(finalCount).ToList();

        // 6. Cập nhật Map và Lưu trữ
        currentEnemy = listEnemy;
        foreach (var item in currentMap)
        {
            if (item != null) item.ResetData();
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
