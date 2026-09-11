using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class UniversalRecursiveDetector : EditorWindow
{
    [MenuItem("Tools/Kiểm Tra Lỗi/Quét Toàn Bộ Hàm Đệ Quy (Thiếu base)")]
    public static void ShowWindow()
    {
        GetWindow<UniversalRecursiveDetector>("Quét Lỗi Đệ Quy Tổng Thể");
    }

    private void OnGUI()
    {
        GUILayout.Label("Bộ quét tất cả các hàm gọi lại chính nó (Quên 'base')", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Tool sẽ quét toàn bộ file .cs để tìm các hàm override hoặc hàm thông thường có hành vi gọi lại chính tên hàm đó mà không đi kèm từ khóa 'base.'.",
            MessageType.Info);
        GUILayout.Space(10);

        if (GUILayout.Button("Bắt đầu quét toàn dự án", GUILayout.Height(40)))
        {
            ScanAllMethods();
        }
    }

    private static void ScanAllMethods()
    {
        string[] allFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        int errorCount = 0;

        Debug.Log($"<color=cyan>[Bắt đầu quét tổng thể]</color> Đang phân tích {allFiles.Length} file script...");

        // Regex nhận diện khai báo hàm chuẩn (bao gồm cả override, public, private, void, các kiểu trả về...)
        // Group 1: Tên hàm
        string methodDeclarationPattern =
            @"(?:public|protected|private|internal|static|\s)+[\w<>\[\]]+\s+(\w+)\s*\([^)]*\)\s*\{?";

        foreach (string file in allFiles)
        {
            if (file.Contains("UniversalRecursiveDetector") || file.Contains("Assembly-CSharp")) continue;

            string[] lines = File.ReadAllLines(file);
            string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                Match match = Regex.Match(line, methodDeclarationPattern);

                // Nếu dòng này là dòng khai báo hàm và không phải là gọi hàm thông thường
                if (match.Success && !line.Contains(";") && !line.Contains("return"))
                {
                    string methodName = match.Groups[1].Value;

                    // Bỏ qua các từ khóa hệ thống hoặc hàm khởi tạo/Property dễ bị nhận nhầm
                    if (methodName == "if" || methodName == "get" || methodName == "set" || methodName == "switch" ||
                        methodName == "catch")
                        continue;

                    // Tiến hành quét thân hàm bắt đầu từ dòng này để tìm lỗi
                    int openBraces = 0;
                    bool foundBodyStart = false;
                    string methodBodyText = "";
                    int bodyStartLine = i;

                    for (int j = i; j < lines.Length; j++)
                    {
                        string currentLine = lines[j];

                        // Đếm dấu ngoặc để xác định điểm kết thúc hàm
                        foreach (char c in currentLine)
                        {
                            if (c == '{')
                            {
                                openBraces++;
                                foundBodyStart = true;
                            }

                            if (c == '}') openBraces--;
                        }

                        if (foundBodyStart)
                        {
                            // Nếu đang ở dòng đầu tiên (dòng khai báo), chỉ lấy phần sau dấu {
                            if (j == i)
                            {
                                int idx = currentLine.IndexOf('{');
                                if (idx != -1) methodBodyText += currentLine.Substring(idx + 1) + "\n";
                            }
                            else
                            {
                                methodBodyText += currentLine + "\n";
                            }
                        }

                        // Khi openBraces về 0 nghĩa là đã đi hết thân hàm
                        if (foundBodyStart && openBraces <= 0)
                        {
                            // Kiểm tra xem trong thân hàm có gọi lại chính tên hàm đó không
                            // Kiểm tra "methodName(" nhưng loại trừ "base.methodName("
                            string callPattern = @"\b" + Regex.Escape(methodName) + @"\s*\(";
                            string baseCallPattern = @"\bbase\." + Regex.Escape(methodName) + @"\s*\(";

                            if (Regex.IsMatch(methodBodyText, callPattern))
                            {
                                if (!Regex.IsMatch(methodBodyText, baseCallPattern))
                                {
                                    // Loại trừ trường hợp tự đệ quy hợp lệ (có tham số thay đổi) nếu bạn muốn, 
                                    // nhưng vì bạn đang tìm lỗi quên 'base' nên cứ cảnh báo hết.
                                    errorCount++;
                                    var fileObj = AssetDatabase.LoadAssetAtPath<MonoScript>(relativePath);

                                    Debug.LogError(
                                        $"<color=red>[NGUY HIỂM - STACK OVERFLOW]</color> Phát hiện hàm gọi trùng tên thiếu 'base'!\n" +
                                        $"📍 File: {relativePath} (Dòng {i + 1})\n" +
                                        $"🛠️ Tên hàm bị lỗi: <color=yellow>{methodName}()</color>\n" +
                                        $"📝 Dòng khai báo: {line.Trim()}",
                                        fileObj
                                    );
                                }
                            }

                            break; // Thoát vòng lặp quét thân hàm này, chuyển sang tìm hàm tiếp theo
                        }
                    }
                }
            }
        }

        // Báo cáo kết quả
        if (errorCount == 0)
        {
            Debug.Log(
                "<color=green>[Hoàn tất]</color> Tuyệt vời! Không phát hiện hàm nào bị lỗi tự gọi đệ quy vô hạn.");
            EditorUtility.DisplayDialog("Kết quả", "Hoàn hảo! Không tìm thấy lỗi đệ quy nào thiếu base.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Kết quả",
                $"Phát hiện {errorCount} hàm có nguy cơ Stack Overflow! Vui lòng kiểm tra tab Console.", "OK");
        }
    }
}