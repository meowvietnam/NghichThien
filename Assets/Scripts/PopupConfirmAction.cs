using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;
using LongDH.LongDHModules;
using LongDHModules;

public class PopupConfirmAction : Popup, IUISingleton
{
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Text contentText;

    /// <summary>Nút Xác Nhận - public để hand-tut khoanh sáng được.</summary>
    public Button BtnConfirm => btnConfirm;

    /// <summary>
    /// Nội dung đang hiển thị. Hand-tut dùng nó làm mốc nhận biết người chơi đã xác nhận xong:
    /// popup này là singleton nên khi một hành động bật LIÊN TIẾP 2 popup (VD tu luyện lúc tâm
    /// cảnh thấp), popup thứ 2 hiện ra ngay trong cùng frame popup thứ 1 ẩn đi - nhìn vào
    /// activeInHierarchy sẽ không thấy nó từng tắt, nhưng nội dung thì đổi.
    /// </summary>
    public string ContentText => contentText != null ? contentText.text : string.Empty;

    [Button]
    public void ShowPopConfirmAction(string content, Action actionConfirm)
    {
        Show();
        contentText.supportRichText = true;
        contentText.text = content;
        SetEventButton(btnConfirm, () => { OnClickEvent(actionConfirm); });
        var fitter = contentText.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            for (int i = 0; i < 3; i++)
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                fitter.enabled = false;
                fitter.enabled = true;
            }
        }
    }

    protected virtual void OnClickEvent(Action actionConfirm)
    {
        Hide();
        actionConfirm?.Invoke();
    }

    public void Init()
    {
        Pool.Set(this);
        gameObject.SetActive(false);
    }
}
