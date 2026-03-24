using Sirenix.OdinInspector;
using UnityEngine;


public enum ETirePhapBao
{

    None,
    HaPhamPhuKhi,
    TrungPhamPhuKhi,
    ThuongPhamPhuKhi,
    HaPhamPhapKhi,
    TrungPhamPhapKhi,
    ThuongPhamPhapKhi,
    HaPhamPhapBao,
    TrungPhamPhapBao,
    ThuongPhamPhapBao,

}
public enum ETypeItem
{
    None,
    ThanThong,
    CongPhap,
    DanDuoc,
    DuocLieu,
    VatLieu
}
public abstract class BaseItemData : BaseData
{

    protected abstract ETypeItem typeItem { get; }
    
    [SerializeField]
    public Sprite frameItem => null;

    public abstract string GetDesc();
    public static string GetColoredText(string text, string colorName)
    {
        return $"<color={colorName}>{text}</color>";
    }

   

}
