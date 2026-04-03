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

public class BaseItemData : BaseData
{

    
    [SerializeField]
    public Sprite frameItem => null;

    public virtual string GetDesc()
    {
        return "";
    }    
    public static string GetColoredText(string text, string colorName)
    {
        return $"<color={colorName}>{text}</color>";
    }

   

}
