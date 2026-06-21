using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Network;
using LongDH.LongDHModules;

public class FirebaseService : SingletonMonoDontDestroy<AdsService>
{
    private FirebaseApp app;
    private void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                Observer.AddObserver(EventList.Network.COMPLETE_SIDEQUEST, LogCompleteSideQuest);
                Debug.Log($"connect firebase analytics thành công");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }    
    public void LogCompleteSideQuest()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("complete_side_quest",
            new Firebase.Analytics.Parameter("complete_with_count",Client.Instance.CharacterData.QuestManager.CurrentCountSideQuest+1));
    }    
}
