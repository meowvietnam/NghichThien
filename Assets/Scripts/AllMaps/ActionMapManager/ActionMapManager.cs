using LongDH.LongDHModules;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionMapManager : MonoBehaviour
{
    public abstract int TimeMonthRandom { get;  }
    public abstract string fileSaveTime { get;  }

    private void Awake()
    {
        Pool.Set(this.GetType(), this);
        Observer.AddObserver(EventList.GamePlay.TIME_ADJUST, OnTimeAdjust);

    }
    protected virtual void Start()
    {
        OnTimeAdjust();
    }
    private void OnTimeAdjust()
    {
        // 1. Lấy tổng số tháng hiện tại của thế giới
        int timeWordCurrent = GameManager.GetTimeWorldInt();

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
        PlayerPrefs.SetInt(fileSaveTime, GameManager.GetTimeWorldInt());
        PlayerPrefs.Save();
    }

    [Button]
    public void ClearDataTime()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Đảm bảo dữ liệu được ghi xuống đĩa ngay lập tức
    }
    

}
