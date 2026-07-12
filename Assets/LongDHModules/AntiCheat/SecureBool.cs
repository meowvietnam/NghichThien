using System;
using UnityEngine;

[System.Serializable]
public class SecureBool
{
    [SerializeField] int encValue;
    [SerializeField] int key;
    [SerializeField] bool fakeValue;

    public SecureBool(bool value)
    {
        Set(value);
    }

    public bool Value
    {
        get
        {
            bool realValue = (encValue ^ key) == 1;

            if (realValue != fakeValue)
            {
                Debug.LogError("Bool memory cheat detected!");
            }

            Set(realValue);
            return realValue;
        }
        set
        {
            Set(value);
        }
    }

    private void Set(bool value)
    {
        System.Random rand = new System.Random();

        key = rand.Next(10000, 999999);

        int intVal = value ? 1 : 0;
        encValue = intVal ^ key;

        fakeValue = value;
    }

    public void RenewKeys()
    {
        Set(fakeValue);
    }

    public static implicit operator bool(SecureBool s)
    {
        return s == null ? false : s.Value;
    }

    public static implicit operator SecureBool(bool v)
    {
        return new SecureBool(v);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    private void OnValidate()
    {
        RenewKeys();
    }
}