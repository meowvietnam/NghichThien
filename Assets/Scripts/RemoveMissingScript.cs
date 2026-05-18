using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class RemoveMissingScript : MonoBehaviour
{
    [Button]
    public void RemoveScript()
    {
        Remove(gameObject);
    }

    public static void Remove(GameObject gameObject)
    {
        var transforms = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (var tf in transforms)
        {
            var go = tf.gameObject;
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            Debug.Log($"Remove missing script in {go.name}");
        }
    }
}
