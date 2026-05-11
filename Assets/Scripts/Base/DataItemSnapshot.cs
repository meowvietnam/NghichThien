using CreateCharacter;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Base;
using Network;
using System.Linq;
public class DataItemSnapshot 
{
    public ItemRef ItemRef { get; set; }

    public IItemData IItemData { get; set; }
    public Equipment EquipmentData => ItemProviderFactory.GetEquipmentProvider(ItemRef.Type)?.GetEquipmentById(ItemRef.Id);
    public FurnaceData furnaceData;
    public FurnaceData FurnaceData
    {
        get
        {
            // Nếu _furnaceData null thì tạo mới một cái rồi trả về
            return furnaceData ??= ItemRef.Type == ItemType.Furnace ? Client.Instance.CharacterData.InventoryManager.Furnaces.FirstOrDefault(x => x.SelfId == ItemRef.Id) : null;
        }
    }
    public BaseItemData BaseItemData { get; set; }
}
