using UnityEngine;
using UnityEngine.UI;

public class ConvertAnimToUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Image img;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        img.sprite = renderer.sprite;
    }
}
