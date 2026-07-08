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
        var charData = Client.Instance.CharacterData;
        if(!charData.IsInfiniteEnergy)
        {
            energyText.text = $"{charData.Progression.CurrentEnergy}/100";
        }   
        else
        {
            energyText.text = $"Vô hạn";

        }
    }    
    private void InitTimer()
    {

        // 4. Gán Text
        timerText.text = GameManager.GetDateTimeText(GameManager.GetDayTimeWorldIntPlayer());
    }

    private void OnDestroy()
    {
        Observer.RemoveObserver(EventList.GamePlay.TIME_ADJUST, InitTimer);
        Observer.RemoveObserver(EventList.GamePlay.MOVE_MAP_SUCCESS, InitEnergy);
    }
}
