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

        public const string ADD_BASESKILL = "Combat.ADD_BASESKILL";
        public const string UPDATE_AMKINH_THANTHONG= "Combat.UPDATE_AMKINH_THANTHONG";
        public const string CHARACTER_HEALING = "Combat.CHARACTER_HEALING";

    }
    public static class GamePlay
    {  
        public const string UPDATE_PLAYER_STATS = "GamePlay.UPDATE_PLAYER_STATS";
        public const string LOAD_SCENE = "GamePlay.LOAD_SCENE";
        public const string BACK_SCENE = "GamePlay.BACK_SCENE";

    }
    public static class Network
    {
        public const string USE_DAN = "GamePlay.USE_DAN";
        public const string LEARN_NGODAO = "GamePlay.LEARN_NGODAO";
        public const string LEARN_SKILL = "GamePlay.LEARN_SKILL";
        public const string EQUIP_THANTHONG = "GamePlay.EQUIP_THANTHONG";
        public const string EQUIP_CONGPHAP = "GamePlay.EQUIP_CONGPHAP";
        public const string UNEQUIP_CONGPHAP = "GamePlay.UNEQUIP_CONGPHAP";
        public const string UNEQUIP_THANTHONG = "GamePlay.UNEQUIP_THANTHONG";

        public const string TU_LUYEN = "GamePlay.TU_LUYEN";
        public const string LINH_NGO = "GamePlay.LINH_NGO";
        public const string DOT_PHA_CONGPHAP = "GamePlay.DOT_PHA_CONGPHAP";

    }
}
