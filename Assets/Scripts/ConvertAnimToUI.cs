using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConvertAnimToUI : MonoBehaviour
{
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Image img;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(DelayFrame());
    }

    private IEnumerator DelayFrame()
    {
        img.enabled = false;
        yield return null;
        img.enabled = true;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        img.sprite = renderer.sprite;
        img.color = renderer.color;
        img.SetNativeSize();
    }
}