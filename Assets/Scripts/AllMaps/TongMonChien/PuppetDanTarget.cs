using AllMaps.ItemProvider;
using LuyenDan;
using LuyenDan.Inventory;

namespace AllMaps.TongMonChien
{
    /// <summary>
    /// Đích dùng đan là một Rối của tông (chỉ Tộc Chủ). Túi đồ dùng giao diện sẵn có; lệnh gửi WarUseHpPill.
    /// Rối chỉ nhận đan hồi khí huyết (Hp / HoiFullHp).
    /// </summary>
    public sealed class PuppetDanTarget : IDanUseTarget
    {
        private readonly TongMonChienController controller;

        public PuppetDanTarget(TongMonChienController controller, int puppetId)
        {
            this.controller = controller;
            PuppetId = puppetId;
        }

        public int PuppetId { get; }

        public string Title => $"Rối #{PuppetId}";

        public bool CanUse(LinhDan dan, out string reason)
        {
            if (dan.EBuffType == EDanBuffType.Hp || dan.EBuffType == EDanBuffType.HoiFullHp)
            {
                reason = null;
                return true;
            }

            reason = "Rối chỉ dùng được đan hồi khí huyết.";
            return false;
        }

        public void Use(LinhDan dan, ushort count)
        {
            controller.SendPuppetHpPill(PuppetId, (ushort)dan.Id, count);
        }
    }
}
