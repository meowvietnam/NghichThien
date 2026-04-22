using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ETaskDaily
{
    TongMon,
    VanTichThanh,
    DatPhongThanh,
    ThienTinhThanh,
    QuangLangThanh,
    ThienCoCac,
    
} 
public class TaskDailyPanel : BaseBehaviour , INetworkEvent.ILoadTask , INetworkEvent.IReLoadTask , INetworkEvent.IDoneTask
{
    [SerializeField] private Text textNameTask;
    [SerializeField] private List<TaskDaily> listTaskDaily;
    [SerializeField] private TaskDaily taskDaily = null;
    [SerializeField] private Button btnWatchAds;
    [SerializeField]
    private Popup popupTaskDaily;
    [SerializeField]
    private Button btnTaskShow;
    private void OnEnable()
    {
        textNameTask.text = GameManager.dictMapTypeWithName.GetValueOrDefault(Pool.Get<GameManager>().currentEMapType);


    }
    public static ETaskDaily GetTaskType()
    {
        if (Pool.Get<GameManager>().CurrentEMonPhai != Base.EMonPhai.None)
        {
            return ETaskDaily.TongMon;
        }
        else
        {
            switch (Pool.Get<GameManager>().currentEMapType)
            {
                
                case AllMaps.EMapType.VanTichThanh:
                    return ETaskDaily.VanTichThanh;

                case AllMaps.EMapType.ThienTinhThanh:
                    return ETaskDaily.ThienTinhThanh;

                case AllMaps.EMapType.DatPhongThanh:
                    return ETaskDaily.DatPhongThanh;

                case AllMaps.EMapType.QuangLangThanh:
                    return ETaskDaily.QuangLangThanh;

                case AllMaps.EMapType.ThienCoCac:
                    return ETaskDaily.ThienCoCac;

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
        SetEventButton(btnWatchAds, () =>
        {
            Pool.Get<PopupConfirmAction>().ShowPopConfirmAction("Xác nhận xem quảng cáo để làm mới các nhiệm vụ chưa làm !", () =>
            {
                Observer.Notify(EventList.Network.RELOAD_TASK, this, GetTaskType());

            });
        });
        SetEventButton(btnTaskShow, () =>
        {
            Observer.Notify(EventList.Network.LOAD_TASK, this, GetTaskType());
            popupTaskDaily.Show();

        });



    }
    public void DoneTask(TaskDaily task)
    {
        if (taskDaily != null) return;
        taskDaily = task;
        Observer.Notify(EventList.Network.DONE_TASK, this, taskDaily.taskDailySnapshot);

    }    
    private void InitTask(List<TaskDailySnapshot> listTask)
    {
        for (int i = 0; i < listTaskDaily.Count; i++)
        {
            listTaskDaily[i].InitTask(listTask[i]);
        }

    }
    public void SuccessLoadTask(List<TaskDailySnapshot> listTask)
    {
        popupTaskDaily.Show();
        InitTask(listTask);



    }

    public void FailLoadTask(string message)
    {
        popupTaskDaily.Hide();
    }

    public void SuccessReLoadTask(List<TaskDailySnapshot> listTask)
    {
        InitTask(listTask);


    }

    public void FailReLoadTask(string message)
    {
        popupTaskDaily.Hide();

    }

    public void SuccessDoneTask()
    {
        taskDaily=null;
    }

    public void FailDoneTask(string message)
    {
    }
}
