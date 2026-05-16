using AllMaps;
using LongDH.LongDHModules;
using System;
using UnityEngine;

public class SideQuizQuestData : ScriptableObject, IActionMap
{
    [Serializable]
    public class Quiz {

        public string quiz;
        public string[] answer;
        public int rightAnswerIndex;
    }
    public Quiz[] quizDatas;
  

    public void EnterMap(Map map)
    {
        Pool.Get<SideQuestManager>().QuizQuestInitShow(quizDatas[UnityEngine.Random.Range(0,quizDatas.Length)]);
    }
}
