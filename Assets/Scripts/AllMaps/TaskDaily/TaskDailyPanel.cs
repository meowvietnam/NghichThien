using LongDH.LongDHModules;
using System.Collections.Generic;
using AllMaps;
using CreateCharacter;
using LongDHModules;
using Network;
using UnityEngine;
using UnityEngine.UI;

public enum EDailyQuest
{
    TongMon,
    VanTichThanh,
    DatPhongThanh,
    ThienTinhThanh,
    QuangLangThanh,
    ThienCoCac,
}

public class TaskDailyPanel : BaseBehaviour, INetworkEvent.ILoadTask, INetworkEvent.IReLoadTask, INetworkEvent.IDoneTask
{
    [SerializeField] private Text textNameTask;
    [SerializeField] private List<TaskDaily> listTaskDaily;
    [SerializeField] private TaskDaily taskDaily = null;
    [SerializeField] private Button btnWatchAds;
    [SerializeField] private Popup popupTaskDaily;
    [SerializeField] private Button btnTaskShow;

    private void OnEnable()
    {
        textNameTask.text = GameManager.dictMapTypeWithName.GetValueOrDefault(Pool.Get<GameManager>().currentEMapType);
    }

    public static EDailyQuest GetTaskType()
    {
        if (Pool.Get<GameManager>().CurrentEMonPhai != Base.EMonPhai.None)
        {
            return EDailyQuest.TongMon;
        }
        else
        {
            switch (Pool.Get<GameManager>().currentEMapType)
            {
                case AllMaps.EMapType.VanTichThanh:
                    return EDailyQuest.VanTichThanh;

                case AllMaps.EMapType.ThienTinhThanh:
                    return EDailyQuest.ThienTinhThanh;

                case AllMaps.EMapType.DatPhongThanh:
                    return EDailyQuest.DatPhongThanh;

                case AllMaps.EMapType.QuangLangThanh:
                    return EDailyQuest.QuangLangThanh;

                case AllMaps.EMapType.ThienCoCac:
                    return EDailyQuest.ThienCoCac;
                default:
                    break;
            }
        }

        return default;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Pool.Set(this);
        popupTaskDaily.Hide();
        SetEventButton(btnWatchAds, ReloadTask);
        SetEventButton(btnTaskShow, () =>
        {
            Observer.Notify(EventList.Network.LOAD_TASK, this, GetTaskType());
            popupTaskDaily.Show();
        });
    }

    private void ReloadTask()
    {
        var charData = Client.Instance.CharacterData;
        if (!charData.IsNoAds)
        {
            var popup = Pool.Get<PopupConfirmAction>();
            popup.ShowPopConfirmAction("Xác nhận xem quảng cáo để làm mới các nhiệm vụ chưa làm!", ConfirmReloadTask);
        }
        else
        {
            ConfirmReloadTask();
        }
    }

    private void ConfirmReloadTask()
    {
        Observer.Notify(EventList.Network.RELOAD_TASK, this, GetTaskType());
    }

    public void DoneTask(TaskDaily task)
    {
        if (taskDaily != null) return;
        taskDaily = task;
        Observer.Notify(EventList.Network.DONE_TASK, this, taskDaily.DailyQuest.Id);
    }

    private void InitTask(List<DailyQuest> listQuests)
    {
        for (var i = 0; i < listQuests.Count; i++)
        {
            listTaskDaily[i].InitTask(listQuests[i]);
        }
    }

    public void SuccessLoadTask(List<DailyQuest> listTask)
    {
        popupTaskDaily.Show();
        InitTask(listTask);
    }

    public void FailLoadTask(string message)
    {
        popupTaskDaily.Hide();
    }

    public void SuccessReLoadTask(List<DailyQuest> listTask)
    {
        InitTask(listTask);
    }

    public void FailReLoadTask(string message)
    {
        popupTaskDaily.Hide();
    }

    public void SuccessDoneTask()
    {
        taskDaily.InitTask(taskDaily.DailyQuest);
        taskDaily = null;
    }

    public void FailDoneTask(string message)
    {
        NotifyGroup.Instance.Show(message);
    }
}