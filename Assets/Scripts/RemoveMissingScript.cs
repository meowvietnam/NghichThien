using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class RemoveMissingScript : MonoBehaviour
{
    [Button]
    public void RemoveScript()
    {
        var transforms = GetComponentsInChildren<Transform>(true);
        foreach (var tf in transforms)
        {
            var go = tf.gameObject;
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            Debug.Log($"Remove missing script in {go.name}");
        }
    }
}
