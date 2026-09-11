#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class SetIDTools
{
    [MenuItem("Tools/MyGame/Set IDs for Selected Folder")]
    public static void SetIDsInFolder()
    {
        // Lấy các đối tượng đang được chọn trong cửa sổ Project
        Object[] selectedObjects = Selection.GetFiltered(typeof(BaseData), SelectionMode.DeepAssets);

        int count = 0;
        foreach (Object obj in selectedObjects)
        {
            // Thay 'YourBaseClass' bằng tên Class cha của các file ThanThong kia
            if (obj is BaseData item && item.spriteItem != null)
            {
                // Thực hiện logic set ID tương tự hàm của bạn
                // Ép kiểu hoặc dùng Reflection nếu id là field protected
                Undo.RecordObject(item, "Set ID Auto");

                // Vì id là protected, bạn có thể gọi public method SetID() nếu có
                item.SetId();

                EditorUtility.SetDirty(item);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Successfully updated IDs for {count} items!");
    }
}
#endif