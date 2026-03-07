using UnityEngine;

public class SecureInt
{
    [SerializeField] int encA;
    [SerializeField] int encB;

    [SerializeField] int keyA;
    [SerializeField] int keyB;

    [SerializeField] int fakeValue;

    public SecureInt(int value)
    {
        Set(value);
    }

    public int Value
    {
        get
        {
            int partA = encA ^ keyA;
            int partB = encB ^ keyB;

            int real = partA + partB;

            if (real != fakeValue)
            {
                AntiCheatManager.CheatDetected("Int memory cheat detected");
            }

            return real;
        }
        set
        {
            Set(value);
        }
    }

    void Set(int value)
    {
        int split = UnityEngine.Random.Range(0, value + 1);

        int partA = split;
        int partB = value - split;

        keyA = UnityEngine.Random.Range(10000, 99999);
        keyB = UnityEngine.Random.Range(10000, 99999);

        encA = partA ^ keyA;
        encB = partB ^ keyB;

        fakeValue = value;
    }

    public static implicit operator int(SecureInt s)
    {
        return s.Value;
    }

    public static implicit operator SecureInt(int v)
    {
        return new SecureInt(v);
    }
}
