using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviour
{
    public BaseItemData itemData;
    [SerializeField] private Image img;
    // Thay đổi Action để nhận vào RectTransform (đối tượng được thả vào)
    private Action<RectTransform> actionPointerUp;
    private List<RectTransform> overlays;
    [SerializeField]private RectTransform rectTransform;
    [SerializeField]private Canvas canvas;
    private bool isDragging = false; // Biến cờ kiểm soát
   
    [Button]
    private void GetComponent()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        // Tắt Raycast Target của chính cái ảnh kéo để nó không chặn ô bên dưới
        if (img != null) img.raycastTarget = false;
    }    
    private void Update()
    {
        if (!isDragging) return;

        // 1. Cập nhật vị trí theo chuột/touch
        Vector3 currentPos = Input.mousePosition;
        if (Input.touchCount > 0) currentPos = Input.GetTouch(0).position;

        rectTransform.position = currentPos;

        // 2. Kiểm tra lệnh THẢ
        // Dùng GetMouseButton(0) == false nghĩa là chuột KHÔNG CÒN ĐÈ nữa
        if (!Input.GetMouseButton(0) && (Input.touchCount == 0 || Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            HandleRelease(currentPos);
        }
    }

    private void HandleRelease(Vector2 releasePosition)
    {
        isDragging = false;
        RectTransform hitOverlay = null;

        if (overlays != null)
        {
            foreach (var rect in overlays)
            {
                if(canvas == null)
                {
                    canvas = GetComponentInParent<Canvas>();

                }
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, releasePosition, canvas.worldCamera))
                {
                    hitOverlay = rect;
                    break;
                }
            }
        }

        actionPointerUp?.Invoke(hitOverlay);
        if (actionPointerUp== null)
        {
            Debug.Log("action up null");
        }    
        StartCoroutine(DelayFrameHide());
    }
    public void Show(BaseItemData data, List<RectTransform> targetOverlay, Action<RectTransform> actionPointerUpOverlay)
    {
        this.itemData = data;
        this.img.sprite = data.spriteItem;
        this.img.SetNativeSize();
        this.overlays = targetOverlay;
        this.actionPointerUp = actionPointerUpOverlay;
        gameObject.SetActive(true);
        this.img.enabled = false;
        StartCoroutine(DelayFrame());
        isDragging = true; // Kích hoạt trạng thái kéo ngay lập tức
    }
    private IEnumerator DelayFrame()
    {
        yield return null;
        this.img.enabled = true;
    }
    private IEnumerator DelayFrameHide()
    {
        yield return null;
        Hide();
    }
    private void Hide() => gameObject.SetActive(false);
}
