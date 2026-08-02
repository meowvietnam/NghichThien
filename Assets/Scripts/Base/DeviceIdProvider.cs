using System;
using UnityEngine;

namespace Base
{
    public static class DeviceIdProvider
    {
        public static string GetDeviceId()
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;

            if (string.IsNullOrWhiteSpace(deviceId) ||
                deviceId == SystemInfo.unsupportedIdentifier)
            {
                const string key = "local_device_id";

                deviceId = PlayerPrefs.GetString(key, string.Empty);

                if (string.IsNullOrWhiteSpace(deviceId))
                {
                    deviceId = Guid.NewGuid().ToString("N");
                    PlayerPrefs.SetString(key, deviceId);
                    PlayerPrefs.Save();
                }
            }

            return deviceId;
        }
    }
}