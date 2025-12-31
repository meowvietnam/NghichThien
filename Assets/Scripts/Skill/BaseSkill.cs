using UnityEngine;


public abstract class BaseSkill : BaseItemData
{
    
    public enum EKeyWord
    {
        None,
        LienKich,
        HauKeVoLuc,
        TuongSinh,
        TuongKhac,
    }
    public ETierSkill eTierSkill;
    public abstract void Execute(CharacterCombat caster, CharacterCombat target);
   
    public interface LienKich
    {
        public abstract void ExecuteLienKich(CharacterCombat caster, CharacterCombat target);


    }
    public interface HauKeVoLuc
    {
        public abstract void ExecuteHauKeVoLuc(CharacterCombat caster, CharacterCombat target);


    }
    public interface TuongSinh
    {
        public abstract void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target);


    }
    public interface TuongKhac
    {
        public abstract void ExecuteTuongKhac(CharacterCombat caster, CharacterCombat target);


    }
    public string GetKeyWord(EKeyWord eKeyWord)
    {
        string text = string.Empty;
        switch (eKeyWord)
        {
            case EKeyWord.None:
                break;
            case EKeyWord.LienKich:
                text = "Liên Kích";
                break;
            case EKeyWord.HauKeVoLuc:
                text = "Hậu Kế Vô Lực";

                break;
            case EKeyWord.TuongSinh:
                text = "Tương Sinh";

                break;
            case EKeyWord.TuongKhac:
                text = "Tương Khắc";

                break;
            default:
                break;
        }
        return GetColoredText($"[{text}]","green");

    }
    public string GetDescKeyWord(EKeyWord eKeyWord)
    {
        string text = string.Empty;
        switch (eKeyWord)
        {
            case EKeyWord.None:
                break;
            case EKeyWord.LienKich:
                text = "Nếu trước đó dùng X hệ kỹ năng thì sẽ kích hoạt";
                break;
            case EKeyWord.HauKeVoLuc:
                text = "Mỗi hiệp chỉ sử dụng một lần";

                break;
            case EKeyWord.TuongSinh:
                text = "Nếu dùng thần thông với linh khi tương sinh thì sẽ kích hoạt";

                break;
            case EKeyWord.TuongKhac:
                text = "Nếu dùng thần thông với linh khi tương khắc thì sẽ kích hoạt";

                break;
            default:
                break;
        }
        return $"{GetKeyWord(eKeyWord)}: " + text;

    }
    public string GetTextLinhKhi(ELinhKhi eLinhKhi)
    {
        string text = string.Empty;
        switch (eLinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                text = "Kim";
                break;
            case ELinhKhi.Moc:
                text = "Mộc";

                break;
            case ELinhKhi.Thuy:
                text = "Thủy";

                break;
            case ELinhKhi.Hoa:
                text = "Hỏa";

                break;
            case ELinhKhi.Tho:
                text = "Thổ";

                break;
            case ELinhKhi.Ma:
                text = "Ma";

                break;
            default:
                break;
        }
        return text;
    }
}
