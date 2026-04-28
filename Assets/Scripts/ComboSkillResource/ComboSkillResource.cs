using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using AllMaps.NgoDao;
using Base;
using Skill;

[CreateAssetMenu(fileName = nameof(ComboSkillResource), menuName = "Combo Skill Resource/" + nameof(ComboSkillResource))]

public class ComboSkillResource : ScriptableObject
{
    public enum ETypeCombo
    {
        TrungDoc,
        TrienNhieu,
        MocThuy,
        ThieuDot,
        KimKiem,
        ThuyKiem,
        Tho,
        Kiem
    
    }

    [Serializable]
    public class Combo
    {
        public List<ThanThong> thanThongs;
        public List<CongPhap> congPhaps;
        public List<NgoDao> ngoDaos;
        [Button]
        public void LogRequireNgoDao()
        {
            Debug.Log(ngoDaos.Sum(x=>x.require));
        }
        [Button]
        public void LogTierCongPhap()
        {
            foreach (var item in congPhaps)
            {
                Debug.Log(item.eTierSkill);
            } 
                
        }
        [Button]
        public void LogTierThanThong()
        {
            foreach (var item in thanThongs)
            {
                Debug.Log(item.eTierSkill);
            }

        }
        // trang bị nữa
    }
    [Serializable]
    public class ComboWithType
    {
        public string nameCombo;
        public Combo[] comboWithDaiCanhGioi;
        public BaseSkill.ETypeSkill[] eTypeSkill;
        public ETypeCombo typeCombo;
        public EMonPhai monPhai;
        // trang bị nữa
    }
    public ComboWithType[] comboNPC;
    private static ComboSkillResource instance;

    public static ComboSkillResource Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ComboSkillResource>("Combo Skill Resource");
            }

            return instance;
        }
    }
}
