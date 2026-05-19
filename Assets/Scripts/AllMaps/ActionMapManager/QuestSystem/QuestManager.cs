using AllMaps;
using Base;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : ActionEnterMapManager , INetworkEvent.ICompleteSideQuest
{
    public enum ETypeSideQuest
    {
        Quiz,
        Combat,

    }
    [SerializeField] public Dictionary<string, string> skins;
    [SerializeField] public Dictionary<string, Color> color;
    [SerializeField] public Gender gender;
    public override string fileSaveMapNode => "SaveSideQuestMap";

    public override int TimeMonthRandom => 5;

    public override string fileSaveTime => "OldTimeRandomSideQuest";

    public override int CountRandom => 3 - Client.Instance.CharacterData.QuestManager.CurrentCountSideQuest;

    [Header("Quiz Side Quest Visual")]
    [SerializeField] private AnswerQuizQuest answerPrefab;
    [SerializeField] private SideQuizQuestResource.Quiz currentQuiz;
   // [Header("Combat Side Quest Visual")]

    // Thay vì khai báo biến List, hãy viết thành hàm trả về List
    public List<string> GetQuizDialogs(string quizQuestion)
    {
        return new List<string>
    {
        "Đạo hữu dừng bước! Ta thấy ngươi căn cốt kinh kỳ, nhưng đường tu tiên cốt ở ngộ tính, chứ không chỉ có man lực.",
        "Ta có vài câu hỏi về thiên địa quy luật. Trả lời đúng, ta sẽ tặng ngươi tiên ngọc.",
        "Ngươi đã sẵn sàng chưa? Hãy nghe kỹ câu hỏi đầu tiên...",
        $"{quizQuestion}" // Truyền thẳng câu hỏi vào đây, không gọi qua Pool nữa
    };
    }
    List<string> correctDialog = new List<string>
    {
            "Ha ha ha! Khá khen cho câu trả lời của ngươi! Không hổ danh là đứa con của trời.",
            "Đây là phần thưởng xứng đáng dành cho ngộ tính đỉnh cao của ngươi. Cầm lấy đi!"
    };
    List<string> wrongDialog = new List<string>
    {
            "Sai rồi! Tiếc thay cho một tâm trí còn quá nhiều tạp niệm, chưa thấu hiểu được chân lý.",
    };
    [SerializeField] private Popup quizQuestParent;
    public void SetSkinSideQuestCurrent(Gender gender, Dictionary<string, string> skin, Dictionary<string,Color>color)
    {
        this.gender = gender;
        this.skins = skin;
        this.color = color;
    }
    public void QuizQuestInitShow(SideQuizQuestResource.Quiz quiz)
    {

        quizQuestParent.Show();
        currentQuiz = quiz;
        List<AnswerQuizQuest> listBtnAnswer = new List<AnswerQuizQuest>();
        var skinRandom = SayDialogManager.Instance.ShowDialogRandomSkin("Người thần bí", GetQuizDialogs(currentQuiz.quiz), null, null, () =>
        {
            for (int i = 0; i < listBtnAnswer.Count; i++)
            {
                var answerBtn = listBtnAnswer[i];
                answerBtn.transform.DOScale(Vector3.one, 0.5f);

            }
        },false);
        SetSkinSideQuestCurrent(skinRandom.gender,skinRandom.skin,skinRandom.color);
        for (int i = 0; i < currentQuiz.answer.Length; i++)
        {
            var answerBtn = answerPrefab.Spawn(answerPrefab.transform.parent);
            answerBtn.gameObject.SetActive(true);
            answerBtn.transform.localScale = Vector3.zero;
            answerBtn.Init(i, currentQuiz.answer[i]);
            listBtnAnswer.Add(answerBtn);

        }
        listBtnAnswer = listBtnAnswer.OrderBy(x => UnityEngine.Random.value).ToList();
        



    }
    public void AnswerQuest(AnswerQuizQuest answer)
    {
        answerPrefab.RecycleAll();
        quizQuestParent.Hide();

        if (answer.currentIndexAnswer == currentQuiz.rightAnswerIndex)
        {
            Observer.Notify(EventList.Network.COMPLETE_SIDEQUEST,this);
            // Fake Reward
            SayDialogManager.Instance.ShowDialog("Người thần bí", gender, skins, color, correctDialog,null,null,() =>{

                Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = 50 }, "Đây là phần thưởng xứng đáng dành cho ngộ tính đỉnh cao của ngươi!");


            });

        }
        else
        {
            SayDialogManager.Instance.ShowDialog("Người thần bí",gender,skins,color, wrongDialog);

        }

        
    }

    public void SuccessCompleteSideQuest(int moneyVip)
    {
        SayDialogManager.Instance.ShowDialog("Người thần bí", gender, skins, color, correctDialog, null, null, () => {

            Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = moneyVip }, "Đây là phần thưởng xứng đáng dành cho ngộ tính đỉnh cao của ngươi!");


        });
    }

    public void FailCompleteSideQuest(string message)
    {
    }

    public override ActionMapResourceSO GetActionMapResourceSO()
    {
      return  actionMapResourceSOs[UnityEngine.Random.Range(0, actionMapResourceSOs.Length)];
    }
}
