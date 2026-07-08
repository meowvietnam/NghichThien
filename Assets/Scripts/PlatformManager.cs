using UnityEngine;

public class PlatformManager
{
    public static Platform GetPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                return Platform.Mobile;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.LinuxPlayer:
                return Platform.Desktop;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
                return Platform.Desktop;
            default:
                Debug.LogWarning($"Unsupported platform {Application.platform}, fallback to desktop platform.");
                return Platform.Desktop;
        }
    }
}
