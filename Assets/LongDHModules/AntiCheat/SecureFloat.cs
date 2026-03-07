using System;
using UnityEngine;

public class SecureFloat : MonoBehaviour
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
            float partA = IntToFloat(encA ^ keyA);
            float partB = IntToFloat(encB ^ keyB);

            float real = partA + partB;

            if (Mathf.Abs(real - fakeValue) > 0.001f)
            {
                AntiCheatManager.CheatDetected("Float memory cheat detected");
            }

            return real;
        }
        set
        {
            Set(value);
        }
    }

    void Set(float value)
    {
        float split = UnityEngine.Random.Range(0f, value);

        float partA = split;
        float partB = value - split;

        keyA = UnityEngine.Random.Range(10000, 99999);
        keyB = UnityEngine.Random.Range(10000, 99999);

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
        return s.Value;
    }

    public static implicit operator SecureFloat(float v)
    {
        return new SecureFloat(v);
    }
}
