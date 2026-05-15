using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AllMaps.NgoDao;
using Network;
using Network.Extensions;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Skill;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace DumpGame
{
    public static class ReflectionUtils
    {
        public static bool IsOverride(System.Type type, string methodName)
        {
            var method = type.GetMethod(
                methodName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );
            if (method == null)
                return false;
            return method.GetBaseDefinition().DeclaringType != method.DeclaringType;
        }
    }

    public enum Type
    {
        NgoDao,
        CongPhap,
        ThanThong
    }

    public enum SerializeType
    {
        Normal,
        Array,
        List,
        String,
    }

    [Serializable]
    public class NDCPTTManager : IMemorySerializable<NDCPTTManager>
    {
        [field: SerializeField] public List<NgoDao> NgoDaos { get; set; }
        [field: SerializeField] public List<CongPhap> CongPhaps { get; set; }
        [field: SerializeField] public List<ThanThong> ThanThongs { get; set; }

        private System.Type[] types =
        {
            typeof(AmDuongDong),
            typeof(AmDuongKinh),
            typeof(AmDuongLuongNghiKiem),
            typeof(AnhNac),
            typeof(AnhSat),
            typeof(AnHuPha),
            typeof(AnNacThuat),
            typeof(AtMocDieuPhap),
            typeof(BachDeKiemQuyet),
            typeof(BachDeThanhKiem),
            typeof(BachQuyCauLinhTran),
            typeof(BanCan),
            typeof(BangCo),
            typeof(BangNhamThuat),
            typeof(BangPhachKinh),
            typeof(BangSuongQuyet),
            typeof(BangTamQuyet),
            typeof(BangVuThuat),
            typeof(BaoNhien),
            typeof(BaoPhac),
            typeof(BaoThapQuanTuongPhap),
            typeof(BasicCPBuffDame),
            typeof(BasicCPBuffDame2),
            typeof(BasicTTBuffPercentLinhKhi),
            typeof(BasicTTDealDame),
            typeof(BasicTTHapThuLinhKhi),
            typeof(BasicTTHoTrao),
            typeof(BatCucBang),
            typeof(BaThe),
            typeof(BatQuai),
            typeof(BeKhi),
            typeof(BonLoiQuyet),
            typeof(CanKhonBachXaoKinh),
            typeof(CanKhonHoaKiemQuyet),
            typeof(CapHuyetDang),
            typeof(CauHonTran),
            typeof(ChamVuQuyet),
            typeof(ChanHuyet),
            typeof(ChanLongCuuBien),
            typeof(ChanLongHuyetNo),
            typeof(ChanNguyenHuyenDien),
            typeof(ChichHoa),
            typeof(ChichMachChi),
            typeof(ChichViemLuanHoi),
            typeof(ChichViemTamTaiTran),
            typeof(ChietTinhBoKhiQuyet),
            typeof(ChiThuy),
            typeof(ChuHaiQuyet),
            typeof(ChuongDocQuyet),
            typeof(CPBuffDameOrHoTrao),
            typeof(CPBuffLinhCanCombat),
            typeof(CucAmLanhHoa),
            typeof(CuLinhTrao),
            typeof(CuongKimMocThuyHoaTho),
            typeof(CuongLanQuyet),
            typeof(CuongNhuTinhTe),
            typeof(CuongThuyAn),
            typeof(CuuChuyen),
            typeof(CuuChuyenDuThanKiem),
            typeof(CuuCungDienDaoTran),
            typeof(CuuCungSatTran),
            typeof(CuuHuyenNoiKinh),
            typeof(CuuLongDanLoiQuyet),
            typeof(CuuLongThanHoaTrao),
            typeof(CuuThienKiepHoa),
            typeof(CuuTrungDoTap),
            typeof(CuuUThu),
            typeof(CuuVanLuanHoiQuyet),
            typeof(DaiDeLuanHoaKinh),
            typeof(DaiLaThienHuyenQuyet),
            typeof(DaiNguHanhCamTienThu),
            typeof(DaiNguHanhTran),
            typeof(DaiThonDiemThuat),
            typeof(DaiThonDietThuat),
            typeof(DaiTuDiThuat),
            typeof(DaiVanNhamThuat),
            typeof(DamHonThuat),
            typeof(DanDocChu),
            typeof(DanDuongAn),
            typeof(DanKhi),
            typeof(DanLuc),
            typeof(DanLucThuat),
            typeof(DiaLongThuat),
            typeof(DiaSatKiemTran),
            typeof(DiaSatNgungKet),
            typeof(DichMach),
            typeof(DiemLinhChieuLai),
            typeof(DiepDiemQuyet),
            typeof(DietLinhChuong),
            typeof(DietLinhQuyet),
            typeof(DiTranPhaTran),
            typeof(DoanKim),
            typeof(DoatLinhQuyet),
            typeof(DoatLinhThuat),
            typeof(DongBang),
            typeof(DoThienLietHoaTienTran),
            typeof(DucHoa),
            typeof(DucHoaChu),
            typeof(DuLinhTran),
            typeof(DungHoaThuan),
            typeof(DungThuanQuyet),
            typeof(DuongDaoThuat),
            typeof(DuongXuanBachHoaQuyet),
            typeof(DuThanNguThuyQuyet),
            typeof(GoGiap),
            typeof(HacHoiTuong),
            typeof(HacUeCham),
            typeof(HangMaToaCotTran),
            typeof(HanSuongThu),
            typeof(HauThoAn),
            typeof(HoaCat),
            typeof(HoaDiem),
            typeof(HoaDiemThuat),
            typeof(HoaDoc),
            typeof(HoaHuyetDocChuong),
            typeof(HoaKinh),
            typeof(HoanAnhDien),
            typeof(HoanAnhLuc),
            typeof(HoaNhatDo),
            typeof(HoanLinhChu),
            typeof(HoanTran),
            typeof(HoaSonKinh),
            typeof(HoatHuyetThuat),
            typeof(HoaTranChanQuyet),
            typeof(HoaTranKiemPhap),
            typeof(HoaTranQuyet),
            typeof(HoaVuQuyet),
            typeof(HoaVuThuat),
            typeof(HoaXaThuat),
            typeof(HoChu),
            typeof(HoiNguyen),
            typeof(HoiPhongAn),
            typeof(HoiXuanQuyet),
            typeof(HongLienNghiepDiem),
            typeof(HonKhieu),
            typeof(HonNguyenDanKhiQuyet),
            typeof(HonNguyenTranDanLuc),
            typeof(HonThienThuat),
            typeof(HoTheKiemCuong),
            typeof(HuDocChu),
            typeof(HuyenAmKinh),
            typeof(HuyenAnhThanHanhQuyet),
            typeof(HuyenBangKinh),
            typeof(HuyenBangNgungCotThuat),
            typeof(HuyenBangVu),
            typeof(HuyenDuongBoDiemQuyet),
            typeof(HuyenLinhThuat),
            typeof(HuyenMonDanKhiChanQuyet),
            typeof(HuyenThanhThienDienLuc),
            typeof(HuyenThuyChanPhap),
            typeof(HuyenVuChanPhap),
            typeof(HuyenVuKiemTran),
            typeof(HuyenVuThoNapThuat),
            typeof(HuyenVuThuat),
            typeof(HuyetAnhThuat),
            typeof(HuyetDocKinh),
            typeof(HuyetHonLuc),
            typeof(HuyetTePheHonThuat),
            typeof(KhaiSonQuyet),
            typeof(KhienLinhChu),
            typeof(KhiLieu),
            typeof(KhinhKiem),
            typeof(KhiToanTram),
            typeof(KhoaiKiem),
            typeof(KhoLinhKinh),
            typeof(KhoTanCamLai),
            typeof(KhuAnhThuat),
            typeof(KhuDangThuat),
            typeof(KhuHanAn),
            typeof(KhuTran),
            typeof(KhuVatThuat),
            typeof(KiemCuongQuyet),
            typeof(KiemDienNhanDiaThien),
            typeof(KiemDienThienDaoTien),
            typeof(KiemKhi),
            typeof(KiemThuatYeuQuyet),
            typeof(KienNghi),
            typeof(KimCuongDiSonQuyet),
            typeof(KimCuongLuyenTheQuyet),
            typeof(KimCuongTram),
            typeof(KimHongKiemPhap),
            typeof(KimHongKiemQuyet),
            typeof(KimHongKiemTran),
            typeof(KimHongQuanNhat),
            typeof(KimMangThuat),
            typeof(KimNguyenKiemMang),
            typeof(KinhDao),
            typeof(KinhDaoKiemQuyet),
            typeof(KinhHonChu),
            typeof(KyMon),
            typeof(KyMonDonGiapLuc),
            typeof(KySinh),
            typeof(KySinhDanHuyetThuat),
            typeof(KyTruongQuyet),
            typeof(LacAnh),
            typeof(LacHonTran),
            typeof(LacNhamThuat),
            typeof(LacThachTran),
            typeof(LaHauChanHonQuyet),
            typeof(LamThuong),
            typeof(LangVanQuyet),
            typeof(LanHoa),
            typeof(LaYenBo),
            typeof(LeHoa),
            typeof(LeKiemQuyet),
            typeof(LiemTuc),
            typeof(LiemTucThuat),
            typeof(LienNhac),
            typeof(LietBangQuyet),
            typeof(LietDiemQuyet),
            typeof(LietHoaChuong),
            typeof(LietKhiTram),
            typeof(LinhDangAn),
            typeof(LinhLucHoTrao),
            typeof(LinhQuangAn),
            typeof(LinhSaThuat),
            typeof(LinhTeChuong),
            typeof(LinhTheAn),
            typeof(LinhThuyQuyet),
            typeof(LinhThuyTienTran),
            typeof(LinhTucThuat),
            typeof(LoanAnhSat),
            typeof(LoanDangThuat),
            typeof(LoanHon),
            typeof(LongTuongThonHaiQuyet),
            typeof(LuongNghi),
            typeof(LuongNghiLinhNhanQuyet),
            typeof(LuuQuangChanQuyet),
            typeof(LuyenAnhLuc),
            typeof(LuyenCot),
            typeof(LyHoaKiemPhap),
            typeof(LyHoaTuyetViem),
            typeof(MaDiemQuyet),
            typeof(ManhQuyThamKhongThu),
            typeof(ManKiem),
            typeof(ManNguuHoTheQuyet),
            typeof(MeTungBo),
            typeof(NDSucThe),
            typeof(NghichMach),
            typeof(NghiepHoaAn),
            typeof(NgocVanCong),
            typeof(NgoDaoBuffStats),
            typeof(NguDinhKhaiSonQuyet),
            typeof(NguDocChu),
            typeof(NguDocNhiepHonTran),
            typeof(NguDocThucTamQuyet),
            typeof(NguHanh),
            typeof(NguHanhKiemQuyet),
            typeof(NguHanhThienCuong),
            typeof(NguKiemCuuQuyet),
            typeof(NguKiemThuat),
            typeof(NguKimMocThuyHoaTho),
            typeof(NguLinhDien),
            typeof(NgungMach),
            typeof(NgungSa),
            typeof(NgungSaQuyet),
            typeof(NgungThanChu),
            typeof(NgungThuyQuyLuu),
            typeof(NguNoiQuanTuongPhap),
            typeof(NguPhuDoanNguyenQuyet),
            typeof(NguPhuongThienDiaQuyet),
            typeof(NguQuyPhuThanThuat),
            typeof(NguTangLuyenTranCong),
            typeof(NguThe),
            typeof(NguThuyQuyet),
            typeof(NguTran),
            typeof(NguULuyenThanQuyet),
            typeof(NguyetBocHanHuy),
            typeof(NguyetHoaLuuChieu),
            typeof(NguyetPhaDanHuQuyet),
            typeof(NhanDiaThienKiem),
            typeof(NhapVi),
            typeof(NhatKhiPhanThanCong),
            typeof(NhienDiemQuyet),
            typeof(NhienHoaThuat),
            typeof(NhienHonChu),
            typeof(NhienHonQuyet),
            typeof(NhienTan),
            typeof(NhieuHonThuat),
            typeof(NhomLua),
            typeof(NhuThuyVoNgan),
            typeof(NietBanQuyet),
            typeof(NoDao),
            typeof(NoDaoQuyet),
            typeof(NoThuyAn),
            typeof(NoThuyThienThuong),
            typeof(OatToanTaoHoa),
            typeof(PhanAnhThu),
            typeof(PhanDuongQuyet),
            typeof(PhanHoaDiCanhThuat),
            typeof(PhanHoaThoDiemQuyet),
            typeof(PhanKhiThuat),
            typeof(PhanKimAn),
            typeof(PhanLinhQuyet),
            typeof(PhanMocQuyet),
            typeof(PhanTamQuyet),
            typeof(PhanThienCong),
            typeof(PhaPhuTramChu),
            typeof(PhaThe),
            typeof(PheAnh),
            typeof(PheHonQuyet),
            typeof(PheTam),
            typeof(PhiChamThuat),
            typeof(PhiTinhKiemQuyet),
            typeof(PhiVanKinh),
            typeof(PhongDoQuyVuc),
            typeof(PhongHau),
            typeof(PhucTheAn),
            typeof(PhuongLinhYeuHoa),
            typeof(PhuongVien),
            typeof(PhuTranLuyenTamQuyet),
            typeof(QuangViNgungNguyetCong),
            typeof(QuanKhi),
            typeof(QuyAnhTrongTrong),
            typeof(QuyNguyenChu),
            typeof(QuyNguyenKiemKhi),
            typeof(QuyNhat),
            typeof(QuyTran),
            typeof(QuyTucThuat),
            typeof(SaLao),
            typeof(SaPhuoc),
            typeof(SaTang),
            typeof(SiMiQuyQuyen),
            typeof(SinhKimMocThuyHoaTho),
            typeof(SinhSinhTaoHoaQuyet),
            typeof(SongKich),
            typeof(SucLinhQuyet),
            typeof(SucLuc),
            typeof(SucThe),
            typeof(SungDoanh),
            typeof(SuongThienHanNguyetTran),
            typeof(TamChuyenLuyenSonQuyet),
            typeof(TamDuongHuyenHoaBien),
            typeof(TamMuoiChanHoa),
            typeof(TamTaiHoaGiapKiep),
            typeof(TamTaiKiemTran),
            typeof(TamTaiQuyNguyenKiem),
            typeof(TamThinh),
            typeof(TamThonChanNguyenChuong),
            typeof(TamTrai),
            typeof(TamTuongSinhLinhQuyet),
            typeof(TamTuongThoatCotKiep),
            typeof(TangKiemQuyet),
            typeof(TangLyQuyet),
            typeof(TanNguyenCoKhiCong),
            typeof(TapHonThuat),
            typeof(TapKhi),
            typeof(TapTrung),
            typeof(TatPhongKinh),
            typeof(TauHuyet),
            typeof(TeHuyetTrao),
            typeof(ThachPhuThuat),
            typeof(ThaiAmHuyenDocCong),
            typeof(ThaiAmKyMonTran),
            typeof(ThaiAmLucHuCong),
            typeof(ThaiAtHuyenCong),
            typeof(ThaiAtKiemQuyet),
            typeof(ThaiCuc),
            typeof(ThaiDuongCong),
            typeof(ThaiNhienVoBa),
            typeof(ThaiThanhDieuPhap),
            typeof(ThaiThanhHuyenThuat),
            typeof(ThaiThuongHoaLongChanQuyet),
            typeof(ThaiThuongHoaLongQuyet),
            typeof(ThaiTucKinh),
            typeof(ThanhDangQuyet),
            typeof(ThanhLienKiemQuyet),
            typeof(ThanhNguyetLoHoaThe),
            typeof(ThanHoangAn),
            typeof(ThanHuyenLuyenTheQuyet),
            typeof(ThanhXaKinh),
            typeof(ThanhXaYeuQuyet),
            typeof(ThanKiemQuyet),
            typeof(ThanMonDoKhiQuyet),
            typeof(ThanNiemQuyet),
            typeof(ThanThucPhap),
            typeof(ThanUyThuc),
            typeof(ThatSatKiemQuyet),
            typeof(ThauThachThuat),
            typeof(ThienBangDiaLiet),
            typeof(ThienDanThuat),
            typeof(ThienKiemQuyet),
            typeof(ThienMaGiaiThe),
            typeof(ThienNhanThuat),
            typeof(ThienPhong),
            typeof(ThienQuanTram),
            typeof(ThienTinhHoaHaiQuyet),
            typeof(ThienTinhNhuanThuyQuyet),
            typeof(ThienTinhNhuocVu),
            typeof(ThietGiapChu),
            typeof(ThieuDotHoaAn),
            typeof(ThieuDuongQuyet),
            typeof(ThiThanThu),
            typeof(ThoDiemQuyet),
            typeof(ThoiTamTrao),
            typeof(ThoNapCongPhap),
            typeof(ThonDiem),
            typeof(ThonDiemQuyet),
            typeof(ThongBaoHopNguyenQuyet),
            typeof(ThongHoa),
            typeof(ThongHuyet),
            typeof(ThongKim),
            typeof(ThongMinh),
            typeof(ThongMinhKiem),
            typeof(ThongMinhKiemQuyet),
            typeof(ThongMoc),
            typeof(ThongTho),
            typeof(ThongThuy),
            typeof(ThongUMinhDo),
            typeof(ThonLinhChu),
            typeof(ThonNhatThanVienBien),
            typeof(ThoTangLuanChuyenTran),
            typeof(ThoTangMeTranTran),
            typeof(ThoTuongTienTran),
            typeof(ThucCotDocHoa),
            typeof(ThucThanThu),
            typeof(ThuHonThuat),
            typeof(ThuHuyet),
            typeof(ThuLaoThuat),
            typeof(ThuNguyetHoThien),
            typeof(ThuongVanAmNguyet),
            typeof(ThuyLieuThuat),
            typeof(ThuyLinhDieuPhap),
            typeof(ThuyLongDan),
            typeof(ThuyVuQuyet),
            typeof(TichDietChi),
            typeof(TiDocChanDiem),
            typeof(TietLinhThu),
            typeof(TietMach),
            typeof(TieuMeTungTran),
            typeof(TieuTuDiCamPhap),
            typeof(TinhDieuNgungHonQuyet),
            typeof(TinhHaKiemDien),
            typeof(TinhHaKiemPhap),
            typeof(TinhHaKiemTran),
            typeof(TinhKhi),
            typeof(TinhKhiQuyet),
            typeof(TinhThanThanNiemThe),
            typeof(TinhThuyVoBa),
            typeof(TinhViThanDietQuyet),
            typeof(ToaHoaQuyet),
            typeof(ToiHuyetChuong),
            typeof(ToKimChanPhap),
            typeof(TramThien),
            typeof(TramThienKiem),
            typeof(TranHonChuong),
            typeof(TranY),
            typeof(TrienMocQuyet),
            typeof(TrienTiChu),
            typeof(TrieuNguyenNguKimQuyet),
            typeof(TrongKiem),
            typeof(TrongKiemQuyet),
            typeof(TrongNguyenLuanChuyenQuyet),
            typeof(TrongNguyenNhuYThe),
            typeof(TrongNguyenVoPhongCong),
            typeof(TrongNham),
            typeof(TrongNhamQuyet),
            typeof(TrucSonKiemPhap),
            typeof(TrungHuyet),
            typeof(TruongSinhQuyet),
            typeof(TuDiem),
            typeof(TuDuongHonNguyenKinh),
            typeof(TuKhi),
            typeof(TuLanh),
            typeof(TuLienKimAnhTrao),
            typeof(TuLinh),
            typeof(TuLinhTran),
            typeof(TungDiaKimQuang),
            typeof(TuNguyenChu),
            typeof(TuongGiapCong),
            typeof(TuongSinh),
            typeof(TuPhuongDuLinhDo),
            typeof(TuQuyThuat),
            typeof(TuTieuCong),
            typeof(TuTranQuyet),
            typeof(TuTranThuat),
            typeof(TuTuong),
            typeof(TuTuongThienDanLuc),
            typeof(TuyenThuyAn),
            typeof(TuyetTran),
            typeof(UHonLuc),
            typeof(UViemQuyet),
            typeof(UyenHuyetMinhDo),
            typeof(VanAnhLuc),
            typeof(VanHonLuc),
            typeof(VanKhiThuat),
            typeof(VanKiemQuyet),
            typeof(VanThoQuyTran),
            typeof(VanThuyKiemDien),
            typeof(VanTranHoaTho),
            typeof(VanTranQuyLinhQuyet),
            typeof(VanTuongKySinhQuyet),
            typeof(VanTuongTrienThanQuyet),
            typeof(VanVuKiemPhap),
            typeof(VoAnhThoi),
            typeof(VongLuongQuyQuyen),
            typeof(VongUuQuyet),
            typeof(VuAnThuat),
            typeof(VuTuongHoaThan),
            typeof(XichViemLinhTran),
            typeof(XuanThaoQuyet),
            typeof(XuanTiAn)
        };

        private object Read(System.Type type, IReader reader)
        {
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod(
                "Deserialize",
                new[]
                {
                    typeof(IReader),
                    type.MakeByRefType()
                });

            object[] args =
            {
                reader,
                instance
            };
            method.Invoke(instance, args);
            return instance;
        }

        private T Read<T>(IReader reader) where T : class
        {
            var typeIndex = reader.Read<ushort>();
            var type = types[typeIndex];
            var instance = Read(type, reader);
            return instance as T;
        }

        public void Deserialize(IReader reader, ref NDCPTTManager value)
        {
            var ndCount = reader.Read<int>();
            value.NgoDaos = Enumerable.Range(0, ndCount).Select(_ => Read<NgoDao>(reader)).ToList();
            var cpCount = reader.Read<int>();
            value.CongPhaps = Enumerable.Range(0, cpCount).Select(_ => Read<CongPhap>(reader)).ToList();
            var ttCount = reader.Read<int>();
            value.ThanThongs = Enumerable.Range(0, ttCount).Select(_ => Read<ThanThong>(reader)).ToList();
        }

        public int ByteSize()
        {
            return 12
                   + NgoDaos.Sum(x => x.ByteSize() + 2)
                   + CongPhaps.Sum(x => x.ByteSize() + 2)
                   + ThanThongs.Sum(x => x.ByteSize() + 2);
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(NgoDaos.Count);
            NgoDaos.ForEach(x =>
            {
                var type = x.GetType();
                var typeIndex = (ushort)Array.IndexOf(types, type);
                writer.Write(typeIndex);
                writer.Write(x);
            });
            writer.Write(CongPhaps.Count);
            CongPhaps.ForEach(x =>
            {
                var type = x.GetType();
                var typeIndex = (ushort)Array.IndexOf(types, type);
                writer.Write(typeIndex);
                writer.Write(x);
            });
            writer.Write(ThanThongs.Count);
            ThanThongs.ForEach(x =>
            {
                var type = x.GetType();
                var typeIndex = (ushort)Array.IndexOf(types, type);
                writer.Write(typeIndex);
                x.Serialize(writer);
            });
        }
    }

    [Serializable]
    public class SerializeMember
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public SerializeType SerializeType { get; set; }
        [field: SerializeField] public string Type { get; set; }
    }

    [Serializable]
    public class MetaData
    {
        [field: SerializeField] public Type Type { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string ClassName { get; set; }
        [field: SerializeField] public string ParentClassName { get; set; }
        [field: SerializeField] public string ScriptPath { get; set; }
        [field: SerializeField] public bool HasBuffValue { get; set; }
        [field: SerializeField] public List<SerializeMember> SerializeMembers { get; set; } = new();
    }

    [Serializable]
    public class MemberDefinition
    {
        [field: SerializeField] public string Type { get; set; }
        [field: SerializeField] public List<string> Members { get; set; } = new();
    }

    public class MemberInfo
    {
        public System.Type Type { get; set; }
        public string Name { get; set; }
    }

    public class Dump : MonoBehaviour
    {
        [SerializeField] private List<NgoDao> ngoDaos;
        [SerializeField] private List<CongPhap> congPhaps;
        [SerializeField] private List<ThanThong> thanThongs;
        [SerializeField] public List<MetaData> metaDatas;
        [SerializeField] public List<MemberDefinition> memberDefinitions;

        [Button("Refresh")]
        public void Refresh()
        {
            string[] guids = AssetDatabase.FindAssets("t:BaseData");
            Debug.Log($"Found {guids.Length} BaseData assets");
            var list = new List<BaseData>();
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BaseData data = AssetDatabase.LoadAssetAtPath<BaseData>(path);
                list.Add(data);
            }

            ngoDaos = list.OfType<NgoDao>().ToList();
            congPhaps = list.OfType<CongPhap>().ToList();
            thanThongs = list.OfType<ThanThong>().ToList();
        }

        public string GetPath(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            path = Path.GetFullPath(path);
            return path;
        }

        public string GetScriptPath(ScriptableObject so)
        {
            var monoScript = MonoScript.FromScriptableObject(so);
            var path = GetPath(monoScript);
            return path;
        }

        private List<MemberInfo> GetMemberInfos(System.Type classType)
        {
            var fields = classType.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly
            );
            var properties = classType.GetProperties(BindingFlags.Instance |
                                                     BindingFlags.Public |
                                                     BindingFlags.NonPublic |
                                                     BindingFlags.DeclaredOnly);
            var members = fields.Select(f => new MemberInfo { Type = f.FieldType, Name = f.Name }).ToList();
            members.AddRange(properties.Select(f => new MemberInfo { Type = f.PropertyType, Name = f.Name }));
            return members;
        }

        public List<SerializeMember> GetMembers(System.Type classType)
        {
            var serializeMembers = new List<SerializeMember>();
            var definition = memberDefinitions.FirstOrDefault(x => x.Type == classType.Name);
            if (definition != null)
            {
                var members = GetMemberInfos(classType);
                members = members.Where(f => definition.Members.Contains(f.Name)).ToList();
                serializeMembers = members.Select(x =>
                {
                    SerializeType serializeType = SerializeType.Normal;
                    string type = x.Type.Name;
                    if (x.Type.IsArray)
                    {
                        serializeType = SerializeType.Array;
                        type = x.Type.GetElementType().Name;
                    }
                    else if (x.Type.IsGenericType)
                    {
                        var genericType = x.Type.GetGenericTypeDefinition();
                        if (genericType == typeof(List<>))
                        {
                            serializeType = SerializeType.List;
                            type = x.Type.GetGenericArguments()[0].Name;
                        }
                    }
                    else if (x.Type == typeof(string))
                    {
                        serializeType = SerializeType.String;
                    }

                    return new SerializeMember()
                    {
                        Name = x.Name,
                        SerializeType = serializeType,
                        Type = type,
                    };
                }).ToList();
            }

            return serializeMembers;
        }

        [Button]
        public void GetMemberDefinitions()
        {
            memberDefinitions = new();
            foreach (var ngoDao in ngoDaos)
            {
                if (!(ngoDao is NgoDaoBuffStats))
                {
                    continue;
                }

                var type = ngoDao.GetType();
                if (memberDefinitions.Any(x => x.Type == type.Name))
                {
                    continue;
                }

                var members = GetMemberInfos(type);
                members = members.ToList();
                var definition = new MemberDefinition()
                {
                    Type = type.Name,
                    Members = members.Select(x => x.Name).ToList(),
                };
                memberDefinitions.Add(definition);
            }

            foreach (var congPhap in congPhaps)
            {
                var type = congPhap.GetType();
                if (memberDefinitions.Any(x => x.Type == type.Name))
                {
                    continue;
                }

                var hasBuffValue = ReflectionUtils.IsOverride(type, "EquipCongPhap");
                if (!hasBuffValue)
                {
                    continue;
                }

                var members = GetMemberInfos(type);
                members = members.ToList();
                var definition = new MemberDefinition()
                {
                    Type = type.Name,
                    Members = members.Select(x => x.Name).ToList(),
                };
                memberDefinitions.Add(definition);
            }
        }

        public MetaData GetMetaData(ScriptableObject item)
        {
            var type = Type.CongPhap;
            switch (item)
            {
                case CongPhap cp:
                    type = Type.CongPhap;
                    break;
                case ThanThong th:
                    type = Type.ThanThong;
                    break;
                case NgoDao nd:
                    type = Type.NgoDao;
                    break;
            }

            var classType = item.GetType();
            var parentClassType = classType.BaseType;
            var hasBuffValue = false;
            switch (type)
            {
                case Type.CongPhap:
                    hasBuffValue = ReflectionUtils.IsOverride(classType, "EquipCongPhap");
                    break;
                case Type.ThanThong:
                    break;
                case Type.NgoDao:
                    hasBuffValue = item is NgoDaoBuffStats;
                    break;
            }

            var serializeMembers = GetMembers(classType);
            var metaData = new MetaData()
            {
                Type = type,
                Name = item.name,
                ClassName = classType.Name,
                ParentClassName = parentClassType?.Name,
                HasBuffValue = hasBuffValue,
                ScriptPath = GetScriptPath(item),
                SerializeMembers = serializeMembers,
            };
            return metaData;
        }

        [Button]
        public void ExtractMetaData()
        {
            metaDatas = ngoDaos.Select(x => GetMetaData(x))
                .Concat(congPhaps.Select(y => GetMetaData(y)))
                .Concat(thanThongs.Select(z => GetMetaData(z)))
                .ToList();
            metaDatas = metaDatas
                .GroupBy(x => x.ScriptPath)
                .Select(x => x.First())
                .ToList();
            var json = JsonConvert.SerializeObject(metaDatas, Formatting.Indented);
            File.WriteAllText(Path.Combine(Application.dataPath, "Exports", "metadatas.json"), json);
        }

        [SerializeField] private NDCPTTManager manager;

        [Button]
        public void DumpAll()
        {
            var t = new NDCPTTManager()
            {
                NgoDaos = ngoDaos,
                CongPhaps = congPhaps,
                ThanThongs = thanThongs,
            };
            var byteSize = t.ByteSize();
            Debug.Log(byteSize);
            var writer = new Writer(byteSize);
            writer.Write(t);
            var reader = new Reader(writer.Buffer);
            manager = reader.Read<NDCPTTManager>();
        }
    }
}