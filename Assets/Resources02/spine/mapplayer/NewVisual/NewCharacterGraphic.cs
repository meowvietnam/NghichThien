using Base;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using CreateCharacter;
using Spine.Unity.AttachmentTools; // THÊM NAMESPACE NÀY Ở ĐẦU FILE
public enum EVehicleType
{
    Base,
    Sword,
}
public class NewCharacterGraphic : MonoBehaviour
{
    [SerializeField] public SkeletonAnimation graphic;
    [SerializeField] protected SkinResources skinResources;
    private Dictionary<string, string> currentSkins = new Dictionary<string, string>();
    private Dictionary<string, Color> currentColors = new Dictionary<string, Color>();
    public Vector3 refLocalPos; // lấy male làm chuẩn
    public float addLocalY = 30f; // lấy male làm chuẩn
    public GameObject[] visualVehicle;
    private void OnEnable()
    {
        if (refLocalPos == Vector3.zero)
        {
            refLocalPos = this.transform.localPosition;
        }
    }
    private void Awake()
    {
        refLocalPos = this.transform.localPosition;
    }
    private void OnDestroy()
    {
        // Dọn dẹp material/texture instance tự tạo khi object bị xoá
        if (graphic != null && graphic.Skeleton != null)
        {
            AtlasUtilities.ClearCache();
        }
    }
    public void InitVisual(EVehicleType vehicleType,Gender gender, Dictionary<string, string> combines, Dictionary<string, Color> colors = null)
    {
        foreach (var item in visualVehicle)
        {
            item.gameObject.SetActive(false);
        }
        visualVehicle[(int)vehicleType].gameObject.SetActive(true);
        SetGender(gender);
        SetSkin(combines,colors);
    }    
    public void SetGender(Gender gender)
    {
        var skeletonDataAsset = skinResources.GenderSkinGroups.FirstOrDefault(skinResource => skinResource.Gender == gender)?.SkeletonDataAsset;
        graphic.skeletonDataAsset = skeletonDataAsset;
        currentSkins.Clear();
        currentColors.Clear();
        graphic.Initialize(true);
        graphic.AnimationState.SetAnimation(0, "Idle_0", true);
        if (refLocalPos == Vector3.zero)
        {
            refLocalPos = this.transform.localPosition;
        }
        // lấy localpos male làm chuẩn
        if (gender == Gender.Female)
        {
            this.transform.localPosition = refLocalPos + new Vector3(0, addLocalY, 0);
        }
        else
        {
            this.transform.localPosition = refLocalPos;
        }
    }
    public void SetSkin(Dictionary<string, string> combines, Dictionary<string, Color> colors = null)
    {
        var combineSkin = new Skin("character_combined");
        var skeleton = graphic.Skeleton;
        var data = skeleton.Data;

        if (combines == null) return;

        foreach (var (skinId, skinName) in combines)
        {
            var skin = data.FindSkin(skinName);
            if (skin == null) continue;
            currentSkins[skinId] = skinName;
            combineSkin.AddSkin(skin);
        }

        // 1. Gán Skin đã kết hợp vào Skeleton
        skeleton.SetSkin(combineSkin);
        skeleton.SetSlotsToSetupPose();

        // 2. TẠO MATERIAL INSTANCE SEPARATE (Khắc phục lỗi dính Material sang Object khác)
        if (graphic.SkeletonDataAsset != null)
        {
            foreach (var atlasAsset in graphic.SkeletonDataAsset.atlasAssets)
            {
                if (atlasAsset == null) continue;
                foreach (var originalMat in atlasAsset.Materials)
                {
                    if (originalMat == null) continue;

                    // Nếu chưa có Custom Override cho Material này thì tạo bản sao
                    if (!graphic.CustomMaterialOverride.ContainsKey(originalMat))
                    {
                        Material instanceMat = new Material(originalMat);
                        instanceMat.name = originalMat.name + " (Instance)";
                        graphic.CustomMaterialOverride[originalMat] = instanceMat;
                    }
                }
            }
        }

        // 3. Đổi màu cho Slot
        if (colors != null)
        {
            foreach (var (skinId, color) in colors)
            {
                if (!currentSkins.TryGetValue(skinId, out var skinName)) continue;
                var skin = data.FindSkin(skinName);
                if (skin == null) continue;

                foreach (var attachment in skin.Attachments)
                {
                    var slot = skeleton.Slots.Items[attachment.SlotIndex];
                    slot.SetColor(color);
                }
                currentColors[skinId] = color;
            }
        }

        graphic.Update(0);
    }
    [Button]
    public (Dictionary<string, string> skins, Dictionary<string, Color> colors) RandomSkin(Gender gender, bool useDefaultColors)
    {
        var skins = new Dictionary<string, string>();
        var colors = new Dictionary<string, Color>();
        var palette = skinResources.Colors;
        var group = skinResources.GenderSkinGroups.First(x => x.Gender == gender);
        foreach (var groupSkinGroup in group.SkinGroups)
        {
            foreach (var skinCategory in groupSkinGroup.Categories)
            {
                var skin = skinCategory.Skins[Random.Range(0, skinCategory.Skins.Count)];
                skins[skinCategory.Id] = skin.Name;
            }

            foreach (var colorGroup in groupSkinGroup.ColorGroups)
            {
                foreach (var colorGroupCategoryId in colorGroup.CategoryIds)
                {
                    var color = useDefaultColors ? colorGroup.DefaultColor : palette[Random.Range(0, palette.Count)];
                    colors[colorGroupCategoryId] = color;
                }
            }
        }
        SetGender(gender);
        SetSkin(skins, colors);
        return (skins, colors);
    }
    public static (Dictionary<string, string> skins, Dictionary<string, Color> colors) RandomSkinStatic(Gender gender, bool useDefaultColors)
    {
        var skins = new Dictionary<string, string>();
        var colors = new Dictionary<string, Color>();
        var palette = SkinResources.Instance.Colors;
        var group = SkinResources.Instance.GenderSkinGroups.First(x => x.Gender == gender);
        foreach (var groupSkinGroup in group.SkinGroups)
        {
            foreach (var skinCategory in groupSkinGroup.Categories)
            {
                var skin = skinCategory.Skins[Random.Range(0, skinCategory.Skins.Count)];
                skins[skinCategory.Id] = skin.Name;
            }

            foreach (var colorGroup in groupSkinGroup.ColorGroups)
            {
                foreach (var colorGroupCategoryId in colorGroup.CategoryIds)
                {
                    var color = useDefaultColors ? colorGroup.DefaultColor : palette[Random.Range(0, palette.Count)];
                    colors[colorGroupCategoryId] = color;
                }
            }
        }

        return (skins, colors);
    }

}
