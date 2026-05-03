using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasAutoScaler : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    void Awake()
    {
        // Giữ Script này tồn tại xuyên suốt các Scene
        DontDestroyOnLoad(this.gameObject);

        // Đăng ký sự kiện mỗi khi load scene mới
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Chạy lần đầu cho Scene hiện tại
        ApplySmartScale();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySmartScale();
    }

    void ApplySmartScale()
    {
        // Tìm tất cả Canvas trong Scene mới load
        CanvasScaler[] scalers = FindObjectsOfType<CanvasScaler>();

        // Tính tỉ lệ màn hình thực tế (Width / Height)
        float screenRatio = (float)Screen.width / (float)Screen.height;

        // Tỉ lệ chuẩn 16:9 là ~1.77
        // Tỉ lệ iPad 4:3 là ~1.33

        float targetMatch;
        if (screenRatio < 1.5f)
        {
            // Màn hình vuông (iPad, Tablet) -> Ưu tiên Width để không bị tràn ngang
            targetMatch = 0f;
        }
        else
        {
            // Màn hình dài (iPhone, Android Phone) -> Ưu tiên Height để UI không bị bé
            targetMatch = 1f;
        }

        foreach (var scaler in scalers)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = targetMatch;
        }

        Debug.Log($"<color=green>Canvas Scaler updated!</color> Screen Ratio: {screenRatio}, Match set to: {targetMatch}");
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi object bị xóa
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}