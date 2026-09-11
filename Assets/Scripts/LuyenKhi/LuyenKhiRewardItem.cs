using System;
using System.IO;
using LuyenDan;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class LuyenKhiRewardItem : IMemorySerializable<LuyenKhiRewardItem>
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public EquipmentTier Tier { get; set; }
        [field: SerializeField] public ArtifactType Type { get; set; }
        private Sprite icon;
        private Sprite iconFight;
        [field: SerializeField] public string IconPath { get; set; }
        [field: SerializeField] public string IconFightPath { get; set; }

        public Sprite Icon
        {
            get
            {
                if (icon == null)
                {
                    icon = Resources.Load<Sprite>(IconPath);
                }

                return icon;
            }
        }

        public Sprite IconFight
        {
            get
            {
                if (string.IsNullOrWhiteSpace(IconFightPath))
                {
                    return iconFight;
                }

                if (iconFight == null)
                {
                    iconFight = Resources.Load<Sprite>(IconFightPath);
                }

                return iconFight;
            }
        }


        public void Deserialize(IReader reader, ref LuyenKhiRewardItem value)
        {
            value.Id = reader.Read<int>();
            value.Tier = reader.Read<EquipmentTier>();
            value.Type = reader.Read<ArtifactType>();
        }

        public int ByteSize()
        {
            return Id.ByteSize() + Tier.ByteSize() + Type.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Tier);
            writer.Write(Type);
        }
#if UNITY_EDITOR
        public void Move()
        {
            var icon = Icon;
            var iconFight = IconFight;
            var root = "Assets/Resources/";
            var path = Path.GetDirectoryName(root + IconPath);
            var fightPath = Path.GetDirectoryName(root + IconFightPath);
            AssetOptimize.Move(icon.texture, path);
            if (IconFight != null)
            {
                AssetOptimize.Move(iconFight.texture, fightPath);
            }
        }
#endif
    }
}