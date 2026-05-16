using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideQuestManager : MonoBehaviour
{
    public enum ETypeSideQuest
    { 
        Quiz,
        Combat,
        Delivery
    }


    [Header("Quiz Quest Visual")]
    [SerializeField] private Button answerPrefab;
    [SerializeField] private SideQuizQuestData.Quiz currentQuiz;
    public int timeMonthRandomSideQuest => 5;

    private void Awake()
    {
        Pool.Set(this);
    }
    public void QuizQuestInitShow(SideQuizQuestData.Quiz quiz)
    {
        List<string> contentText = new List<string>
        {
            "Đạo hữu dừng bước! Ta thấy ngươi căn cốt kinh kỳ, nhưng đường tu tiên cốt ở ngộ tính, chứ không chỉ có man lực.",
            "Ta có vài câu hỏi về thiên địa quy luật. Trả lời đúng, ta sẽ tặng ngươi tiên ngọc.",
            "Ngươi đã sẵn sàng chưa? Hãy nghe kỹ câu hỏi đầu tiên...",
            $"{quiz.quiz}"
        };
        currentQuiz = quiz;
        for (int i = 0; i < currentQuiz.answer.Length; i++)
        {
            var answerBtn = answerPrefab.Spawn(answerPrefab.transform.parent);
            answerBtn.transform.localScale = Vector3.zero;
            answerBtn.transform.DOScale(Vector3.one,0.5f);
        } 
        void InitActionBtnQuiz()
        {

        }

            
    }    


    
}
