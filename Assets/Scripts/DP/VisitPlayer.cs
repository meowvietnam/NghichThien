using System;
using DP;
using Network;
using Network.Extensions.MemoryPack;
using UnityEngine;
using UnityEngine.UI;

public class VisitPlayer : BaseBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Button visitButton;
    private string playerName;
    private void Start() 
    {
        SetEventButton(visitButton, Visit);
    }
    private void Visit()
    {
        DongPhuPanel.Instance.Visit(playerName);
    }
  
    public void Init(string name)
    {
        this.playerName = name;
        nameText.text = name;
    }
}
