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
        energyText.text = $"{Client.Instance.CharacterData.Progression.CurrentEnergy}/100";
    }    
    private void InitTimer()
    {

        // 4. Gán Text
        timerText.text = GameManager.GetTimeWorldText(GameManager.GetTimeWorldIntPlayer());
    }    
}
