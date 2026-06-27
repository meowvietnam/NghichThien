using AllMaps;
using Base;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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
            StartCameraCinematic();

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
            if (firstMainQuest != null && firstMainQuest.Id != CurrentMainQuest.Id)
            {
                Debug.Log($"[Quest System] Phát hiện đổi Main Quest từ ID: {firstMainQuest.Id} sang ID mới: {CurrentMainQuest.Id}. Tiến hành dọn dẹp Map cũ.");

                // Xóa dữ liệu quest cũ trên các node map
                currentMapMainQuests.ForEach(x => x?.ResetDataAction());
                currentMapMainQuests.Clear();

                // Sinh quest mới (Lưu ý: Đảm bảo trong CheckSpawnQuest() sẽ nạp lại data vào currentMapMainQuests để tránh lặp vô hạn)
                CheckSpawnQuest();
            }
        }

    }
    // Gọi hàm này để bắt đầu chuỗi sự kiện camera
    public void StartCameraCinematic()
    {
        StartCoroutine(FollowItemsOneByOneRoutine());
    }

    private IEnumerator FollowItemsOneByOneRoutine()
    {
        var player = Pool.Get<Player>();    
        while (player.IsMoving)
        {
            yield return null; // Chờ đến frame tiếp theo và kiểm tra lại trạng thái di chuyển của player
            yield return null; // Chờ đến frame tiếp theo và kiểm tra lại trạng thái di chuyển của player

        }
        // 1. Duyệt qua từng quest item
        foreach (var item in currentMapMainQuests)
        {
            if (item == null) continue;

            // Tập trung camera vào item hiện tại
            CameraFollow.Instance.target = item.transform;

            // Chờ 2 giây trước khi tiếp tục vòng lặp
            yield return new WaitForSeconds(1f);
        }

        // 2. Sau khi đã đi qua hết tất cả các item, quay lại follow Player
        if (player != null)
        {
            CameraFollow.Instance.target = player.transform;
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
        Pool.Get<QuestManager>().currentQuest.QuestDoneData(); // gửi lên sv quest id cũ
        Pool.Get<QuestManager>().currentQuest = null;
        currentMapMainQuests.Clear();
    }

    public void Init()
    {
        Pool.Set(this);
        Observer.AddObserver(EventList.GamePlay.MOVE_MAP_SUCCESS, CheckSpawnQuest);
        Observer.AddObserver(EventList.GamePlay.USER_ENTER_MAP, RemoveMap);
        currentMainQuest = Client.Instance.CharacterData.QuestManager.CurrentMainQuest;
        CheckSpawnQuest();

    }
    [Button]
    public void ExportMainQuestRS()
    {
        var guids = AssetDatabase.FindAssets("t:BaseData");
        var assets = guids.Select(x => AssetDatabase.GUIDToAssetPath(x))
            .Select(x => AssetDatabase.LoadAssetAtPath<BaseData>(x))
            .OrderBy(x => x.Id)
            .ToList();

        var mainQuests = assets.OfType<MainQuestResource>().ToList();
        this.mainQuestResources = mainQuests;
        var mainQuestRS = new MainQuestManagerModel()
        {
            MainQuestResource = mainQuests,
        };
        var writer = new Writer(mainQuestRS.ByteSize());
        writer.Write(mainQuestRS);
        var path = Path.Combine(Application.dataPath, "Exports", "MainQuestManagerModel.bin");
        File.WriteAllBytes(path, writer.Buffer);
    }

}
