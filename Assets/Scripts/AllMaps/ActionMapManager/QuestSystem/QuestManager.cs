using Base;
using LongDH.LongDHModules;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestManager : ActionEnterMapManager
{
    [SerializeField] protected QuestResource currentQuest;
    public string nameChar;
    public CharacterStatsSecure characterProviderQuest;
    public void SetCharProviderQuest(CharacterStatsSecure characterStats)
    {
       this.characterProviderQuest = characterStats;
    }
    [SerializeField] public Dictionary<string, string> skins => characterProviderQuest.Skins;
    [SerializeField] public Dictionary<string, Color> color => characterProviderQuest.Colors;
    [SerializeField] public Gender gender => characterProviderQuest.Gender;
    public virtual void InitQuest(QuestResource quest)
    {
        this.currentQuest = quest;
    }
    public void QuestCombatTrigger(GameManager.CharCombatInit charCombat, ETypeCombat typeCombat, ECombatResult result)
    {
        if (typeCombat == ETypeCombat.AllMaps && result == ECombatResult.Win)
        {
            DoneQuest(characterProviderQuest , () =>
            {
                characterProviderQuest = null;
            });

        }
    }
   
    public void DoneQuest(CharacterStatsSecure characterShow , Action actionDoneDialog)
    {
        SayDialogManager.Instance.ShowDialog(characterShow.nameUser, characterShow.Gender, characterShow.Skins, characterShow.Colors, currentQuest.CompleteSideQuest, null, null, () => {
            currentQuest.QuestDone();
            // Fake Reward
            Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = 50 }, $"{currentQuest.CompleteSideQuest[currentQuest.CompleteSideQuest.Count - 1]}");
            currentQuest = null;
            actionDoneDialog?.Invoke();


        });
    }
 
    public void InitShowQuestHaveChar(Action actionComplete, List<string> startDialog , CharacterStatsSecure character , bool isHide = true)
    {
        nameChar = character.nameUser;
        SayDialogManager.Instance.ShowDialog(character.nameUser,character.Gender,character.Skins,character.Colors, startDialog, null, null, () =>
        {
            actionComplete?.Invoke();
        }, isHide);

    }
   
}
