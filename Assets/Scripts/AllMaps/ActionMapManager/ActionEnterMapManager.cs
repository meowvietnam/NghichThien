using AllMaps;
using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LongDHModules;
using UnityEngine;

public abstract class ActionEnterMapManager : ActionMapManager
{
    [SerializeField] protected ActionMapResourceSO[] actionMapResourceSOs;
    [SerializeField] public List<Map> currentMapRandom;
    public abstract string fileSaveMapNode { get; }

    public override void LoadAfterBackAllMaps()
    {
        base.LoadAfterBackAllMaps();
        currentMapRandom.Clear();
        LoadData();
    }

    public override void Init()
    {
        base.Init();
        Observer.AddObserver(EventList.GamePlay.USER_ENTER_MAP, RemoveMap);
        LoadAfterBackAllMaps();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Observer.RemoveObserver(EventList.GamePlay.USER_ENTER_MAP, RemoveMap);
    }

    public abstract override int TimeMonthRandom { get; }
    public abstract override string fileSaveTime { get; }

    public abstract int CountRandom { get; }

    public override void RandomAction()
    {
        FillMissingSpawns();
        base.RandomAction();
        SaveRandom();
    }

    private void FillMissingSpawns()
    {
        var mapManager = Pool.Get<MapManager>();
        var mapExcept = mapManager.maps
            .Where(x => mapManager.CanSpawnRandomContent(x) && x.IsEmptyAction && x.MapType != MapType.Stronghold)
            .ToList();
        foreach (var item in currentMapRandom)
        {
            if (item == null) continue;
            if (item.actionMap == null)
            {
                item.ResetDataAction();
            }
        }

        currentMapRandom.RemoveAll(item => item == null || (item != null && item.IsEmptyAction));
        var missingCount = System.Math.Max(0, CountRandom - currentMapRandom.Count);
        for (int i = 0; i < missingCount; i++)
        {
            if (mapExcept.Count == 0) break;
            int randomMapId = mapExcept[UnityEngine.Random.Range(0, mapExcept.Count)].Id;
            var map = mapManager.FindMap(randomMapId);
            mapExcept.Remove(map);
            SetActionOnMap(randomMapId);
        }
    }

    protected virtual void LoadData()
    {
        string path = Application.persistentDataPath + $"/{fileSaveMapNode}.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            // Tiếp tục xử lý dữ liệu (JsonUtility.FromJson...)
            var data = JsonConvert.DeserializeObject<List<int>>(json);
            foreach (var item in data)
            {
                SetActionOnMap(item);
            }

            FillMissingSpawns();
            SaveRandom();

            Debug.Log($"Đã load dữ liệu thành công! {data.Count}");
        }
        else
        {
            RandomAction();
            Debug.LogWarning("Không tìm thấy file save. Khởi tạo dữ liệu mới.");
            return;
        }
    }

    public abstract ActionMapResourceSO GetActionMapResourceSO();

    private void RemoveMap(params object[] data)
    {
        Map map = (Map)data[0];

        currentMapRandom.Remove(map);
        SaveRandom();
    }

    private void SetActionOnMap(int id)
    {
        var mapManager = Pool.Get<MapManager>();
        var map = mapManager.FindMap(id);
        if (!mapManager.CanSpawnRandomContent(map) || map.MapType == MapType.Stronghold) return;

        mapManager.SetDataOnMap(GetActionMapResourceSO(), id);
        currentMapRandom.Add(map);
    }

    public void RemoveSpawnFrom(Map map)
    {
        if (map == null || currentMapRandom == null || !currentMapRandom.Contains(map)) return;

        map.ResetDataAction();
        currentMapRandom.RemoveAll(value => value == map);
        FillMissingSpawns();
        SaveRandom();
    }

    public virtual void SaveRandom()
    {
        // Lọc trùng và chỉ lấy ra danh sách các Id (int)
        if (currentMapRandom.All(x => x == null)) return;
        List<int> dataSaveNode = currentMapRandom
            .Where(x => x != null) // ĐIỀU KIỆN: Nếu x == null thì "continue" (bỏ qua không xử lý)
            .GroupBy(x => x.Id) // Gom nhóm theo Id
            .Select(group => group.Key) // Lấy ra cái Id đại diện của nhóm
            .ToList();
        string json = JsonConvert.SerializeObject(dataSaveNode);
        File.WriteAllText(Application.persistentDataPath + $"/{fileSaveMapNode}.json", json);
    }
}
