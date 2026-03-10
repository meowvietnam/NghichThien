using UnityEngine;

public class AntiCheatManager 
{
    public static void CheatDetected(string reason)
    {
        Debug.Log("Cheat detected: " + reason);

        // tuỳ game bạn muốn làm gì
        Application.Quit();
    }
}
