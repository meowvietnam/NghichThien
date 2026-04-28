using Network.Models;

namespace Network.Callback
{
    public interface ILuyenKhiCallback
    {
        void OnLuyenKhiResult(LuyenKhiResult result);
        void OnSetEquipmentNameResult(SetEquipmentNameResult result);
    }
}