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


public class SideQuestManager : QuestManager
{
   
   
    public override string fileSaveMapNode => "SaveSideQuestMap";

    public override int TimeMonthRandom => 5;

    public override string fileSaveTime => "OldTimeRandomSideQuest";

    public override int CountRandom => 3 - Client.Instance.CharacterData.QuestManager.CurrentCountSideQuest;

 
    [Header("Quiz Side Quest Visual")]
    [SerializeField] private AnswerQuizQuest answerPrefab;
    [SerializeField] private SideQuizQuestResource.Quiz currentQuiz;
   // [Header("Combat Side Quest Visual")]

    // Thay vì khai báo biến List, hãy viết thành hàm trả về List
   
    [SerializeField] private Popup quizQuestParent;
    
   
    public void QuizQuestInitShow(SideQuizQuestResource.Quiz quiz)
    {
        if(currentQuest is SideQuizQuestResource quizQuestResource)
        {
            quizQuestParent.Show();
            currentQuiz = quiz;
            List<AnswerQuizQuest> listBtnAnswer = new List<AnswerQuizQuest>();
            InitShowQuestRandomSkin(() =>
            {
                for (int i = 0; i < listBtnAnswer.Count; i++)
                {
                    var answerBtn = listBtnAnswer[i];
                    answerBtn.transform.DOScale(Vector3.one, 0.5f);

                }
            }, quizQuestResource.GetQuizDialogs(currentQuiz.quiz));
           
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

    
        



    }
    public void AnswerQuest(AnswerQuizQuest answer)
    {
        answerPrefab.RecycleAll();
        quizQuestParent.Hide();
        if (currentQuest is SideQuizQuestResource quizQuestResource)
        {
            if (answer.currentIndexAnswer == currentQuiz.rightAnswerIndex)
            {
                DoneQuest();
              


            }
            else
            {
                SayDialogManager.Instance.ShowDialog(nameChar, gender, skins, color, quizQuestResource.wrongDialog);

            }
        }

       


    }

    public override ActionMapResourceSO GetActionMapResourceSO()
    {
        return actionMapResourceSOs[UnityEngine.Random.Range(0, actionMapResourceSOs.Length)];
    }
}
