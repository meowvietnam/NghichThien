using Base;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LuyenKhi
{
    [CreateAssetMenu]
    public class LuyenKhiResources : ScriptableObject, IMemorySerializable<LuyenKhiResources>
    {
        [field: SerializeField] public MaterialGradeConfig[] MaterialGradeConfigs { get; private set; }
        [field: SerializeField] public EquipmentIcon[] EquipmentIcons { get; private set; }
        [field: SerializeField] public EquipmentTierConfig[] EquipmentTiers { get; private set; }
        [field: SerializeField] public EquipmentGradeConfig[] EquipmentGrades { get; private set; }
        [field: SerializeField] public Material[] Materials { get; private set; }
        [field: SerializeField] public LuyenKhiRewardItem[] RewardItems { get; private set; }
        [field: SerializeField] public LianQiEffect[] LianQiEffects { get; private set; }
        [field: SerializeField] public EffectTypeConfig[] EffectTypeConfigs { get; private set; }
        [field: SerializeField] public StringDict Strings { get; private set; } = new();
        private static LuyenKhiResources instance;

        public static LuyenKhiResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<LuyenKhiResources>("LuyenKhi/LuyenKhiResources");
                }

                return instance;
            }
        }

        public IItemData FindItem(ItemRef itemRef)
        {
            var type = itemRef.Type;
            if (type == ItemType.Material)
            {
                return Materials.First(x => x.Id == itemRef.Id);
            }

            return null;
        }
        public Material FindMaterial(ushort Id)
        {
            return Materials.FirstOrDefault(x => x.Id == Id);
        }

        public void Deserialize(IReader reader, ref LuyenKhiResources value)
        {
            value.MaterialGradeConfigs = reader.ReadArray<MaterialGradeConfig>();
            value.EquipmentTiers = reader.ReadArray<EquipmentTierConfig>();
            value.EquipmentGrades = reader.ReadArray<EquipmentGradeConfig>();
            value.Materials = reader.ReadArray<Material>();
            value.RewardItems = reader.ReadArray<LuyenKhiRewardItem>();
            value.LianQiEffects = reader.ReadArray<LianQiEffect>();
            value.EffectTypeConfigs = reader.ReadArray<EffectTypeConfig>();
            value.Strings = reader.Read<StringDict>();
        }
        public int ByteSize()
        {
            return MaterialGradeConfigs.ByteSize() + EquipmentTiers.ByteSize()
                                                   + EquipmentGrades.ByteSize() + Materials.ByteSize()
                                                   + RewardItems.ByteSize()
                                                   + LianQiEffects.ByteSize()
                                                   + EffectTypeConfigs.ByteSize() + Strings.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(MaterialGradeConfigs);
            writer.WriteArray(EquipmentTiers);
            writer.WriteArray(EquipmentGrades);
            writer.WriteArray(Materials);
            writer.WriteArray(RewardItems);
            writer.WriteArray(LianQiEffects);
            writer.WriteArray(EffectTypeConfigs);
            writer.Write(Strings);
        }

        [Button]
        public void Export()
        {
            var writer = new Writer(ByteSize());
            writer.Write(this);
            var path = $"{Application.dataPath}/Exports/LuyenKhiResources.bin";
            File.WriteAllBytes(path, writer.Buffer);
            Debug.Log(path);
        }
#if UNITY_EDITOR
        [Button]
        public void LoadIconsFromFolder()
        {
            // 1. Mở cửa sổ chọn Folder trong máy
            string folderPath = EditorUtility.OpenFolderPanel("Chọn folder chứa ảnh FightFaBao", "Assets", "");

            if (string.IsNullOrEmpty(folderPath)) return;

            // 2. Chuyển đổi đường dẫn tuyệt đối sang đường dẫn Assets của Unity
            if (folderPath.StartsWith(Application.dataPath))
            {
                folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
            }
            else
            {
                Debug.LogError("Vui lòng chọn folder nằm bên trong thư mục Assets của Project!");
                return;
            }

            int count = 0;
            foreach (var item in RewardItems)
            {
                if (item.Icon != null)
                {
                    string spriteName = item.Icon.name;
                    // Tìm kiếm file có tên tương ứng trong folder đã chọn
                    string fullPath = Path.Combine(folderPath, spriteName + ".png");
                    // Có thể bổ sung kiểm tra .jpg, .tga tùy bạn

                    Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);

                    if (loadedSprite != null)
                    {
                        // item.IconFight = loadedSprite;
                        count++;
                    }
                }
            }

            // Lưu lại thay đổi vào Asset/Scene
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            Debug.Log($"Đã cập nhật thành công {count} Icon Fight từ folder: {folderPath}");
        }
#endif
    }
}