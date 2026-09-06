using System.Linq;
using System.Reflection;
using UnityEngine;

public static partial class EventList
{
    public static class Combat
    {
        public const string END_TURN_COMBAT = "Combat.END_TURN_COMBAT";
        public const string NEW_TURN_COMBAT = "Combat.NEW_TURN_COMBAT";
        public const string PLAY_SKILL = "Combat.PLAY_SKILL";
        public const string ADJUST_LINHKHI = "Combat.HAPTHU_LINHKHI";
        public const string ADD_BUFF = "Combat.ADD_BUFF";
        public const string CHARACTER_ADJUST_HP = "Combat.CHARACTER_ADJUST_HP";
        public const string CACULATE_DAME_BY_THANTHONG = "Combat.CACULATE_DAME_BY_THANTHONG";
        public const string CHARACTER_TAKE_DAME = "Combat.CHARACTER_TAKE_DAME";
        public const string CHARACTER_BEFORE_TAKE_DAME = "Combat.CHARACTER_BEFORE_TAKE_DAME";
        public const string BEFORE_EXCUTE_SKILL = "Combat.BEFORE_EXCUTE_SKILL";

        public const string LIEN_KICH = "Combat.LIEN_KICH";
        public const string END_COMBAT = "Combat.END_COMBAT";
        public const string TRUNG_DOC = "Combat.TRUNG_DOC";
        public const string NE_TRANH_SUCCESS = "Combat.NE_TRANH_SUCCESS";
        public const string CHECK_NE_TRANH = "Combat.CHECK_NE_TRANH";
        public const string CHARACTER_TAKE_DAME_BY_STATIC = "GamePlay.CHARACTER_TAKE_DAME_BY_STATIC";
        public const string KIEM_KHI = "Combat.KIEM_KHI";

        public const string ADD_SKILL = "Combat.ADD_SKILL";
        public const string UPDATE_AMKINH_THANTHONG = "Combat.UPDATE_AMKINH_THANTHONG";
        public const string CHARACTER_HEALING = "Combat.CHARACTER_HEALING";

        public const string SEND_SYNC_PLAYSKILL = "Combat.SEND_SYNC_PLAYSKILL"; // gửi id char  và id skill 
        public const string RECEIVE_SYNC_PLAYSKILL = "Combat.SEND_SYNC_PLAYSKILL"; // nhận id char  và id skill 

        public const string SEND_SYNC_ADJUST_LINHKHI = "Combat.SEND_SYNC_ADJUST_LINHKHI"; // gửi id char và linh khi hien tai 
        public const string RECEIVE_SYNC_ADJUST_LINHKHI = "Combat.RECEIVE_SYNC_ADJUST_LINHKHI"; //  nhận id char và linh khi hien tai 

        public const string SEND_SYNC_BUFF = "Combat.SEND_SYNC_BUFF"; // gửi id char và type buff/int id buff và new Count 
        public const string RECEIVE_SYNC_BUFF = "Combat.RECEIVE_SYNC_BUFF"; //  nhận id char và type buff/int id buff và new Count 

        public const string SEND_SYNC_DAME = "Combat.SEND_SYNC_DAME"; // gửi id char và dame cuối cùng đã đc trừ vào máu 
        public const string RECEIVE_SYNC_DAME = "Combat.RECEIVE_SYNC_DAME"; //  nhận id char và dame cuối cùng đã đc trừ vào máu  
        public static void RemoveAllCombatObservers()
        {
            // Lấy tất cả các trường const string trong class Combat
            var eventFields = typeof(Combat)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null))
                .ToList();

            // Gọi hàm xóa đã thêm ở class Observer (Cách 1)
            LongDH.LongDHModules.Observer.RemoveObserversForEvents(eventFields);

            Debug.Log("Đã xóa toàn bộ Observer của Combat");
        }
    }

    public static class GamePlay
    {
        public const string SHOW_RANK_INFO_POPUP = "GamePlay.SHOW_RANK_INFO_POPUP";
        public const string UPDATE_PLAYER_STATS = "GamePlay.UPDATE_PLAYER_STATS";
        public const string LOAD_SCENE = "GamePlay.LOAD_SCENE";
        public const string BACK_SCENE = "GamePlay.BACK_SCENE";
        public const string TIME_ADJUST = "GamePlay.TIME_ADJUST";
        public const string MOVE_MAP_SUCCESS = "GamePlay.MOVE_MAP_SUCCESS";
        public const string USER_ENTER_MAP = "GamePlay.ACTIVE_EVENT_MAP";
        public const string UPDATE_MAILS = "GamePlay.UPDATE_MAILS";
    }

    public static class Network
    {
        public const string IN_GAME_LOGOUT = "Network.IN_GAME_LOGOUT";
        public const string SIGN_IN_END = "Network.SIGN_IN_END";
        public const string BEGIN_SIGN_IN = "Network.BEGIN_SIGN_IN";
        public const string RESET_ARENA_RANK = "Network.RESET_ARENA_RANK";
        public const string GET_RANK = "Network.GET_RANK";
        public const string LOAD_RANK = "Network.LOAD_RANK";
        public const string CLAIM_RANK_REWARD = "Network.CLAIM_RANK_REWARD";
        public const string USE_DAN = "Network.USE_DAN";
        public const string LEARN_NGODAO = "Network.LEARN_NGODAO";
        public const string LEARN_SKILL = "Network.LEARN_SKILL";
        public const string EQUIP_THANTHONG = "Network.EQUIP_THANTHONG";
        public const string EQUIP_CONGPHAP = "Network.EQUIP_CONGPHAP";
        public const string UNEQUIP_CONGPHAP = "Network.UNEQUIP_CONGPHAP";
        public const string UNEQUIP_THANTHONG = "Network.UNEQUIP_THANTHONG";

        public const string TU_LUYEN = "Network.TU_LUYEN";
        public const string LINH_NGO = "Network.LINH_NGO";
        public const string DOT_PHA_CONGPHAP = "Network.DOT_PHA_CONGPHAP";
        public const string TANG_QUA_NPC = "Network.TANG_QUA_NPC";
        public const string THINH_GIAO_NPC = "Network.THINH_GIAO_NPC";
        public const string BUY_AND_SELL = "Network.BUY_AND_SELL";
        public const string LOAD_SHOP = "Network.LOAD_SHOP";
        public const string RELOAD_SHOP = "Network.RELOAD_SHOP";

        public const string LOAD_TASK = "Network.LOAD_TASK";
        public const string RELOAD_TASK = "Network.RELOAD_TASK";
        public const string DONE_TASK = "Network.DONE_TASK";

        public const string EQUIP_EQUIPMENT = "Network.EQUIP_EQUIPMENT";
        public const string COMPLETE_SIDEQUEST = "Network.COMPLETE_SIDEQUEST";
        public const string JOIN_MONPHAI = "Network.JOIN_MONPHAI";
        public const string COMPLETE_MAINQUEST = "Network.COMPLETE_MAINQUEST";

        public const string RESULT_COMBAT = "Network.RESULT_COMBAT";
        public const string LOAD_CHAR_COMBAT = "Network.LOAD_CHAR_COMBAT";
        public const string LOAD_CHAR_COMBAT_ARENA = "Network.LOAD_CHAR_COMBAT_ARENA";
        public const string GACHA = "Network.GACHA";
        public const string SPECIAL_LEVELUP = "Network.SPECIAL_LEVELUP";
        public const string LEVEL_UP = "Network.LEVEL_UP";
        public const string WATCH_ADS_COMBAT = "Network.WATCH_ADS_COMBAT";
        public const string WATCH_ADS_SIDE_QUEST = "Network.WATCH_ADS_SIDE_QUEST";
    }
}
