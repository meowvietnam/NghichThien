using CreateCharacter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EBuffAndDeBuff
{
    None,
    HoTrao,
    DeTonThuong,
    KiemKhi,
    KietSuc,
    TreKhi,
    SucThe,
    GiamTonThuong,
    NoiThuong,
    TrienNhieu,
    TriLieu,
    ChanKim,
    TrungDoc,
    HuyenVu,
    ChayMau,
    ChanMoc,
    ChanThuy,
    ChanHoa,
    ChanTho,
    TuKhi,
    Cham,
    SuongGia,
    DongKet,
    ChanKiem,
    ChiThuy,
    KinhDao,


}

public abstract class BaseSkill : BaseItemData
{
    public static List<EBuffAndDeBuff> debuffs = new List<EBuffAndDeBuff>
    {
        EBuffAndDeBuff.DeTonThuong,
        EBuffAndDeBuff.KietSuc,
        EBuffAndDeBuff.TreKhi,
        EBuffAndDeBuff.NoiThuong,
        EBuffAndDeBuff.TrienNhieu,
        EBuffAndDeBuff.TrungDoc,
        EBuffAndDeBuff.ChayMau,
        EBuffAndDeBuff.SuongGia,
        EBuffAndDeBuff.DongKet,


    };
    public static List<EBuffAndDeBuff> buffs = Enum.GetValues(typeof(EBuffAndDeBuff))
    .Cast<EBuffAndDeBuff>()
    .Where(e => e != EBuffAndDeBuff.None)
    .Where(e => !BaseSkill.debuffs.Contains(e))
    .ToList();
    public enum ETypeSkill
    {
        None,
        Kim,
        Moc,
        Thuy,
        Hoa,
        Tho,
        Ma,
        Kiem,
        Khi,
        The,
        Than,

    }
    public enum EKeyWord
    {
        None,
        LienKich,
        HauKeVoLuc,
        TuongSinh,
        TuongKhac,
        TuongDong,
        KiemY,
        HutMau
    }
    public ETierSkill eTierSkill;
    public ETypeSkill[] eTypeSkill;

    public EKeyWord[] eKeywordSkillHave;
    public EBuffAndDeBuff[] eKeywordBuffSkillHave;

   

  
    public abstract void Execute(CharacterCombat caster, CharacterCombat target, params object[] data);
   
    public interface LienKich
    {
        public abstract void ExecuteLienKich(CharacterCombat caster, CharacterCombat target);


    }
    public interface TuongSinh
    {
        public abstract void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target);


    }
    public interface TuongKhac
    {
        public abstract void ExecuteTuongKhac(CharacterCombat caster, CharacterCombat target);


    }
    public interface TuongDong
    {
        public abstract void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target);

    }
    public abstract string GetDesc(int level);

    
    public static string GetKeyWordBuffOrDeBuff(EBuffAndDeBuff eKeyWord)
    {
        string text = string.Empty;
        switch (eKeyWord)
        {
            case EBuffAndDeBuff.None:
                break;
            case EBuffAndDeBuff.HoTrao:
                text = "Hộ Trảo";
                break;
            case EBuffAndDeBuff.DeTonThuong:
                text = "Dễ Tổn Thương";

                break;
            case EBuffAndDeBuff.KiemKhi:
                text = "Kiếm Khí";
                break;
            case EBuffAndDeBuff.KietSuc:
                text = "Kiệt Sức";
                break;
            case EBuffAndDeBuff.TreKhi:
                text = "Trệ Khí";
                break;
            case EBuffAndDeBuff.SucThe:
                text = "Sục Thể";
                break;
            case EBuffAndDeBuff.GiamTonThuong:
                text = "Giảm Tổn Thương";
                break;
            case EBuffAndDeBuff.NoiThuong:
                text = "Nội Thương";

                break;
            case EBuffAndDeBuff.TrienNhieu:
                text = "Triền Nhiễu";

                break;
            case EBuffAndDeBuff.TriLieu:
                text = "Trị Liệu";
                break;

            case EBuffAndDeBuff.ChanKim:
                text = "Chấn Kim";
                break;
            case EBuffAndDeBuff.TrungDoc:
                text = "Trúng Độc";
                break;
            case EBuffAndDeBuff.HuyenVu:
                text = "Huyễn Vụ";
                break;
            case EBuffAndDeBuff.ChayMau:
                text = "Chảy Máu";
                break;
            case EBuffAndDeBuff.ChanMoc:
                text = "Chấn Mộc";
                break;
            case EBuffAndDeBuff.ChanThuy:
                text = "Chấn Thủy";
                break;
            case EBuffAndDeBuff.ChanHoa:
                text = "Chấn Hỏa";
                break;
            case EBuffAndDeBuff.ChanTho:
                text = "Chấn Thổ";
                break;
            case EBuffAndDeBuff.TuKhi:
                text = "Tụ Khí";
                break;
            case EBuffAndDeBuff.Cham:
                text = "Châm";
                break;
            case EBuffAndDeBuff.SuongGia:
                text = "Sương Giá";
                break;
            case EBuffAndDeBuff.DongKet:
                text = "Đông Kết";
                break;
            case EBuffAndDeBuff.ChanKiem:
                text = "Chấn Kiếm";
                break;
            case EBuffAndDeBuff.ChiThuy:
                text = "Chỉ Thủy";
                break;
            case EBuffAndDeBuff.KinhDao:
                text = "Kinh Đào";
                break;
            default:
                break;
        }
        return GetColoredText($"[{text}]", "green");

    }
    public static string GetDescBuffOrDeBuff(EBuffAndDeBuff eKeyWord)
    {
        string text = string.Empty;
        switch (eKeyWord)
        {
            case EBuffAndDeBuff.None:
                break;
            case EBuffAndDeBuff.HoTrao:
                text = "Mỗi tầng ngăn cản 1 điểm sát thương";
                break;
            case EBuffAndDeBuff.DeTonThuong:
                text = "Trong hiệp này, mỗi tầng sát thương phải nhận thêm +1. Kết thúc hiệp giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.KiemKhi:
                text = $"Kết thúc hiệp, mỗi tầng gây 1 điểm sát thương kiếm hệ cho đối phương";
                break;
            case EBuffAndDeBuff.KietSuc:
                text = "Bản thân hiệp sau không được thi pháp";
                break;
            case EBuffAndDeBuff.TreKhi:
                text = "Giai đoạn hấp thụ linh khí, mỗi tầng -1 linh khí có thể hấp thụ sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.SucThe:
                text = "Mỗi nhận 1 điểm sát thương thì giải trừ 1 tầng. Bắt đầu hiệp, mỗi tầng gây 1 điểm sát thương kiếm hệ cho đối phương sau đó giải trừ toàn bộ";

                break;
            case EBuffAndDeBuff.GiamTonThuong:
                text = "Trong hiệp này, mỗi tầng ngăn cản 1 điểm sát thương. Kết thúc hiệp giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.NoiThuong:
                text = $"Giai đoạn hấp thụ linh khí, nhận sát thương {GetColoredText($"mỗi tầng * số linh khí hấp thụ","red")} sau đó giải trừ 1 tầng";
                break;
            case EBuffAndDeBuff.TrienNhieu:
                text = "Trong hiệp này, khi bản thân thi pháp nhận mỗi tầng 1 điểm sát thương. Kết thúc hiệp giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.TriLieu:
                text = $"Kết thúc hiệp, mỗi tầng khôi phục 1 điểm sinh mệnh sau đó giải trừ 1 tầng";
                break;
            case EBuffAndDeBuff.ChanKim:
                text = $"Kim hệ thần thông mỗi tầng sát thương tăng thêm 1 sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.TrungDoc:
                text = $"Kết thúc hiệp, mỗi tầng nhận 1 điểm sát thương sau đó giải trừ 1 tầng";
                break;
            case EBuffAndDeBuff.HuyenVu:
                text = $"Có 5% tỉ lệ né tránh sát thương. Tỉ lệ này không cộng dồn";
                break;
            case EBuffAndDeBuff.ChayMau:
                text = $"Kết thúc hiệp, bản thân nhận 1% tổn thương của lượng HP đã tổn thất sau đó giải trừ 1 tầng";
                break;
            case EBuffAndDeBuff.ChanMoc:
                text = $"Mộc hệ thần thông mỗi tầng sát thương tăng thêm 1 sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.ChanThuy:
                text = $"Thủy hệ thần thông mỗi tầng sát thương tăng thêm 1 sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.ChanHoa:
                text = $"Hỏa hệ thần thông mỗi tầng sát thương tăng thêm 1 sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.ChanTho:
                text = $"Thổ hệ thần thông mỗi tầng sát thương tăng thêm 1 sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.TuKhi:
                text = $"Giai đoạn hấp thụ linh khí, mỗi tầng hấp thụ nhiều thêm 1 điểm linh khí sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.Cham:
                text = $"Chỉ thị vật";
                break;
            case EBuffAndDeBuff.SuongGia:
                text = $"Mỗi khi sử dụng thần thông gây tổn thương, mỗi tầng giảm -1 điểm sát thương có thể gây ra. Sau đó giải trừ 1 tầng";
                break;
            case EBuffAndDeBuff.DongKet:
                text = "Bản thân hiệp sau không được thi pháp";
                break;
            case EBuffAndDeBuff.ChanKiem:
                text = $"Kiếm hệ thần thông mỗi tầng sát thương tăng thêm 1 sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.ChiThuy:
                text = $"Chỉ thị vật";
                break;
            case EBuffAndDeBuff.KinhDao:
                text = $"Chỉ thị vật";
                break;
            default:
                break;
        }
        return $"{GetKeyWordBuffOrDeBuff(eKeyWord)}: " + text;

    }
    public static string GetKeyWord(EKeyWord eKeyWord)
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
            case EKeyWord.TuongDong:
                text = "Đồng";

                break;
            case EKeyWord.KiemY:
                text = "Kiếm Ý";

                break;
            case EKeyWord.HutMau:
                text = "Hút Máu";

                break;
            default:
                break;
        }
        return GetColoredText($"[{text}]","green");

    }
    public static string GetDescKeyWord(EKeyWord eKeyWord)
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
            case EKeyWord.TuongDong:
                text = "Nếu dùng thần thông với toàn bộ linh khí là cùng hệ thì sẽ kích hoạt";

                break;
            case EKeyWord.KiemY:
                text = $"Sử dụng thần thông này thu hoạch {GetColoredText($"[Kiếm Khí] +1", "green")}";
                break;
            case EKeyWord.HutMau:
                text = "Hồi máu bằng lượng sát thương mà thần thông gây ra";
                break;
            default:
                break;
        }
        return $"{GetKeyWord(eKeyWord)}: " + text;

    }
    public static string GetTextLinhKhi(ELinhKhi eLinhKhi)
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
    public string GetTextTypeSkill()
    {
        string text = string.Empty;
        for (int i = 0; i < eTypeSkill.Length; i++)
        {
            var item  = eTypeSkill[i];
            text += GetTextTypeSkill(item);
            if(i < eTypeSkill.Length-1)
            {
                text += " - ";
            }    

        }
       
        return GetColoredText($"{text}","red");
    }    
    public static string GetTextTypeSkill(ETypeSkill typeSkill)
    {
        string text = string.Empty;
        switch (typeSkill)
        {
            case ETypeSkill.None:
                break;
            case ETypeSkill.Kim:
                text = "Kim";
                break;
            case ETypeSkill.Moc:
                text = "Mộc";
                break;
            case ETypeSkill.Thuy:
                text = "Thủy";
                break;
            case ETypeSkill.Hoa:
                text = "Hỏa";

                break;
            case ETypeSkill.Tho:
                text = "Thổ";
                break;
            case ETypeSkill.Ma:
                text = "Ma";
                break;
            case ETypeSkill.Kiem:
                text = "Kiếm";

                break;
            case ETypeSkill.Khi:
                text = "Khí";

                break;
            case ETypeSkill.The:
                text = "Thể";

                break;
            case ETypeSkill.Than:
                text = "Thần";

                break;
            default:
                break;
        }
        return text;
    }
    public bool CheckIsEqualETypeSkill(CharacterCombat caster)
    {
        
        return CheckIsEqualETypeSkill(caster,this.eTypeSkill);   
    }
    public static bool CheckIsEqualETypeSkill(CharacterCombat caster , ETypeSkill[] skill)
    {
        if (caster.thanThongCurrentUse == null) return false;
        // Kiểm tra hệ của Skill đang dùng có khớp với Công Pháp (SO) này không
        bool isExecute = skill.Any(skill => caster.thanThongCurrentUse.thanThong.eTypeSkill.Contains(skill));
        Debug.Log("Check Equal " + isExecute);
        return isExecute;
    }
   
    public void KiemY(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, 1);
    }    
  
}
