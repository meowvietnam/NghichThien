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
    [SerializeField] private List<RectTransform> overlays;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private bool isDragging = false; // Biến cờ kiểm soát

    /// <summary>Đang trong thao tác kéo vật phẩm. Hand-tut dùng để đổi khung sáng sang ô đích.</summary>
    public bool IsDragging => isDragging;

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
                if (canvas == null)
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

        StartCoroutine(DelayFrameHide());
        actionPointerUp?.Invoke(hitOverlay);
        if (actionPointerUp == null)
        {
            Debug.Log("action up null");
        }
    }

    public void Show(ItemInventory itemInventory, List<RectTransform> targetOverlay,
        Action<RectTransform> actionPointerUpOverlay)
    {
        if (itemInventory.itemData == null) return;
        this.itemData = itemInventory.itemData;
        this.img.sprite = itemInventory.icon.sprite;
        this.img.SetNativeSize();
        this.overlays = targetOverlay;
        this.actionPointerUp = actionPointerUpOverlay;
        gameObject.SetActive(true);
        this.img.enabled = false;
        StartCoroutine(DelayFrame());
        isDragging = true; // Kích hoạt trạng thái kéo ngay lập tức
        SetAnimHighlightTarget(true);
    }

    private void SetAnimHighlightTarget(bool active)
    {
        foreach (var item in overlays)
        {
            if (item == null) continue;

            // Thêm tham số 'true' ở đây là chìa khóa!
            HighlightAnim anim = item.GetComponentInChildren<HighlightAnim>(true);

            if (anim != null)
            {
                anim.gameObject.SetActive(active);

                // Nếu bạn muốn dùng cái shader "con rắn" hồi nãy để bật bằng code:
                // anim.EnableBorder(true); 
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy HighlightAnim trong {item.name}");
            }
        }
    }

    private IEnumerator DelayFrame()
    {
        yield return null;
        this.img.enabled = true;
    }

    private IEnumerator DelayFrameHide()
    {
        SetAnimHighlightTarget(false);
        yield return null;

        Hide();
    }

    private void Hide() => gameObject.SetActive(false);
}