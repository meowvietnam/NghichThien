

public class INetworkEvent 
{
    public interface IUseDan
    {
        void SuccessUseDan();
        void FailUseDan(string message);
    }
    public interface ILearnNgoDao // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessLearnNgoDao();
        void FailLearnNgoDao(string message);
    }
    public interface ILearnSkill // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessLearnSkill();
        void FailLearnSkill(string message);
    }
    public interface IEquipThanThong // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessEquipThanThong();
        void FailEquipThanThong(string message);
    }
    public interface IUnEquipThanThong // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessUnEquipThanThong();
        void FailUnEquipThanThong(string message);
    }
    public interface IEquipCongPhap // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessEquipCongPhap();
        void FailEquipCongPhap(string message);
    }
    public interface IUnEquipCongPhap // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessUnEquipCongPhap();
        void FailUnEquipCongPhap(string message);
    }
    public interface IUpdateCongPhap // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessUpdateCongPhap();
        void FailUpdateCongPhap(string message);
    }
    public interface ITuLuyen // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessTuLuyen();
        void FailTuLuyen(string message);
    }
    public interface ILinhNgo // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessLinhNgo();
        void FailLinhNgo(string message);
    }
}
