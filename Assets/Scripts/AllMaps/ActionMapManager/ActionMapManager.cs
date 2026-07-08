using LongDH.LongDHModules;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using LongDHModules;
using UnityEngine;
using UnityEngine.SceneManagement;
using Network;

public abstract class ActionMapManager : MonoBehaviour , IUISingleton
{
    public abstract int TimeMonthRandom { get;  }
    public abstract string fileSaveTime { get;  }
    public string FileSaveTime => $"{Client.Instance.CharacterData.Name}_{fileSaveTime}";


    private void OnCheckSceneToTrigger(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "AllMaps")
        {
            LoadAfterBackAllMaps();
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
        float monthTimeWordCurrent = GameManager.GetMonthTimeWorldIntPlayer();

        // 2. Lấy thời điểm thực hiện random lần cuối từ PlayerPrefs
        // Nếu là lần đầu tiên (giá trị -1), ta coi như cần random ngay lập tức
        float oldTimeRandomInt = PlayerPrefs.GetFloat($"{FileSaveTime}", -1);

        // --- XỬ LÝ RANDOM NPC (Mỗi 5 tháng) ---
        if (oldTimeRandomInt == -1 || (monthTimeWordCurrent - oldTimeRandomInt) >= TimeMonthRandom)
        {
            RandomAction();            // Lưu lại mốc thời gian mới
            Debug.Log($"<color=cyan>NPC Spawned</color> at month: {monthTimeWordCurrent}");
        }

     


    }
    public virtual void RandomAction()
    {
        PlayerPrefs.SetFloat(FileSaveTime, GameManager.GetMonthTimeWorldIntPlayer());
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

    protected virtual void OnDestroy()
    {
        Observer.RemoveObserver(EventList.GamePlay.TIME_ADJUST, OnTimeAdjust);
        SceneManager.sceneLoaded -= OnCheckSceneToTrigger;
    }
}
