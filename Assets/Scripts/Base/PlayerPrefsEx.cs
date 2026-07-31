using System;
using UnityEngine;

namespace Base
{
    public class PlayerPrefsEx
    {
        public static void SetBytes(string key, byte[] value)
        {
            PlayerPrefs.SetString(key, Convert.ToBase64String(value));
        }
        public static byte[] GetBytes(string key)
        {
            string value = PlayerPrefs.GetString(key, "");
            return string.IsNullOrEmpty(value)
                ? Array.Empty<byte>()
                : Convert.FromBase64String(value);
        }
    }
}