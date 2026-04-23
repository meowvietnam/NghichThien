using System;
using System.Collections.Generic;
using System.Linq;
using LuyenKhi;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class LuyenKhiRewardItem
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public EquipmentTier Tier { get; set; }
        [field: SerializeField] public SubEquipmentType Type { get; set; }
        [field: SerializeField, PreviewField(75)] public Sprite Icon { get; set; }
        private static Dictionary<string, SubEquipmentType> d = new ()
        {
            {"huan", SubEquipmentType.ChiHoan},
            {"jia", SubEquipmentType.GiapTru},
            {"jian", SubEquipmentType.PhiKiem},
            {"ling", SubEquipmentType.BaoLenh},
            {"pao", SubEquipmentType.TruongBao},
            {"xia", SubEquipmentType.BaoHap},
            {"yin", SubEquipmentType.BaoAn},
            {"zhen", SubEquipmentType.PhiCham},
            {"zhong", SubEquipmentType.BaoChung},
            {"zhu", SubEquipmentType.Linh},
        };

        public void Load()
        {
            var name = Icon.name;
            var type = name.Split('_')[2];
            try
            {
                Type = d[type];
            }
            catch (KeyNotFoundException)
            {
                Debug.Log(type);
            }
        }
    }
}