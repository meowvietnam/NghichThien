using LongDH.LongDHModules;
using Network;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniTaskPanel : PanelGroup
{
    [SerializeField] private Text timerText;
    [SerializeField] private Text energyText;
    public override void Init()
    {
        base.Init();
        Observer.AddObserver(EventList.GamePlay.TIME_ADJUST,InitTimer);
        Observer.AddObserver(EventList.GamePlay.MOVE_MAP_SUCCESS, InitEnergy);

    }
    private void Start()
    {
        InitTimer();
        InitEnergy();
    }

    private void InitEnergy()
    {
        energyText.text = $"{Client.Instance.CharacterData.CurrentEnergy}/100";
    }    
    private void InitTimer()
    {
        // 1. Lấy giá trị TimeSpan gốc
        TimeSpan originalAge = Client.Instance.CharacterData.Stats.TuoiTac;

        // 2. Tạo một mốc thời gian cơ sở (Base) và cộng tuổi vào, sau đó trừ đi 15 năm
        // Giả sử tính từ mốc 01/01/0001
        DateTime baseDate = new DateTime(1, 1, 1).Add(originalAge).AddYears(-15);

        // 3. Tính toán các thành phần (Trừ đi 1 để về đúng số năm/tháng/ngày đã trôi qua)
        int years = baseDate.Year - 1;
        int months = baseDate.Month - 1;
        int days = baseDate.Day - 1;

        // 4. Gán Text
        timerText.text = $"{years} năm {months} tháng {days} ngày";
    }    
}
