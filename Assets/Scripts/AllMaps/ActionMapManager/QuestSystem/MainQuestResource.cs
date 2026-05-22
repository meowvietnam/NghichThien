using AllMaps;
using LongDH.LongDHModules;
using Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MainQuestResource), menuName = "Action Map/Main Quest/" + nameof(MainQuestResource))]
public class MainQuestResource : QuestResource, INetworkEvent.ICompleteMainQuest
{
    public int id;
    public override List<string> CompleteQuest => completeQuest;
    public List<string> completeQuest;
    public List<string> startQuest;
    public string requireActive;
    public List<AudioClip> voiceStartQuest;
    public List<AudioClip> voiceComplete;
    public List<Sprite> spriteListAvatarStart;
    public List<Sprite> spriteListAvatarDone;
    public Sprite spriteDoneQuest;
    public RewardSnapshot rewardRef;
    public NPCData[] enemyNeedCombat;
    public string nameCharDialog;
    public float scaleThisChar = 3;
    public int idNode;
    public string descDoneQuest;

    public override void QuestDoneData()
    {
        Observer.Notify(EventList.Network.COMPLETE_MAINQUEST, this, Pool.Get<MainQuestManager>().currentMainQuest.Value);
        SayDialogManager.Instance.ShowDialogWithAvatar(nameCharDialog, spriteListAvatarDone, completeQuest, spriteDoneQuest, voiceComplete, () =>
        {
            string detailedDesc = !string.IsNullOrEmpty(descDoneQuest) ? $"\n{descDoneQuest}" : "";

            Pool.Get<EventRewardPanel>().InitShowEventReward(rewardRef, $"Hoàn thành chương {id + 1}{detailedDesc}");
        }, true, scaleThisChar);
    }

    public void FailMainQuest(string message)
    {
    }


    public void SuccessCompleteMainQuest()
    {
        Pool.Get<MainQuestManager>().currentMainQuest.Value = Client.Instance.CharacterData.QuestManager.CurrentMainQuest;
        Pool.Get<MainQuestManager>().CheckSpawnQuest();
    }

    public override void ActiveEventMap(Map map)
    {
        base.ActiveEventMap(map);
        if (CanActiveEventMap())
        {
           Pool.Get<QuestManager>().enemyCombats = new List<NPCData>(enemyNeedCombat.ToList());
           SayDialogManager.Instance.ShowDialogWithAvatar(nameCharDialog, spriteListAvatarStart, startQuest, null, voiceStartQuest, () =>
           {
                Pool.Get<GameManager>().GoToPrepareCombat(new GameManager.CharCombatInit { eTypeVisual = enemyNeedCombat[0].eTypeVisualChar, charStats = enemyNeedCombat[0].charStats }, ETypeCombat.AllMaps);

           },true, scaleThisChar);
        }
        else
        {
            Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"{requireActive}", null);
        }
    }

    public override void QuestDoneUI()
    {
        Pool.Get<MainQuestManager>().QuestDoneCombat();
    }
}
