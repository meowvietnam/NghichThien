using AllMaps;
using Base;
using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestResource : ActionMapResourceSO
{
    public override void ActiveEventMap(Map map)
    {
        Pool.Get<SideQuestManager>().InitQuest(this);
    }
    public virtual List<string> CompleteSideQuest { get; }

    public abstract void QuestDone();
}
public abstract class SideQuestResource : QuestResource,INetworkEvent.ICompleteSideQuest
{
    public enum ETypeSideQuest
    {
        Quiz,
        Combat,

    }
    public virtual ETypeSideQuest Type { get; }

    public virtual void SuccessCompleteSideQuest(int moneyVip)
    {
        Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = moneyVip }, $"{CompleteSideQuest[CompleteSideQuest.Count-1]}");

    }

    public void FailCompleteSideQuest(string message)
    {
    }
}
public class MainQuestResource : QuestResource
{
 
    public override void QuestDone()
    {

    }    
}