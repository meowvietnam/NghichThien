using System.Collections.Generic;
using AllMaps.EventReward;
using CreateCharacter;
using LuyenDan;
using Network.Models;

public class INetworkEvent 
{
    public interface IUseDan
    {
        void SuccessUseDan();
        void FailUseDan(string message);
    }
    public interface ILearnNgoDao // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessLearnNgoDao(NgoDaoBuff buff, NgoDao ngoDao, int newNgoDaoDiem);
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
    public interface IDotPhaCongPhap // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessDotPhaCongPhap();
        void FailDotPhaCongPhap(string message);
    }
    public interface ITangQuaNPC // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessTangQuaNpc();
        void FailTangQuaNpc(string message);
    }
    public interface IThinhGiaoNPC // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessThinhGiaoNpc();
        void FailThinhGiaoNpc(string message);
    }
    public interface ILoadShop// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessLoadShop(List<ItemRef> itemRefs);
        void FailLoadShop(string message);
    }
    public interface IReLoadShop// Check đủ ngộ đạo điểm mới learn đượcaaaa
    {
        void SuccessReLoadShop(List<ItemRef> itemRefs);
        void FailReLoadShop(string message);
    }
    public interface IBuyAndSell // Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessBuyAndSell();
        void FailBuyAndSell(string message);
    }
    public interface ILoadTask// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessLoadTask(List<TaskDailySnapshot> listTask);
        void FailLoadTask(string message);
    }
    public interface IReLoadTask// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessReLoadTask(List<TaskDailySnapshot> listTask);
        void FailReLoadTask(string message);
    }
    public interface IDoneTask// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessDoneTask();
        void FailDoneTask(string message);
    }
    public interface IEquipEquipment// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessEquipEquipment();
        void FailEquipEquipment(string message);
    }
   
    public interface ICompleteSideQuest// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessCompleteSideQuest(int moneyVip);
        void FailCompleteSideQuest(string message);
    }
    public interface IJoinMonPhai// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessJoinMonPhai();
        void FailJoinMonPhai(string message);
    }
    public interface ICompleteMainQuest// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessCompleteMainQuest();
        void FailMainQuest(string message);
    }
    public interface IWinCombatBase// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessWinCombatBase(RewardSnapshot rewardSnapshot);
        void FailWinCombatBase(string message);
    }
    public interface ILoadCharCombat// Check đủ ngộ đạo điểm mới learn được
    {
        void SuccessLoadCharCombat(CharacterSnapshot user);
        void FailLoadCharCombat(string message);
    }
}
