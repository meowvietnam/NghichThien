using UnityEditor;
using UnityEngine;

public class MissingScriptsCleaner : Editor
{
    [MenuItem("Tools/Dọn Dẹp Missing Scripts (Toàn Bộ Con)")]
    static void CleanAllChildren()
    {
        // Lấy tất cả GameObject đang được chọn ở Hierarchy hoặc Project
        GameObject[] selectedObjects = Selection.gameObjects;
        int totalRemoved = 0;

        foreach (GameObject go in selectedObjects)
        {
            // Lấy tất cả Transform con (bao gồm cả chính nó)
            Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                // Hàm này của Unity cực mạnh: Xóa toàn bộ script missing trong 1 GameObject
                int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
                if (count > 0)
                {
                    totalRemoved += count;
                    Debug.Log($"<color=yellow>Đã xóa {count} script lỗi tại: {child.name}</color>");
                }
            }

            // Đánh dấu Object đã thay đổi để Unity lưu lại (tránh bị mất sau khi Ctrl+S)
            EditorUtility.SetDirty(go);
        }

        Debug.Log($"<color=green><b>Xong!</b> Tổng cộng đã dọn dẹp {totalRemoved} missing scripts.</color>");
    }
}
