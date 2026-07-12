using UnityEngine;

[System.Serializable]
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
            if (encA == 0 && keyA == 0)
            {
                Set(fakeValue);
                return fakeValue;
            }
            int partA = encA ^ keyA;
            int partB = encB ^ keyB;

            int real = partA + partB;

            if (real != fakeValue)
            {
                AntiCheatManager.CheatDetected("Int memory cheat detected");
            }

            Set(real); // re-encrypt

            return real;
        }
        set
        {
            Set(value);
        }
    }

    void Set(int value)
    {
        System.Random rand = new System.Random();

        int split = rand.Next(-1000, 1000);
        int partA = split;
        int partB = value - split;

        keyA = rand.Next(10000, 99999);
        keyB = rand.Next(10000, 99999);

        encA = partA ^ keyA;
        encB = partB ^ keyB;

        fakeValue = value;
    }

    public void RenewKeys()
    {
        Set(fakeValue);
    }

    public static implicit operator int(SecureInt s)
    {
        return s == null ? 0 : s.Value;
    }

    public static implicit operator SecureInt(int v)
    {
        return new SecureInt(v);
    }

    // Automatically renew keys in the Inspector
    private void OnValidate()
    {
        RenewKeys();
    }
}