using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonStartGame : BaseBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image img1;
    [SerializeField] private Image img2;
    [SerializeField] private Sprite spriteStartGame1;
    [SerializeField] private Sprite spriteDisableGame1;
    [SerializeField] private Sprite spriteStartGame2;
    [SerializeField] private Sprite spriteDisableGame2;
    public void OnPointerDown(PointerEventData eventData)
    {
        img1.sprite = spriteStartGame1;
        img2.sprite = spriteStartGame2;
        img1.SetNativeSize();
        img2.SetNativeSize();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        img1.sprite = spriteDisableGame1;
        img2.sprite = spriteDisableGame2;
        img1.SetNativeSize();
        img2.SetNativeSize();
    }
}
