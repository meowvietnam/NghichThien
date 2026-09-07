using UnityEngine;

public class GGDetector : MonoBehaviour
{
    void Start()
    {
        if (IsGameGuardianInstalled())
        {
            AntiCheatManager.CheatDetected("GameGuardian detected");
        }
    }

    bool IsGameGuardianInstalled()
    {
        string[] ggApps =
        {
            "com.gameguardian",
            "catch_.me_.if_.you_.can_"
        };

        foreach (string app in ggApps)
        {
            if (IsAppInstalled(app))
                return true;
        }

        return false;
    }

    bool IsAppInstalled(string bundle)
    {
        try
        {
            using (AndroidJavaClass unityPlayer =
                   new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                AndroidJavaObject pm =
                    activity.Call<AndroidJavaObject>("getPackageManager");

                pm.Call<AndroidJavaObject>("getPackageInfo", bundle, 0);

                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}