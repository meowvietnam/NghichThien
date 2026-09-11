using AllMaps;
using ComboSkillResource.NPCData;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using MemoryPack;
using Sirenix.OdinInspector;
using Skill;
using UnityEngine;

public partial class EnemyData : NPCData
{
    public override ETypeVisualChar eTypeVisualChar => eTypeVisualCharEnemy == ETypeVisualChar.Character
        ? GameManager.levelWithVisualChar[charStats.level]
        : eTypeVisualCharEnemy;

    public ETypeVisualChar eTypeVisualCharEnemy = ETypeVisualChar.Character;

    // Tiền tố: Chỉ thuộc tính/màu sắc (Hệ)
    protected override string[] Surnames => new[]
    {
        "Hắc", "Bạch", "Xích", "Thanh", "Kim", "Tử", "U", "Huyết", "Ma", "Lôi",
        "Băng", "Hỏa", "Độc", "Thiên", "Địa", "Cổ", "Hoang", "Ảo", "Linh", "Thái"
    };

    // Trung tố: Chỉ đặc điểm hoặc trạng thái
    protected override string[] MiddleNames => new[]
    {
        "Ảnh", "Sát", "Hồn", "Phách", "Cự", "Yêu", "Ma", "Minh", "Vô", "Thiết",
        "Giáp", "Lân", "Nhãn", "Dực", "Trảo", "Vĩ", "Phệ", "Cốt", "Thần", "Thánh"
    };

    // Hậu tố: Danh xưng chung cho sinh vật (Không phân loại loài)
    protected override string[] LastNames => new[]
    {
        "Thú", "Vật", "Giả", "Thể", "Hồn", "Tinh", "Quái", "Ma", "Yêu", "Xác",
        "Binh", "Tướng", "Vương", "Hoàng", "Đế", "Nghiệt", "Ma", "Biến", "Sư", "Tử"
    };


    [Button]
    private void SetHpChia(float soChia)
    {
        var newValue = (int)(charStats.hpMax / soChia);
        charStats.hpMax = newValue;
        charStats.hp = charStats.hpMax.Value;
    }

    public override void SetBaseStats(GameManager gameConfig)
    {
        if (string.IsNullOrEmpty(charStatsCreateRef.nameUser))
        {
            charStatsCreateRef.nameUser = GenerateName();
        }

        charStats = charStatsCreateRef.Clone();
        for (int i = 0; i < charStats.level; i++)
        {
            charStats.hpMax += gameConfig.inforLevels[i].hpMaxBuff;
            charStats.hp = charStats.hpMax;
            charStats.thanThuc += gameConfig.inforLevels[i].thanThucBuff;
            charStats.tocDoBay += gameConfig.inforLevels[i].donTocBuff;
            charStats.thoNguyen += gameConfig.inforLevels[i].thoNguyenBuff;
        }

        float baseHp = charStats.hpMax * 0.5f;

        int daiCanhGioi = ThanThong.GetDaiCanhGioi(charStats.level);

        float levelModifier = 1.0f + (charStats.level - 1) * 0.15f;

        // 4. Tính hệ số đột biến khi tăng Đại Cảnh Giới
        // Mỗi đại cảnh giới quái sẽ mạnh lên hẳn (ví dụ: nhân thêm 1.5 lần, 2.0 lần...)
        float canhGioiModifier = Mathf.Pow(1.5f, daiCanhGioi);

        // 5. Áp dụng công thức tổng
        int finalHp = Mathf.RoundToInt(baseHp * levelModifier * canhGioiModifier);

        // Gán lại cho Enemy
        charStats.hpMax = finalHp;
        charStats.hp = finalHp;


        int minRandom = 0;
        int maxRandom = 0;
        if (daiCanhGioi == 1)
        {
            minRandom = 15;
            maxRandom = 65;
        }
        else if (daiCanhGioi == 2)
        {
            minRandom = 35;
            maxRandom = 125;
        }
        else if (daiCanhGioi == 3)
        {
            minRandom = 65;
            maxRandom = 150;
        }
        else if (daiCanhGioi == 4)
        {
            minRandom = 100;
            maxRandom = 200;
        }
        else if (daiCanhGioi == 5)
        {
            minRandom = 500;
            maxRandom = 1000;
        }

        charStats.tuoiTac = Random.Range(minRandom, maxRandom);
        charStats.ngoDaoSaves.Clear();
        charStats.congPhapOwner.Clear();
        charStats.thanThongOwner.Clear();
        charStats.thanThongEquip.Clear();
        foreach (var item in CurrentCombo.ngoDaos)
        {
            charStats.ngoDaoSaves.Add(item.Id);
        }

        foreach (var item in CurrentCombo.congPhapIds)
        {
            charStats.congPhapOwner.Add(new CharacterStatsSecure.DataSave { id = item, value = 5 });
        }

        foreach (var item in CurrentCombo.thanThongIds)
        {
            charStats.thanThongEquip.Add(item);
            charStats.thanThongOwner.Add(item);
        }

        charStats.chuTu1Used = charStats.congPhapOwner.Count > 0 ? charStats.congPhapOwner[0].id : -1;
        charStats.phuTu1Used = charStats.congPhapOwner.Count > 1 ? charStats.congPhapOwner[1].id : -1;
        charStats.phuTu2Used = charStats.congPhapOwner.Count > 2 ? charStats.congPhapOwner[2].id : -1;
        charStats.phuTu3Used = charStats.congPhapOwner.Count > 3 ? charStats.congPhapOwner[3].id : -1;
        charStats.phuTu4Used = charStats.congPhapOwner.Count > 4 ? charStats.congPhapOwner[4].id : -1;
        charStats.chuTu2Used = charStats.congPhapOwner.Count > 5 ? charStats.congPhapOwner[5].id : -1;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this); // Nếu charStatsCreateRef nằm trong script này
        UnityEditor.AssetDatabase.SaveAssets(); // Lưu lại file vật lý
#endif
    }
}