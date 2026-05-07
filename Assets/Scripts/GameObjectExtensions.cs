using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetLayerRecursive(this GameObject obj, int newLayer)
    {
        if (obj == null) return;

        // Gán layer cho object hiện tại
        obj.layer = newLayer;

        // Duyệt qua tất cả các con trực tiếp và gọi lại hàm này
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetLayerRecursive(newLayer);
        }
    }
    public static void SetLayerRecursiveWithCamera(this GameObject obj, int mask)
    {
        if (obj == null) return;

        // Tự động chuyển từ CullingMask sang Layer Index (lấy bit đầu tiên)
        // Nếu mask truyền vào là 0-31 thì giữ nguyên, nếu là bitmask (2^n) thì chuyển đổi
        int layerIndex = mask;
        if (mask > 31)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    layerIndex = i;
                    break;
                }
            }
        }

        // Bảo vệ: Nếu sau khi chuyển đổi vẫn sai thì thoát
        if (layerIndex < 0 || layerIndex > 31) return;

        // Gán layer cho cha
        obj.layer = layerIndex;

        // ĐỆ QUY: Gọi lại đúng tên hàm này cho các con
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetLayerRecursiveWithCamera(layerIndex);
        }
    }
}