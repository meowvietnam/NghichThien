using UnityEngine;

public static class ValueConstants
{
    public  static  SecureInt[] tuviRequires = new SecureInt[]
    {
        10000,
        20000,
        30000,

    };
    public static SecureInt GetTuviRequired(int level)
    {
        int index = level - 1;
        if (index < 0) return tuviRequires[0];
        if (index >= tuviRequires.Length) return tuviRequires[tuviRequires.Length - 1]; // Trả về mốc cuối nếu max level

        return tuviRequires[index];
    }
}
