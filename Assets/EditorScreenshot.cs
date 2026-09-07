using UnityEngine;
using System.IO;

public class EditorScreenshot : MonoBehaviour
{
    [Header("Cấu hình lưu ảnh")] [SerializeField]
    private string folderName = "Screenshots";

    void Update()
    {
        // Kiểm tra nếu bấm phím Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CaptureGameView();
        }
    }

    private void CaptureGameView()
    {
        // Tạo đường dẫn đến thư mục chứa ảnh (ngang hàng với thư mục Assets)
        string directoryPath = Path.Combine(Application.dataPath, "../", folderName);

        // Nếu thư mục chưa tồn tại thì tự động tạo mới
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Đặt tên file theo mốc thời gian để không bị trùng (Định dạng: NămThángNgày_GiờPhútGiây)
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Screenshot_{timestamp}.png";
        string fullPath = Path.Combine(directoryPath, fileName);

        // Chụp màn hình với độ phân giải gốc của Game View (Kích cỡ tương đương tỉ lệ 1x)
        ScreenCapture.CaptureScreenshot(fullPath, 1);

        // In đường dẫn ra Console để bạn click vào mở cho nhanh
        Debug.Log($"<color=green><b>[Screenshot]</b> Đã lưu ảnh thành công tại:</color> {fullPath}");
    }
}