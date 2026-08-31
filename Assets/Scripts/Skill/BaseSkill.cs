using CreateCharacter;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Network;
using Network.Extensions;
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
    SucLuc,
    ThieuDot,
    HoaDiem,
    Diem,
    Tran,
    DanLuc,
    PhaThe,
    Cat,
    AnNac,
    Hon,
    Anh,
    AnhDich,
    AnhNac,
    ChanLongChiHuyet,
    NgungThan,
    ThanThuong,
    QuyCuu,
    QuyTon,
    HonLuc,
    NguKiemHiep,
    NhanKiem,
    DiaKiem,
    ThienKiem,
    QuyLinh,
    QuyBinh,
    QuyTuong,
    QuyDe,
    KiemTam,
    ChanThan,
    NhieuHon,
    TinhHonChiLuc,
    NguKhiHiep,
    NguKimHiep,
    NguMocHiep,
    NguThuyHiep,
    NguHoaHiep,
    NguThoHiep,
    NguThanThucHiep,
    ChanKhi,
    TangToc,
    TroiChan,
    TichHoa,
    TichKim,
    TichMoc,
    TichThuy,
    TichDoc,
    NguKim,
    NguMoc,
    NguThuy,
    NguHoa,
    NguTho,
    PhanTonThuong,
}

public class BaseSkill : BaseItemData, IMemorySerializable<BaseSkill>
{
    [SerializeField] public int baseTimeLearn;

    [Button]
    public virtual void SetBaseTimeLearn()
    {
    }

    public enum ETierSkill
    {
        None,
        NhanGiaiHa,
        NhanGiaiTrung,
        NhanGiaiThuong,
        DiaGiaiHa,
        DiaGiaiTrung,
        DiaGiaiThuong,
        ThienGiaiHa,
        ThienGiaiTrung,
        ThienGiaiThuong,
    }

    public static List<EBuffAndDeBuff> eDebuffs = new List<EBuffAndDeBuff>
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
        EBuffAndDeBuff.ThieuDot,
        EBuffAndDeBuff.DanLuc,
        EBuffAndDeBuff.PhaThe,
        EBuffAndDeBuff.ThanThuong,
        EBuffAndDeBuff.NhieuHon
    };

    public static List<EBuffAndDeBuff> eNoneBuff = new List<EBuffAndDeBuff>
    {
        EBuffAndDeBuff.QuyCuu,
        EBuffAndDeBuff.QuyLinh,
        EBuffAndDeBuff.QuyBinh,
        EBuffAndDeBuff.QuyTuong,
        EBuffAndDeBuff.QuyTon,
        EBuffAndDeBuff.QuyDe,
        EBuffAndDeBuff.Cham,
        EBuffAndDeBuff.ChiThuy,
        EBuffAndDeBuff.KinhDao,
        EBuffAndDeBuff.Tran,
        EBuffAndDeBuff.Cat,
        EBuffAndDeBuff.Hon,
        EBuffAndDeBuff.Anh,
    };

    public static List<EBuffAndDeBuff> eBuffs = Enum.GetValues(typeof(EBuffAndDeBuff))
        .Cast<EBuffAndDeBuff>()
        .Where(e => e != EBuffAndDeBuff.None)
        .Where(e => !eDebuffs.Contains(e) && !eNoneBuff.Contains(e))
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
        DiHoa,
        TranPhap
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
        AmKinh,
        TranPhap,
        ThanThuc,
        ChanThucTonThuong
    }

    public ETierSkill eTierSkill;
    public ETypeSkill[] eTypeSkill;

    [SerializeField] private EKeyWord[] eKeywordSkillHave;
    [SerializeField] private EBuffAndDeBuff[] eKeywordBuffSkillHave;

    public virtual EKeyWord[] GetEKeywordSkillHave()
    {
        return eKeywordSkillHave;
    }

    public virtual EBuffAndDeBuff[] GetEKeywordBuffSkillHave(int level)
    {
        return eKeywordBuffSkillHave;
    }


    public virtual void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

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

    public interface ThanThuc
    {
        public abstract void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target);
    }

    public virtual string GetDesc(int daiCanhGioi)
    {
        return string.Empty;
    }


    public static string GetKeyWordBuffOrDeBuff(EBuffAndDeBuff eKeyWord, int level = 1)
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
            case EBuffAndDeBuff.SucLuc:
                text = "Sức Lực";
                break;
            case EBuffAndDeBuff.ThieuDot:
                text = "Thiêu Đốt";
                break;
            case EBuffAndDeBuff.HoaDiem:
                text = "Hỏa Diễm";
                break;
            case EBuffAndDeBuff.Diem:
                text = "Diễm";
                break;
            case EBuffAndDeBuff.Tran:
                text = "Trần";
                break;
            case EBuffAndDeBuff.DanLuc:
                text = "Dẫn Lực";
                break;
            case EBuffAndDeBuff.PhaThe:
                text = "Phá Thể";
                break;
            case EBuffAndDeBuff.Cat:
                text = "Cát";
                break;
            case EBuffAndDeBuff.AnNac:
                text = "Ẩn Nặc";
                break;
            case EBuffAndDeBuff.Hon:
                text = "Hồn";
                break;
            case EBuffAndDeBuff.Anh:
                text = "Ảnh";
                break;
            case EBuffAndDeBuff.AnhDich:
                text = "Ảnh Dịch";
                break;
            case EBuffAndDeBuff.AnhNac:
                text = "Ảnh Nặc";
                break;
            case EBuffAndDeBuff.ChanLongChiHuyet:
                text = "Chân Long Chi Huyết";
                break;
            case EBuffAndDeBuff.NgungThan:
                text = "Ngưng Thần";
                break;
            case EBuffAndDeBuff.ThanThuong:
                text = "Thần Thương";
                break;
            case EBuffAndDeBuff.QuyCuu:
                text = "Quỷ Cửu";
                break;
            case EBuffAndDeBuff.QuyTon:
                text = "Quỷ Tôn";
                break;
            case EBuffAndDeBuff.HonLuc:
                text = "Hồn Lực";
                break;
            case EBuffAndDeBuff.NguKiemHiep:
                text = "Ngự Kiếm (Công)";
                break;
            case EBuffAndDeBuff.NhanKiem:
                text = "Nhân Kiếm";
                break;
            case EBuffAndDeBuff.DiaKiem:
                text = "Địa Kiếm";
                break;
            case EBuffAndDeBuff.ThienKiem:
                text = "Thiên Kiếm";
                break;
            case EBuffAndDeBuff.QuyLinh:
                text = "Quỷ Linh";
                break;
            case EBuffAndDeBuff.QuyBinh:
                text = "Quỷ Binh";
                break;
            case EBuffAndDeBuff.QuyTuong:
                text = "Quỷ Tướng";
                break;
            case EBuffAndDeBuff.QuyDe:
                text = "Quỷ Đế";
                break;
            case EBuffAndDeBuff.KiemTam:
                text = "Kiếm Tâm";
                break;
            case EBuffAndDeBuff.ChanThan:
                text = "Chấn Thần";
                break;
            case EBuffAndDeBuff.NhieuHon:
                text = "Nhiễu Hồn";
                break;
            case EBuffAndDeBuff.TinhHonChiLuc:
                text = "Tinh Hồn Chi Lực";
                break;
            case EBuffAndDeBuff.ChanKhi:
                text = "Chấn Khí";
                break;
            case EBuffAndDeBuff.NguKhiHiep:
                text = "Ngự Khí (Công)";

                break;
            case EBuffAndDeBuff.NguKimHiep:
                text = "Ngự Kim (Công)";

                break;
            case EBuffAndDeBuff.NguMocHiep:
                text = "Ngự Mộc (Công)";

                break;
            case EBuffAndDeBuff.NguThuyHiep:
                text = "Ngự Thủy (Công)";

                break;
            case EBuffAndDeBuff.NguHoaHiep:
                text = "Ngự Hỏa (Công)";

                break;
            case EBuffAndDeBuff.NguThoHiep:
                text = "Ngự Thổ (Công)";

                break;
            case EBuffAndDeBuff.NguThanThucHiep:
                text = "Ngự Thần Thức (Công)";

                break;
            case EBuffAndDeBuff.TangToc:
                text = "Tăng Tốc";

                break;
            case EBuffAndDeBuff.TroiChan:
                text = "Trói Chân";

                break;
            case EBuffAndDeBuff.TichHoa:
                text = "Tích Hỏa";

                break;
            case EBuffAndDeBuff.TichKim:
                text = "Tích Kim";

                break;
            case EBuffAndDeBuff.TichMoc:
                text = "Tích Mộc";

                break;
            case EBuffAndDeBuff.TichThuy:
                text = "Tích Thủy";

                break;
            case EBuffAndDeBuff.TichDoc:
                text = "Tích Độc";

                break;
            case EBuffAndDeBuff.NguKim:
                text = "Ngự Kim (Thủ)";

                break;
            case EBuffAndDeBuff.NguMoc:
                text = "Ngự Mộc (Thủ)";

                break;
            case EBuffAndDeBuff.NguThuy:
                text = "Ngự Thủy (Thủ)";

                break;
            case EBuffAndDeBuff.NguHoa:
                text = "Ngự Hỏa (Thủ)";

                break;
            case EBuffAndDeBuff.NguTho:
                text = "Ngự Thổ (Thủ)";

                break;
            case EBuffAndDeBuff.PhanTonThuong:
                text = "Phản Tổn Thương";

                break;
            default:
                break;
        }

        return GetColoredText($"[{text}]", "#ffb143");
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
                text =
                    "Mỗi nhận 1 điểm sát thương thì giải trừ 1 tầng. Bắt đầu hiệp, mỗi tầng gây 1 điểm sát thương kiếm hệ cho đối phương sau đó giải trừ toàn bộ";

                break;
            case EBuffAndDeBuff.GiamTonThuong:
                text = "Trong hiệp này, mỗi tầng ngăn cản 1 điểm sát thương. Kết thúc hiệp giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.NoiThuong:
                text =
                    $"Giai đoạn hấp thụ linh khí, nhận sát thương {GetColoredText($"mỗi tầng * số linh khí hấp thụ", "red")} sau đó giải trừ 1 tầng";
                break;
            case EBuffAndDeBuff.TrienNhieu:
                text =
                    "Trong hiệp này, khi bản thân thi pháp nhận mỗi tầng 1 điểm sát thương. Kết thúc hiệp giải trừ toàn bộ";
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
                text =
                    $"Giai đoạn hấp thụ linh khí, mỗi tầng hấp thụ nhiều thêm 1 điểm linh khí sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.Cham:
                text = $"Chỉ thị vật";
                break;
            case EBuffAndDeBuff.SuongGia:
                text =
                    $"Mỗi khi sử dụng thần thông gây tổn thương, mỗi tầng giảm -1 điểm sát thương có thể gây ra. Sau đó giải trừ 1 tầng";
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
            case EBuffAndDeBuff.SucLuc:
                text = "Trong hiệp này, thần thông gây tổn thương +1. Kết thúc hiệp giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.ThieuDot:
                text = $"Bắt đầu hiệp, mỗi tầng nhận 1 điểm sát thương sau đó giải trừ 1 tầng";
                break;
            case EBuffAndDeBuff.HoaDiem:
                text =
                    $"Mỗi khi nhận sát thương bởi thần thông, phản lại sát thương cho đối thủ mỗi tầng gây thêm 1 điểm sát thương. Kết thúc hiệp, bản thân nhận 1 tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Sử dụng bất kỳ loại thần thông nào không phải {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông thì sẽ giải trừ";
                break;
            case EBuffAndDeBuff.Diem:
                text =
                    $"Trong hiệp này, {GetTextLinhKhi(ELinhKhi.Hoa)} hệ tổn thương +1. Kết thúc hiệp giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.Tran:
                text = $"Chỉ thị vật";
                break;
            case EBuffAndDeBuff.DanLuc:
                text = $"Mỗi khi sử dụng thần thông, mỗi tầng làm tiêu tán 1 điểm linh khí";
                break;
            case EBuffAndDeBuff.PhaThe:
                text = $"Giảm một nửa khả năng hồi phục. Kết thúc hiệp giải trừ";
                break;
            case EBuffAndDeBuff.Cat:
                text = $"Chỉ thị vật";
                break;
            case EBuffAndDeBuff.AnNac:
                text =
                    $"Có 50% tỉ lệ né tránh thần thông sát thương. Sử dụng bất kì thần thông nào cùng đều giải trừ trạng thái này";
                break;
            case EBuffAndDeBuff.Hon:
                text = $"Chỉ thị vật";
                break;
            case EBuffAndDeBuff.Anh:
                text = $"Chỉ thị vật";
                break;
            case EBuffAndDeBuff.AnhDich:
                text = $"Kết thúc hiệp, mỗi tầng sẽ tấn công đối thủ hấp thụ 1 điểm HP";
                break;
            case EBuffAndDeBuff.AnhNac:
                text = $"Có 9% tỉ lệ né tránh thần thông sát thương. Hiệp mới sau đó sẽ giải trừ trạng thái này";
                break;
            case EBuffAndDeBuff.ChanLongChiHuyet:
                text = $"Bắt đầu hiệp, Gỡ ra 10 tầng trạng thái tiêu cực của bản thân";
                break;
            case EBuffAndDeBuff.NgungThan:
                text = $"Mỗi tầng thần thức +1";
                break;
            case EBuffAndDeBuff.ThanThuong:
                text = $"Mỗi tầng thần thức -1";
                break;
            case EBuffAndDeBuff.QuyCuu:
                text = $"Chỉ thị vật";
                break;
            case EBuffAndDeBuff.QuyTon:
                text =
                    $"Bắt đầu hiệp, mỗi {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}, đối thủ nhận ngẫu nhiêu 4 lần trạng thái tiêu cực. Đồng thời, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HonLuc)} khôi phục lại 1500 + 40% Hp tối đa của người sở hữu";
                break;
            case EBuffAndDeBuff.HonLuc:
                text = $"Mỗi tổn thương 1 điểm sẽ giải trừ 1 tầng, tiêu hao hết Quỷ Hồn tiêu tan";
                break;
            case EBuffAndDeBuff.NguKiemHiep:
                text = $"Trong hiệp này, mỗi tầng tăng lên một điểm sát thương đối với thần thông hệ kiếm";
                break;
            case EBuffAndDeBuff.NhanKiem:
                text =
                    $"Kết thúc hiệp, mỗi một tầng thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
                break;
            case EBuffAndDeBuff.DiaKiem:
                text =
                    $"Kết thúc hiệp, mỗi một tầng thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
                break;
            case EBuffAndDeBuff.ThienKiem:
                text =
                    $"Kết thúc hiệp, mỗi một tầng phát động {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} một lần";
                break;
            case EBuffAndDeBuff.QuyLinh:
                text =
                    $"Bắt đầu hiệp, mỗi {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}, đối thủ nhận ngẫu nhiêu 1 lần trạng thái tiêu cực. Đồng thời, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HonLuc)} khôi phục lại 150 + 10% Hp tối đa của người sở hữu";
                break;
            case EBuffAndDeBuff.QuyBinh:
                text =
                    $"Bắt đầu hiệp, mỗi {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}, đối thủ nhận ngẫu nhiêu 2 lần trạng thái tiêu cực. Đồng thời, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HonLuc)} khôi phục lại 300 + 20% Hp tối đa của người sở hữu";
                break;
            case EBuffAndDeBuff.QuyTuong:
                text =
                    $"Bắt đầu hiệp, mỗi {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}, đối thủ nhận ngẫu nhiêu 3 lần trạng thái tiêu cực. Đồng thời, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HonLuc)} khôi phục lại 500 + 30% Hp tối đa của người sở hữu";
                break;
            case EBuffAndDeBuff.QuyDe:
                text =
                    $"Bắt đầu hiệp, mỗi {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}, đối thủ nhận ngẫu nhiêu 5 lần trạng thái tiêu cực. Đồng thời, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HonLuc)} khôi phục lại 5000 + 50% Hp tối đa của người sở hữu";
                break;
            case EBuffAndDeBuff.KiemTam:
                text =
                    $"Mỗi khi sử dụng {GetTextTypeSkill(ETypeSkill.Kiem)} hệ thần thông thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguKiemHiep)}, sau đó giải trừ một tầng. Kết thúc hiệp giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.ChanThan:
                text = $"Thần thức thần thông mỗi tầng sát thương tăng thêm 1 sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.NhieuHon:
                text = $"Giai đoạn hấp thụ linh khí, số lượng linh khí hấp thụ giảm một nửa";
                break;
            case EBuffAndDeBuff.TinhHonChiLuc:
                text = $"Mỗi tầng thần thức +1. Kết thúc hiệp giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.ChanKhi:
                text = $"Khí hệ thần thông mỗi tầng sát thương tăng thêm 1 sau đó giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.NguKhiHiep:
                text = $"Trong hiệp này, mỗi tầng tăng lên một điểm sát thương đối với thần thông hệ Khí";
                break;
            case EBuffAndDeBuff.NguKimHiep:
                text = $"Trong hiệp này, mỗi tầng tăng lên một điểm sát thương đối với thần thông hệ Kim";
                break;
            case EBuffAndDeBuff.NguMocHiep:
                text = $"Trong hiệp này, mỗi tầng tăng lên một điểm sát thương đối với thần thông hệ Mộc";
                break;
            case EBuffAndDeBuff.NguThuyHiep:
                text = $"Trong hiệp này, mỗi tầng tăng lên một điểm sát thương đối với thần thông hệ Thủy";
                break;
            case EBuffAndDeBuff.NguHoaHiep:
                text = $"Trong hiệp này, mỗi tầng tăng lên một điểm sát thương đối với thần thông hệ Hỏa";
                break;
            case EBuffAndDeBuff.NguThoHiep:
                text = $"Trong hiệp này, mỗi tầng tăng lên một điểm sát thương đối với thần thông hệ Thổ";
                break;
            case EBuffAndDeBuff.NguThanThucHiep:
                text = $"Trong hiệp này, mỗi tầng tăng lên một điểm sát thương đối với thần thông hệ Thần Thức";
                break;
            case EBuffAndDeBuff.TangToc:
                text = $"Mỗi tầng độn tốc +1";
                break;
            case EBuffAndDeBuff.TroiChan:
                text = $"Mỗi tầng độn tốc -1";
                break;
            case EBuffAndDeBuff.NguKim:
                text = $"Trong hiệp này, mỗi tầng giảm một điểm sát thương đối với thần thông hệ Kim";
                break;
            case EBuffAndDeBuff.NguMoc:
                text = $"Trong hiệp này, mỗi tầng giảm một điểm sát thương đối với thần thông hệ Mộc";
                break;
            case EBuffAndDeBuff.NguThuy:
                text = $"Trong hiệp này, mỗi tầng giảm một điểm sát thương đối với thần thông hệ Thủy";
                break;
            case EBuffAndDeBuff.NguHoa:
                text = $"Trong hiệp này, mỗi tầng giảm một điểm sát thương đối với thần thông hệ Hỏa";
                break;
            case EBuffAndDeBuff.NguTho:
                text = $"Trong hiệp này, mỗi tầng giảm một điểm sát thương đối với thần thông hệ Thổ";
                break;
            case EBuffAndDeBuff.PhanTonThuong:
                text =
                    $"Mỗi lần nhận tổn thương, mỗi tầng phản lại một điểm sát thương cho đối thủ. Kết thúc hiệp, giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.TichHoa:
                text =
                    $"Kết thúc hiệp, mỗi tầng giải trừ một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Sau đó, giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.TichKim:
                text =
                    $"Mỗi lần nhận tổn thương, mỗi tầng giải trừ một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}. Kết thúc hiệp, giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.TichMoc:
                text =
                    $"Mỗi lần sử dụng thần thông, mỗi tầng giải trừ một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}. Kết thúc hiệp, giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.TichThuy:
                text =
                    $"Mỗi lần sử dụng thần thông, mỗi tầng giải trừ một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}. Kết thúc hiệp, giải trừ toàn bộ";
                break;
            case EBuffAndDeBuff.TichDoc:
                text =
                    $"Bắt đầu hiệp, mỗi tầng giải trừ một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}. Kết thúc hiệp, giải trừ toàn bộ";
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
            case EKeyWord.AmKinh:
                text = "Ám Kình";

                break;
            case EKeyWord.TranPhap:
                text = "Trận Pháp";

                break;
            case EKeyWord.ThanThuc:
                text = "Thần Thức";

                break;
            case EKeyWord.ChanThucTonThuong:
                text = "Chân Thực Tổn Thương";

                break;
            default:
                break;
        }

        return GetColoredText($"[{text}]", "#ffb143");
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
            case EKeyWord.AmKinh:
                text = "Người đang sở hữu Ám Kình sẽ nhận hiệu ứng của thần thông sau X hiệp tiếp theo";
                break;
            case EKeyWord.TranPhap:
                text = "Mỗi cuộc chiến chỉ có thể sử dụng một lần";
                break;
            case EKeyWord.ThanThuc:
                text = "Nếu thần thức lớn hơn đối thủ thì sẽ kích hoạt";
                break;
            case EKeyWord.ChanThucTonThuong:
                text = "Gây tổn thương trực tiếp. Tức là bỏ qua né tránh và các loại giảm trừ sát thương";
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
            var item = eTypeSkill[i];
            text += GetTextTypeSkill(item);
            if (i < eTypeSkill.Length - 1)
            {
                text += " - ";
            }
        }

        return GetColoredText($"{text}", "#ffb143");
    }

    public static string GetTextTierSkill(ETierSkill eTierSkill)
    {
        string text = string.Empty;
        switch (eTierSkill)
        {
            case ETierSkill.None:
                break;
            case ETierSkill.NhanGiaiHa:
                text = "Nhân Giai Hạ";
                break;
            case ETierSkill.NhanGiaiTrung:
                text = "Nhân Giai Trung";
                break;
            case ETierSkill.NhanGiaiThuong:
                text = "Nhân Giai Thượng";
                break;
            case ETierSkill.DiaGiaiHa:
                text = "Địa Giai Hạ";
                break;
            case ETierSkill.DiaGiaiTrung:
                text = "Địa Giai Trung";
                break;
            case ETierSkill.DiaGiaiThuong:
                text = "Địa Giai Thượng";
                break;
            case ETierSkill.ThienGiaiHa:
                text = "Thiên Giai Hạ";
                break;
            case ETierSkill.ThienGiaiTrung:
                text = "Thiên Giai Trung";
                break;
            case ETierSkill.ThienGiaiThuong:
                text = "Thiên Giai Thượng";
                break;
        }

        return text;
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
                text = "Thần Thức";

                break;
            case ETypeSkill.DiHoa:
                text = "Dị Hỏa";

                break;
            case ETypeSkill.TranPhap:
                text = "Trận Pháp";
                break;
        }

        return text;
    }

    public static ETypeSkill MonPhaiToSkill(EMonPhai monPhai)
    {
        switch (monPhai)
        {
            case EMonPhai.KimHongKiemPhai:
                return ETypeSkill.Kim;
            case EMonPhai.TrucSonTong:
                return ETypeSkill.Moc;
            case EMonPhai.TinhHaKiemPhai:
                return ETypeSkill.Thuy;
            case EMonPhai.LyHoaMon:
                return ETypeSkill.Hoa;
            case EMonPhai.HoaTranGiao:
                return ETypeSkill.Tho;
            case EMonPhai.BachDeLau:
                return ETypeSkill.Kiem;
            default:
                return ETypeSkill.None;
        }
    }

    public bool CheckIsEqualETypeSkill(CharacterCombat caster)
    {
        return CheckIsEqualETypeSkill(caster, this.eTypeSkill);
    }

    public static bool CheckIsEqualETypeSkill(CharacterCombat caster, ETypeSkill[] skill)
    {
        if (caster.thanThongCurrentUse == null) return false;
        Debug.Log("Check Equal " + caster.thanThongCurrentUse.thanThong.name);
        // Kiểm tra hệ của Skill đang dùng có khớp với Công Pháp (SO) này không
        bool isExecute = skill.Any(skill => caster.thanThongCurrentUse.thanThong.eTypeSkill.Contains(skill));
        Debug.Log("Check Equal " + isExecute);
        return isExecute;
    }

    public bool CheckIsEqualETypeSkill(ETypeSkill[] eTypeSkills)
    {
        bool isExecute = CheckIsEqualETypeSkill(eTypeSkills, this.eTypeSkill);
        Debug.Log("Check Equal " + isExecute);
        return isExecute;
    }

    public static bool CheckIsEqualETypeSkill(ETypeSkill[] eTypeSkills, ETypeSkill[] eTypeSkillsCurrent)
    {
        bool isExecute = eTypeSkills.Any(skill => eTypeSkillsCurrent.Contains(skill));
        Debug.Log("Check Equal " + isExecute);
        return isExecute;
    }

    public void KiemY(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, 1);
    }

    public static BaseSkill.ETypeSkill GetETypeSkillWithLinhKhi(ELinhKhi elinhKhi)
    {
        switch (elinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                return BaseSkill.ETypeSkill.Kim;
            case ELinhKhi.Moc:
                return BaseSkill.ETypeSkill.Moc;
            case ELinhKhi.Thuy:
                return BaseSkill.ETypeSkill.Thuy;
            case ELinhKhi.Hoa:
                return BaseSkill.ETypeSkill.Hoa;
            case ELinhKhi.Tho:
                return BaseSkill.ETypeSkill.Tho;
            case ELinhKhi.Ma:
                break;
            default:
                break;
        }

        return ETypeSkill.None;
    }

    public override int ByteSize()
    {
        return base.ByteSize() + eTierSkill.ByteSize() + eTypeSkill.ByteSize()
               + eKeywordSkillHave.ByteSize() + eKeywordBuffSkillHave.ByteSize()
               + baseTimeLearn.ByteSize();
    }

    public void Deserialize(IReader reader, ref BaseSkill value)
    {
        BaseData baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.eTierSkill = reader.Read<ETierSkill>();
        value.eTypeSkill = reader.ReadArray<ETypeSkill>();
        value.eKeywordSkillHave = reader.ReadArray<EKeyWord>();
        value.eKeywordBuffSkillHave = reader.ReadArray<EBuffAndDeBuff>();
        value.baseTimeLearn = reader.Read<int>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.Write(eTierSkill);
        writer.WriteArray(eTypeSkill);
        writer.WriteArray(eKeywordSkillHave);
        writer.WriteArray(eKeywordBuffSkillHave);
        writer.Write(baseTimeLearn);
    }
}