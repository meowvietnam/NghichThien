using AllMaps;
using Base;
using LongDH.LongDHModules;
using Network;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainQuestManager : BaseBehaviour , IUISingleton
{
    public List<MainQuestResource> mainQuestResources;
    public SecureInt currentMainQuest = new SecureInt(0);
    public MainQuestResource CurrentMainQuest => currentMainQuest.Value < mainQuestResources.Count ?mainQuestResources[currentMainQuest.Value] : null;
   
    public List<Map> currentMapMainQuests = new();
    public void CheckSpawnQuest()
    {
        if (CurrentMainQuest == null) return;
        if (currentMapMainQuests.Count <= 0)
        {
            var mapManager = Pool.Get<MapManager>();
            foreach (var item in CurrentMainQuest.IdNode)
            {
                currentMapMainQuests.Add(mapManager.SetDataOnMap(CurrentMainQuest, item));
            }
        }
        else
        {
            if(currentMapMainQuests.All(x=>x == null))
            {
                currentMapMainQuests.Clear();
            }    
            // Tìm ngay thằng MainQuest đầu tiên xuất hiện trong danh sách Map hiện tại
            var firstMainQuest = currentMapMainQuests
                .Select(x => x.actionMap as MainQuestResource)
                .FirstOrDefault(quest => quest != null);

            // KIỂM TRA ĐỔI QUEST: Nếu tìm thấy Main Quest cũ trên map, mà ID của nó đã khác với ID Quest hiện tại của Player
            if (firstMainQuest != null && firstMainQuest.id != CurrentMainQuest.id)
            {
                Debug.Log($"[Quest System] Phát hiện đổi Main Quest từ ID: {firstMainQuest.id} sang ID mới: {CurrentMainQuest.id}. Tiến hành dọn dẹp Map cũ.");

                // Xóa dữ liệu quest cũ trên các node map
                currentMapMainQuests.ForEach(x => x?.ResetDataAction());
                currentMapMainQuests.Clear();

                // Sinh quest mới (Lưu ý: Đảm bảo trong CheckSpawnQuest() sẽ nạp lại data vào currentMapMainQuests để tránh lặp vô hạn)
                CheckSpawnQuest();
            }
        }

    }
   
    private void RemoveMap(params object[] data)
    {
        Map map = (Map)data[0];
        if (currentMapMainQuests.Count<=0) return;
        currentMapMainQuests.RemoveAll(x => x != null && x.Id == map.Id);
    }
    public void MainQuestDone()
    {
        Pool.Get<QuestManager>().currentQuest.QuestDoneData();
        Pool.Get<QuestManager>().currentQuest = null;
        currentMapMainQuests.Clear();
        currentMainQuest++;
    }

    public void Init()
    {
        Pool.Set(this);
        Observer.AddObserver(EventList.GamePlay.MOVE_MAP_SUCCESS, CheckSpawnQuest);
        Observer.AddObserver(EventList.GamePlay.USER_ENTER_MAP, RemoveMap);
        currentMainQuest = Client.Instance.CharacterData.QuestManager.CurrentMainQuest;
        CheckSpawnQuest();

    }
}
