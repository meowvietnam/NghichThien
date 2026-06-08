using Sirenix.OdinInspector;
using Skill;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/SkillResource")]

public class SkillResource : ScriptableObject
{
    private static SkillResource instance;

    public static SkillResource Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SkillResource>("Skill/SkillResource");
            }

            return instance;
        }
    }
    public ThanThong[] thanThongs;
    public CongPhap[] congPhaps;
    [Button]
    private void OnUpdateDataThanThong()
    {
#if UNITY_EDITOR
        // Tìm tất cả GUID của các asset có type là ThanThong
        string[] guids = AssetDatabase.FindAssets("t:ThanThong");

        thanThongs = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ThanThong>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        EditorUtility.SetDirty(this); // Đánh dấu object đã thay đổi để Unity lưu lại
        Debug.Log($"Đã cập nhật {thanThongs.Length} ThanThong vào mảng.");
#endif
    }

    [Button]
    private void OnUpdateDataCongPhap()
    {
#if UNITY_EDITOR
        // Tìm tất cả GUID của các asset có type là CongPhap
        string[] guids = AssetDatabase.FindAssets("t:CongPhap");

        congPhaps = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<CongPhap>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        EditorUtility.SetDirty(this);
        Debug.Log($"Đã cập nhật {congPhaps.Length} CongPhap vào mảng.");
#endif
    }
    [Button]
    public void SetTimeLearnSkill()
    {
        foreach (var item in thanThongs)
        {
            item.SetBaseTimeLearn();
#if UNITY_EDITOR
            // Đánh dấu file Thần Thông này đã bị sửa đổi
            EditorUtility.SetDirty(item);
#endif
        }
        foreach (var item in congPhaps)
        {
            item.SetBaseTimeLearn();
#if UNITY_EDITOR
            // Đánh dấu file Công Pháp này đã bị sửa đổi
            EditorUtility.SetDirty(item);
#endif

        }
#if UNITY_EDITOR
        // Đánh dấu file tổng này thay đổi (để lưu danh sách nếu có thay đổi)
        EditorUtility.SetDirty(this);

        // Ép Unity ghi toàn bộ các file đã đánh dấu ở trên xuống ổ cứng
        AssetDatabase.SaveAssets();

        // Buộc Editor vẽ lại giao diện ngay lập tức để cập nhật số liệu mới hiển thị
        AssetDatabase.Refresh();

        Debug.Log("Đã lưu data thành công cho từng Thần Thông và Công Pháp!");
#endif
    }
}
