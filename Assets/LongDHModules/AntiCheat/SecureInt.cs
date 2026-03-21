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
            // Nếu các biến enc/key chưa được khởi tạo (bằng 0), hãy lấy từ fakeValue
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
        int split = Random.Range(-1000, 1000);
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
        return s == null ? 0 : s.Value;
    }

    public static implicit operator SecureInt(int v)
    {
        return new SecureInt(v);
    }
}
