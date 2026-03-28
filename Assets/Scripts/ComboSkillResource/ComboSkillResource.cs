using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;

[CreateAssetMenu(fileName = nameof(ComboSkillResource), menuName = "Combo Skill Resource/" + nameof(ComboSkillResource))]

public class ComboSkillResource : ScriptableObject
{
   
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
        // trang bị nữa
    }
    public ComboWithType[] comboNPC;

}
