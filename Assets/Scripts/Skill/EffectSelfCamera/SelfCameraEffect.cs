using System.Collections;
using UnityEngine;

public class SelfCameraEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DelayFrame());
    }
    private IEnumerator DelayFrame()
    {
        yield return null;
        this.transform.position = Camera.main.transform.position;

    }
}
