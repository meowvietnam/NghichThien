using AllMaps;
using LongDH.LongDHModules;
using System;
using UnityEngine;
using System.IO;
using System.Data;
using System.Collections.Generic;
using ExcelDataReader; // Hãy đảm bảo bạn đã import thư viện này qua DLL hoặc UPM
using Sirenix.OdinInspector; // Thêm thư viện Odin

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = nameof(SideQuizQuestResource), menuName = "Action Map/Side Quest/" + nameof(SideQuizQuestResource))]
public class SideQuizQuestResource : ActionMapResourceSO
{
    [Serializable]
    public class Quiz {

        public string quiz;
        public string[] answer;
        public int rightAnswerIndex;
    }
    public Quiz[] quizDatas;
    
    public override void ActiveEventMap(Map map)
    {

        Pool.Get<QuestManager>().QuizQuestInitShow(quizDatas[UnityEngine.Random.Range(0,quizDatas.Length)]);
    }
#if UNITY_EDITOR
    // Sử dụng [Button] của Odin để tạo nút bấm tùy chỉnh siêu nhanh
    [Button("IMPORT QUIZ FROM EXCEL", ButtonSizes.Large, ButtonStyle.Box)]
    [GUIColor(0.3f, 0.8f, 1f)] // Thêm màu xanh Cyan cho nút bấm nổi bật
    public void ImportFromExcel()
    {
        // Mở bảng chọn file Excel (.xlsx) từ máy tính
        string filePath = EditorUtility.OpenFilePanel("Chọn file Excel Quiz", "", "xlsx");
        if (string.IsNullOrEmpty(filePath)) return;

        // Đăng ký provider để đọc đúng định dạng font tiếng Việt có dấu (UTF-8)
      //  System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        try
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true } // Bỏ qua dòng tiêu đề (Header) đầu tiên trong Excel
                    });

                    DataTable table = result.Tables[0];
                    List<Quiz> tempQuizList = new List<Quiz>();

                    foreach (DataRow row in table.Rows)
                    {
                        string quizText = row[0]?.ToString();
                        if (string.IsNullOrEmpty(quizText)) continue; // Bỏ qua nếu dòng bị trống
                        
                        // Đọc 4 đáp án từ cột index 1 đến 4
                        string[] answers = new string[4];
                        answers[0] = row[1]?.ToString() ?? "";
                        answers[1] = row[2]?.ToString() ?? "";
                        answers[2] = row[3]?.ToString() ?? "";
                        answers[3] = row[4]?.ToString() ?? "";

                        // Đọc index của câu trả lời đúng (cột số 5 trong file Excel)
                        int rightIndex = 0;
                        if (row[5] != null)
                        {
                            int.TryParse(row[5].ToString(), out rightIndex);
                        }

                        Quiz newQuiz = new Quiz
                        {
                            quiz = quizText,
                            answer = answers,
                            rightAnswerIndex = rightIndex
                        };

                        tempQuizList.Add(newQuiz);
                    }

                    // Gán mảng dữ liệu mới
                    quizDatas = tempQuizList.ToArray();

                    // Đánh dấu để Unity lưu lại thay đổi dữ liệu asset này
                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssets();

                    Debug.Log($"<color=green>[Odin Import] Thành công! Đã nạp {quizDatas.Length} câu hỏi vào ScriptableObject.</color>");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[Odin Import] Gặp lỗi khi xử lý file Excel: " + e.Message);
        }
    }
#endif
}
