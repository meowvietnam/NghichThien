using Sirenix.OdinInspector;
using UnityEngine;

public class NPCData : ScriptableObject
{
    [SerializeField] public CharacterStatsSecure charStats;
    [SerializeField] public CharacterStatsSecure charStatsCreateRef;
    [SerializeField] public ComboSkillResource.ComboWithType comboChar;
    [SerializeField] public ComboSkillResource.Combo CurrentCombo => comboChar.comboWithDaiCanhGioi[ThanThong.GetDaiCanhGioi(charStats.level)-1];
    [Button]
    public void LogData()
    {
        Debug.Log(charStats.percentKim.Value);
    }
    [Button]
    public void SetBaseStats(GameConfig gameConfig)
    {
        charStats = charStatsCreateRef.Clone();
        for (int i = 0; i < charStats.level; i++)
        {
            charStats.hpMax += gameConfig.inforLevels[i].hpMaxBuff;
            charStats.hp = charStats.hpMax;
            charStats.thanThuc += gameConfig.inforLevels[i].thanThucBuff;
            charStats.tocDoBay += gameConfig.inforLevels[i].donTocBuff;
            charStats.thoNguyen += gameConfig.inforLevels[i].thoNguyenBuff;

        }
        int daiCanhGioi = ThanThong.GetDaiCanhGioi(charStats.level);
        int minRandom = 0;
        int maxRandom = 0;
        if(daiCanhGioi == 1)
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
        foreach (var item in CurrentCombo.ngoDaos)
        {
            charStats.ngoDaoSaves.Add(item.Id);
        }
        foreach (var item in CurrentCombo.congPhaps)
        {
            charStats.congPhapOwner.Add(new CharacterStatsSecure.DataSave { id = item.Id , value = 5});
            

        }
        foreach (var item in CurrentCombo.thanThongs)
        {
            charStats.thanThongEquip.Add(item.Id);
            charStats.thanThongOwner.Add(item.Id);
        }
        charStats.chuTu1Used = charStats.congPhapOwner.Count > 0 ? charStats.congPhapOwner[0].id : -1;
        charStats.phuTu1Used = charStats.congPhapOwner.Count > 1 ? charStats.congPhapOwner[1].id : -1;
        charStats.phuTu2Used = charStats.congPhapOwner.Count > 2 ? charStats.congPhapOwner[2].id : -1;
        charStats.phuTu3Used = charStats.congPhapOwner.Count > 3 ? charStats.congPhapOwner[3].id : -1;
        charStats.phuTu4Used = charStats.congPhapOwner.Count > 4 ? charStats.congPhapOwner[4].id : -1;
        charStats.chuTu2Used = charStats.congPhapOwner.Count > 5 ? charStats.congPhapOwner[5].id : -1;
    }
}
