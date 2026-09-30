using LuyenDan.Inventory;

namespace AllMaps.ItemProvider
{
    /// <summary>
    /// Đích dùng đan thay cho bản thân (ví dụ: Rối trong Tông môn chiến). Khi có đích đang đăng ký,
    /// túi đồ vẫn dùng nguyên giao diện chọn đan/số lượng nhưng gửi lệnh cho đích này thay vì USE_DAN.
    /// </summary>
    public interface IDanUseTarget
    {
        /// <summary>Tên hiển thị trên hộp chọn số lượng, ví dụ "Rối #3".</summary>
        string Title { get; }

        bool CanUse(LinhDan dan, out string reason);

        void Use(LinhDan dan, ushort count);
    }

    /// <summary>Giữ đích dùng đan hiện tại; null = dùng cho bản thân theo luồng cũ.</summary>
    public static class DanUseRouter
    {
        public static IDanUseTarget Current { get; private set; }

        public static void Set(IDanUseTarget target)
        {
            Current = target;
        }

        /// <summary>Chỉ gỡ khi đích hiện tại đúng là <paramref name="target"/> (tránh gỡ nhầm đích khác).</summary>
        public static void Clear(IDanUseTarget target)
        {
            if (ReferenceEquals(Current, target)) Current = null;
        }
    }
}
