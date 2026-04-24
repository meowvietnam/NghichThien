using Sirenix.OdinInspector;
using UnityEngine;




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
