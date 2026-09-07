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
#if UNITY_EDITOR
using UnityEditor;
#endif
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
            public List<int> thanThongIds;
            public List<int> congPhapIds;
            public List<NgoDao> ngoDaos;

            [Button]
            public void ConvertSkillToId()
            {
                thanThongIds.Clear();
                congPhapIds.Clear();
            }

            [Button]
            public void LogRequireNgoDao()
            {
                Debug.Log(ngoDaos.Sum(x => x.require));
            }

            [Button]
            public void LogTierCongPhap()
            {
            }

            [Button]
            public void LogTierThanThong()
            {
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

        [Button("Convert All Skills To Id")]
        public void ConvertAllSkillsToId()
        {
            if (comboNPC == null) return;

            foreach (var comboWithType in comboNPC)
            {
                if (comboWithType.comboWithDaiCanhGioi == null) continue;

                foreach (var combo in comboWithType.comboWithDaiCanhGioi)
                {
                    combo?.ConvertSkillToId();
                }
            }

#if UNITY_EDITOR
            // Đánh dấu ScriptableObject là "dirty" để Unity lưu thay đổi
            EditorUtility.SetDirty(this);
#endif

            Debug.Log("All skills converted to IDs successfully.");
        }
        //public static ComboSkillResource Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = Resources.Load<ComboSkillResource>("Combo Skill Resource");
        //        }

        //        return instance;
        //    }
        //}
#if UNITY_EDITOR

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
#endif
    }
}