using System;
using UnityEngine;
[System.Serializable]
public class SecureFloat
{
    [SerializeField] int encA;
    [SerializeField] int encB;

    [SerializeField] int keyA;
    [SerializeField] int keyB;

    [SerializeField] float fakeValue;

    public SecureFloat(float value)
    {
        Set(value);
    }

    public float Value
    {
        get
        {
            if (encA == 0 && keyA == 0)
            {
                Set(fakeValue);
                return fakeValue;
            }
            float partA = IntToFloat(encA ^ keyA);
            float partB = IntToFloat(encB ^ keyB);

            float real = partA + partB;

            if (Mathf.Abs(real - fakeValue) > 0.001f)
            {
                AntiCheatManager.CheatDetected("Float memory cheat detected");
            }

            Set(real);
            return real;
        }
        set
        {
            Set(value);
        }
    }

    void Set(float value)
    {
        // Khởi tạo bộ ngẫu nhiên của hệ thống
        System.Random rand = new System.Random();

        // Tạo split ngẫu nhiên từ -1000f đến 1000f
        // Công thức: min + (rand.NextDouble() * (max - min))
        float split = (float)(-1000.0 + (rand.NextDouble() * 2000.0));

        float partA = split;
        float partB = value - split;

        // Tạo Key ngẫu nhiên (Số nguyên)
        keyA = rand.Next(10000, 99999);
        keyB = rand.Next(10000, 99999);

        // Mã hóa
        encA = FloatToInt(partA) ^ keyA;
        encB = FloatToInt(partB) ^ keyB;

        fakeValue = value;
    }

    int FloatToInt(float v)
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(v), 0);
    }

    float IntToFloat(int v)
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(v), 0);
    }

    public static implicit operator float(SecureFloat s)
    {
        return s == null ? 0f : s.Value;
    }

    public static implicit operator SecureFloat(float v)
    {
        return new SecureFloat(v);
    }
}
