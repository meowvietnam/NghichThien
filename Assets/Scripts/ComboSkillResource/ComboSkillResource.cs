using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllMaps.NgoDao;
using Base;
using MemoryPack;
using Sirenix.OdinInspector;
using Skill;
using Snapshot;
using UnityEditor;
using UnityEngine;

namespace ComboSkillResource
{
    [CreateAssetMenu(fileName = nameof(ComboSkillResource),
        menuName = "Combo Skill Resource/" + nameof(ComboSkillResource))]
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
                Debug.Log(ngoDaos.Sum(x => x.require));
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
        }


        [Serializable]
        public class ComboWithType
        {
            public string nameCombo;
            public Combo[] comboWithDaiCanhGioi;
            public BaseSkill.ETypeSkill[] eTypeSkill;
            public ETypeCombo typeCombo;
            public EMonPhai monPhai;
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
        [Button]
        public void Export()
        {
            var comboSkillResource = new Snapshot.ComboSkillResource(this);
            var bytes = MemoryPackSerializer.Serialize(comboSkillResource);
            var path = Path.Combine(Application.dataPath, "Exports", "combo-skill-resources.bin");
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            Debug.Log(path);
        }
    }
}