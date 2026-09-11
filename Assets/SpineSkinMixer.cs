using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SpineSkinMixer : MonoBehaviour
{
    // Kéo thả component SkeletonAnimation vào đây trong Inspector
    [SerializeField] private SkeletonGraphic skeletonGraphic;

    [Tooltip("Danh sách các tiền tố Skin (ví dụ: 'a_hair', 'suit'). Điền tự động sau khi chạy Debug.")]
    public List<string> skinPrefixes = new List<string>();

    [Header("Randomization Settings")]
    [Tooltip("Tỷ lệ MẶC ĐỊNH sử dụng một Skin từ một nhóm. 1.0f nghĩa là luôn chọn 1 skin trong nhóm đó.")]
    [Range(0f, 1f)]
    public float defaultChanceToUseSkin = 1.0f; // Giữ 1.0f nếu không muốn skip ngẫu nhiên

    [Tooltip("Dùng để GHI ĐÈ tỷ lệ mặc định cho các Prefixes đặc biệt (ví dụ: a_mask = 0.5f).")]
    public List<SpecificChance> specificChances = new List<SpecificChance>();

    [Header("Exclusion/Blacklist")] [Tooltip("Danh sách các Skin/Prefix cần loại trừ khỏi Randomizer.")]
    public List<BlacklistEntry> blacklistedSkinIDs = new List<BlacklistEntry>();


    [System.Serializable]
    public class SpecificChance
    {
        // [SpineSkin(dataField: "skeletonAnimation")] 
        public string prefix = "a_mask";
        [Range(0f, 1f)] public float chance = 0.5f;
    }

    [System.Serializable]
    public class BlacklistEntry
    {
        // [SpineSkin(dataField: "skeletonAnimation")] 
        [Tooltip("Tiền tố của nhóm skin cần loại trừ.")]
        public string prefix = "a_suit";

        [Tooltip("Các ID skin cần loại trừ, cách nhau bằng dấu phẩy (ví dụ: 91, 92, 93-101).")]
        public string IDsToExclude = "91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101";
    }

    // =================================================================
    // CÁC HÀM START, UPDATE, DEBUG VÀ UTILITY KHÔNG THAY ĐỔI
    // (Tôi lược bớt để tập trung vào hàm Randomize)
    // =================================================================
    void Start()
    {
        if (skeletonGraphic == null)
        {
            skeletonGraphic = GetComponent<SkeletonGraphic>();
        }
    }

    // Hàm DebugAndPopulateSkinPrefixes (giữ nguyên)
    [ContextMenu("Debug & Populate Skin Prefixes")]
    public void DebugAndPopulateSkinPrefixes()
    {
        if (skeletonGraphic == null || skeletonGraphic.skeletonDataAsset == null)
        {
            Debug.LogError("Skeleton Data hoặc Asset chưa được gán.");
            return;
        }

        SkeletonData data = skeletonGraphic.skeletonDataAsset.GetSkeletonData(false);
        HashSet<string> uniquePrefixes = new HashSet<string>();
        Debug.Log("--- Danh sách Tên Skin Đã Lọc ---");

        foreach (Skin skin in data.Skins)
        {
            string originalName = skin.Name;
            if (originalName.ToLower() == "default") continue;

            int lastUnderscoreIndex = originalName.LastIndexOf('_');

            if (lastUnderscoreIndex != -1 && originalName.Length > lastUnderscoreIndex + 1)
            {
                string refinedName = originalName.Substring(0, lastUnderscoreIndex);
                string suffix = originalName.Substring(lastUnderscoreIndex + 1);

                if (Regex.IsMatch(suffix, @"^\d+$"))
                {
                    uniquePrefixes.Add(refinedName);
                    Debug.Log(refinedName);
                }
            }
        }

        skinPrefixes.Clear();
        skinPrefixes.AddRange(uniquePrefixes.OrderBy(p => p));

        Debug.Log("--------------------------------");
        Debug.Log($"Đã tìm thấy và cập nhật {skinPrefixes.Count} tiền tố Skin vào danh sách 'skinPrefixes'.");
    }


    private Dictionary<string, List<string>> GroupSkinsByPrefix(List<Skin> allSkins)
    {
        // ... (Giữ nguyên logic của hàm này) ...
        Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
        foreach (Skin skin in allSkins)
        {
            string originalName = skin.Name;
            if (originalName.ToLower() == "default" || !originalName.Contains("_")) continue;
            int lastUnderscoreIndex = originalName.LastIndexOf('_');
            if (lastUnderscoreIndex != -1)
            {
                string prefix = originalName.Substring(0, lastUnderscoreIndex);
                string suffix = originalName.Substring(lastUnderscoreIndex + 1);
                if (Regex.IsMatch(suffix, @"^\d+$"))
                {
                    if (!groups.ContainsKey(prefix)) groups.Add(prefix, new List<string>());
                    groups[prefix].Add(originalName);
                }
            }
        }

        return groups;
    }

    private float GetChance(string prefix)
    {
        SpecificChance specific = specificChances.FirstOrDefault(sc => sc.prefix == prefix);
        if (specific != null) return specific.chance;
        return defaultChanceToUseSkin;
    }


    // =================================================================
    // HÀM RANDOMIZE VÀ ÁP DỤNG SKIN (ĐÃ SỬA ĐỔI)
    // =================================================================
    [ContextMenu("Randomize and Apply Skin")]
    public void RandomizeAndApplySkin()
    {
        if (skeletonGraphic == null || skeletonGraphic.skeletonDataAsset == null)
        {
            Debug.LogError("Skeleton Data Asset chưa được gán.");
            return;
        }

        SkeletonData data = skeletonGraphic.skeletonDataAsset.GetSkeletonData(true);
        if (data == null)
        {
            Debug.LogError("Không thể lấy Skeleton Data từ Asset.");
            return;
        }

        var groupedSkins = GroupSkinsByPrefix(data.Skins.ToList());
        Skin mixedSkin = new Skin("RandomMixedSkin");
        Skin defaultSkin = data.FindSkin("default");
        if (defaultSkin != null) mixedSkin.AddSkin(defaultSkin);

        Debug.Log("--- Trộn Skin Ngẫu Nhiên ---");

        // Lặp qua các tiền tố skin bạn muốn random
        foreach (string prefix in skinPrefixes)
        {
            // 1. Kiểm tra logic SKIP ngẫu nhiên cho toàn bộ prefix
            float chance = GetChance(prefix);
            if (Random.value > chance)
            {
                Debug.Log($"[Bỏ qua] {prefix} (Xác suất {chance * 100}%)");
                continue;
            }

            // 2. Tiếp tục với logic random skin (chỉ khi không bị skip)
            if (groupedSkins.ContainsKey(prefix) && groupedSkins[prefix].Any())
            {
                // Lấy danh sách skin gốc của nhóm
                List<string> skinsInGroup = groupedSkins[prefix];

                // --- LOGIC BỔ SUNG: LỌC DANH SÁCH ĐEN (BLACKLIST) ---
                List<string> availableSkins = FilterBlacklistedSkins(prefix, skinsInGroup);

                if (!availableSkins.Any())
                {
                    Debug.LogWarning($"Nhóm {prefix} đã bị lọc hết Skin. Bỏ qua.");
                    continue;
                }

                // Chọn ngẫu nhiên một skin từ danh sách ĐÃ LỌC
                string randomSkinName = availableSkins[UnityEngine.Random.Range(0, availableSkins.Count)];

                Skin randomSkin = data.FindSkin(randomSkinName);
                if (randomSkin != null)
                {
                    mixedSkin.AddSkin(randomSkin);
                    Debug.Log($"[Đã thêm] {prefix}: {randomSkinName}");
                }
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy Skin trong nhóm: {prefix}");
            }
        }

        skeletonGraphic.Skeleton.SetSkin(mixedSkin);
        skeletonGraphic.Skeleton.SetToSetupPose();
        skeletonGraphic.Update(0);

        Debug.Log("-------------------------------");
    }

    // =================================================================
    // HÀM LỌC DANH SÁCH ĐEN MỚI (NEW UTILITY FUNCTION)
    // =================================================================

    /// <summary>
    /// Lọc danh sách skinName dựa trên BlacklistEntry đã được cấu hình.
    /// </summary>
    private List<string> FilterBlacklistedSkins(string prefix, List<string> originalList)
    {
        // 1. Tìm BlacklistEntry cho prefix hiện tại
        BlacklistEntry entry =
            blacklistedSkinIDs.FirstOrDefault(e => e.prefix.Equals(prefix, System.StringComparison.OrdinalIgnoreCase));

        if (entry == null || string.IsNullOrEmpty(entry.IDsToExclude))
        {
            return originalList; // Không có blacklist, trả về danh sách gốc
        }

        // 2. Chuyển đổi chuỗi IDsToExclude thành HashSet các số nguyên để tra cứu nhanh hơn
        HashSet<int> excludedIDs = new HashSet<int>();

        // Chia chuỗi theo dấu phẩy
        string[] parts = entry.IDsToExclude.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            string trimmedPart = part.Trim();
            if (string.IsNullOrEmpty(trimmedPart)) continue;

            // Xử lý dải ID (ví dụ: 93-101)
            if (trimmedPart.Contains('-'))
            {
                string[] rangeParts = trimmedPart.Split('-');
                if (rangeParts.Length == 2 && int.TryParse(rangeParts[0].Trim(), out int start) &&
                    int.TryParse(rangeParts[1].Trim(), out int end))
                {
                    for (int i = start; i <= end; i++)
                    {
                        excludedIDs.Add(i);
                    }
                }
            }
            // Xử lý ID đơn lẻ (ví dụ: 91)
            else if (int.TryParse(trimmedPart, out int singleID))
            {
                excludedIDs.Add(singleID);
            }
        }

        // Nếu không có ID nào để loại trừ, trả về danh sách gốc
        if (excludedIDs.Count == 0)
        {
            return originalList;
        }

        // 3. Lọc danh sách skin
        List<string> filteredList = new List<string>();
        string prefixWithUnderscore = prefix + "_";

        foreach (string skinName in originalList)
        {
            // Tên skin phải bắt đầu bằng prefix (đã được đảm bảo bởi GroupSkinsByPrefix, nhưng kiểm tra lại)
            if (skinName.StartsWith(prefixWithUnderscore))
            {
                // Cắt lấy phần ID (số)
                if (int.TryParse(skinName.Substring(prefixWithUnderscore.Length), out int skinID))
                {
                    // Nếu ID không nằm trong danh sách loại trừ, thì thêm vào danh sách lọc
                    if (!excludedIDs.Contains(skinID))
                    {
                        filteredList.Add(skinName);
                    }
                    else
                    {
                        Debug.Log($"[Loại trừ] Đã loại trừ {skinName} khỏi vòng Random.");
                    }
                }
                else
                {
                    // Nếu không phải là số (Lỗi data skin)
                    filteredList.Add(skinName);
                }
            }
            else
            {
                // Nếu không đúng định dạng (không nên xảy ra)
                filteredList.Add(skinName);
            }
        }

        return filteredList;
    }
}