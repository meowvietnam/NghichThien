using UnityEngine;

public enum ETierSkill {

    None,
    NhanGiaiHa,
    NhanGiaiTrung,
    NhanGiaiThuong,
    DiaGiaiHa,
    DiaGiaiTrung,
    DiaGiaiThuong,
    ThienGiaiHa,
    ThienGiaiTrung,
    ThienGiaiThuong,

}
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
public abstract class BaseItemData : ScriptableObject
{
    [SerializeField] protected string nameItem;
    protected abstract ETypeItem typeItem { get; }
    [SerializeField]
    public Sprite spriteItem;
    [SerializeField]
    public Sprite frameItem => null;

    public abstract string GetDesc();
    public static string GetColoredText(string text, string colorName)
    {
        return $"<color={colorName}>{text}</color>";
    }

}
