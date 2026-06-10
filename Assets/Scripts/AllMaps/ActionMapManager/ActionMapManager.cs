using LongDH.LongDHModules;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using LongDHModules;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ActionMapManager : MonoBehaviour , IUISingleton
{
    public abstract int TimeMonthRandom { get;  }
    public abstract string fileSaveTime { get;  }

    
    private void OnCheckSceneToTrigger(Scene scene, LoadSceneMode mode)
    {
        // Kiểm tra xem ĐÂY CÓ PHẢI là scene bạn muốn hay không (Ví dụ tên scene là "MainMap")
        if (scene.name == "AllMaps")
        {
            Debug.Log($"Đã vào đúng scene: {scene.name}. Tiến hành kích hoạt logic!");

            // Thực hiện logic của bạn tại đây
            // Pool.Get<QuestManager>().QuestCombatTrigger(...);

            // Hủy đăng ký ngay sau khi xử lý xong (nếu chỉ muốn chạy 1 lần)
            LoadAfterBackAllMaps();
        }
        else
        {
            Debug.Log($"Vào scene {scene.name}, không phải scene MainMap nên bỏ qua.");
        }
    }
    public virtual void LoadAfterBackAllMaps()
    {
        OnTimeAdjust();

    }
    private void OnTimeAdjust()
    {
        if (SceneManager.GetActiveScene().name != Pool.Get<GameManager>().listSceneLoaded[0])
        {
            return;
        }
        // 1. Lấy tổng số tháng hiện tại của thế giới
        int timeWordCurrent = GameManager.GetTimeWorldIntPlayer();

        // 2. Lấy thời điểm thực hiện random lần cuối từ PlayerPrefs
        // Nếu là lần đầu tiên (giá trị -1), ta coi như cần random ngay lập tức
        int oldTimeRandomInt = PlayerPrefs.GetInt($"{fileSaveTime}", -1);

        // --- XỬ LÝ RANDOM NPC (Mỗi 5 tháng) ---
        if (oldTimeRandomInt == -1 || (timeWordCurrent - oldTimeRandomInt) >= TimeMonthRandom)
        {
            RandomAction();            // Lưu lại mốc thời gian mới
            Debug.Log($"<color=cyan>NPC Spawned</color> at month: {timeWordCurrent}");
        }

     


    }
    public virtual void RandomAction()
    {
        PlayerPrefs.SetInt(fileSaveTime, GameManager.GetTimeWorldIntPlayer());
        PlayerPrefs.Save();
    }

    [Button]
    public void ClearDataTime()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Đảm bảo dữ liệu được ghi xuống đĩa ngay lập tức
    }

    public virtual void Init()
    {
        Pool.Set(this.GetType(), this);
        Observer.AddObserver(EventList.GamePlay.TIME_ADJUST, OnTimeAdjust);
        SceneManager.sceneLoaded += OnCheckSceneToTrigger;
    }
}
