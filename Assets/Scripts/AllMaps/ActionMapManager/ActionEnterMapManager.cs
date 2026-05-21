using AllMaps;
using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class ActionEnterMapManager : ActionMapManager
{
    [SerializeField] protected ActionMapResourceSO[] actionMapResourceSOs;
    [SerializeField] public List<Map> currentMapRandom;
    public abstract string fileSaveMapNode { get; }

    protected override void Start()
    {
        base.Start();
        LoadData();
        Observer.AddObserver(EventList.GamePlay.USER_ENTER_MAP,RemoveMap);
    }
    private void OnDestroy()
    {
        Observer.RemoveObserver(EventList.GamePlay.USER_ENTER_MAP, RemoveMap);

    }

    public abstract override int TimeMonthRandom { get ;  }
    public abstract override string fileSaveTime { get ;   }

    public abstract int CountRandom { get; }
    public override void RandomAction()
    {
        var mapManager = Pool.Get<MapManager>();
        var mapExcept = mapManager.maps.Where(x => x.IsEmptyAction && !(x is Stronghold)).ToList();
        foreach (var item in currentMapRandom)
        {
            item.ResetDataAction();
        }
        currentMapRandom.Clear();
        for (int i = 0; i < CountRandom; i++)
        {
            int randomMapId = mapExcept[UnityEngine.Random.Range(0, mapExcept.Count)].Id;
            var map = mapManager.FindMap(randomMapId);
            mapExcept.Remove(map);
            SetActionOnMap(randomMapId);
        }
        base.RandomAction();
        SaveRandom();

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
            Debug.Log("Đã load dữ liệu thành công!");
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
        mapManager.SetDataOnMap(GetActionMapResourceSO(), id);
        currentMapRandom.Add(map);

    }
    public virtual void SaveRandom()
    { // Lọc trùng và chỉ lấy ra danh sách các Id (int)
        List<int> dataSaveNode = currentMapRandom
            .GroupBy(x => x.Id)
            .Select(group => group.Key) // Lấy ra cái Id đại diện của nhóm
            .ToList();

        string json = JsonConvert.SerializeObject(dataSaveNode);
        File.WriteAllText(Application.persistentDataPath + $"/{fileSaveMapNode}.json", json);

    }    

}
