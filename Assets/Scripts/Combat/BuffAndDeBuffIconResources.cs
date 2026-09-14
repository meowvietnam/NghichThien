using System.Collections.Generic;
using LuyenKhi;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "BuffAndDeBuffIconResources")]
    public class BuffAndDeBuffIconResources : ScriptableObject
    {
        [field: SerializeField] public List<CombatManager.BuffOrDebuffIcon> BuffOrDebuffIcons { get; private set; }

        public Sprite GetSprite(EffectType type)
        {
            return GetSprite(Convert(type));
        }

        public Sprite GetSprite(EffectType? type)
        {
            return type.HasValue ? GetSprite(type.Value) : null;
        }

        public Sprite GetSprite(EBuffAndDeBuff type)
        {
            if (type == EBuffAndDeBuff.None || BuffOrDebuffIcons == null)
            {
                return null;
            }

            foreach (CombatManager.BuffOrDebuffIcon icon in BuffOrDebuffIcons)
            {
                if (icon.eType == type)
                {
                    return icon.sprite;
                }
            }

            return null;
        }

        public static EBuffAndDeBuff Convert(EffectType type)
        {
            switch (type)
            {
                case EffectType.Bui:
                    return EBuffAndDeBuff.Tran;
                case EffectType.Cat:
                    return EBuffAndDeBuff.Cat;
                case EffectType.ChanHoa:
                    return EBuffAndDeBuff.ChanHoa;
                case EffectType.ChanKhi:
                    return EBuffAndDeBuff.ChanKhi;
                case EffectType.ChanKiem:
                    return EBuffAndDeBuff.ChanKiem;
                case EffectType.ChanKim:
                    return EBuffAndDeBuff.ChanKim;
                case EffectType.ChanMoc:
                    return EBuffAndDeBuff.ChanMoc;
                case EffectType.ChanThan:
                    return EBuffAndDeBuff.ChanThan;
                case EffectType.ChanTho:
                    return EBuffAndDeBuff.ChanTho;
                case EffectType.ChanThuy:
                    return EBuffAndDeBuff.ChanThuy;
                case EffectType.ChayMau:
                    return EBuffAndDeBuff.ChayMau;
                case EffectType.DeTonThuong:
                    return EBuffAndDeBuff.DeTonThuong;
                case EffectType.Diem:
                    return EBuffAndDeBuff.Diem;
                case EffectType.GiamTonThuong:
                    return EBuffAndDeBuff.GiamTonThuong;
                case EffectType.HoaDiem:
                    return EBuffAndDeBuff.HoaDiem;
                case EffectType.HoTrao:
                    return EBuffAndDeBuff.HoTrao;
                case EffectType.KiemKhi:
                    return EBuffAndDeBuff.KiemKhi;
                case EffectType.NguHoa:
                    return EBuffAndDeBuff.NguHoa;
                case EffectType.NguHoaHiep:
                    return EBuffAndDeBuff.NguHoaHiep;
                case EffectType.NguKhiHiep:
                    return EBuffAndDeBuff.NguKhiHiep;
                case EffectType.NguKiemHiep:
                    return EBuffAndDeBuff.NguKiemHiep;
                case EffectType.NguKim:
                    return EBuffAndDeBuff.NguKim;
                case EffectType.NguKimHiep:
                    return EBuffAndDeBuff.NguKimHiep;
                case EffectType.NguMoc:
                    return EBuffAndDeBuff.NguMoc;
                case EffectType.NguMocHiep:
                    return EBuffAndDeBuff.NguMocHiep;
                case EffectType.NgungThan:
                    return EBuffAndDeBuff.NgungThan;
                case EffectType.NguThanHiep:
                    return EBuffAndDeBuff.NguThanThucHiep;
                case EffectType.NguTho:
                    return EBuffAndDeBuff.NguTho;
                case EffectType.NguThoHiep:
                    return EBuffAndDeBuff.NguThoHiep;
                case EffectType.NguThuy:
                    return EBuffAndDeBuff.NguThuy;
                case EffectType.NguThuyHiep:
                    return EBuffAndDeBuff.NguThuyHiep;
                case EffectType.Nhanh:
                    return EBuffAndDeBuff.TangToc;
                case EffectType.PhanTonThuong:
                    return EBuffAndDeBuff.PhanTonThuong;
                case EffectType.PhaThe:
                    return EBuffAndDeBuff.PhaThe;
                case EffectType.QuanQuanh:
                    return EBuffAndDeBuff.TrienNhieu;
                case EffectType.SucThe:
                    return EBuffAndDeBuff.SucThe;
                case EffectType.SuongGia:
                    return EBuffAndDeBuff.SuongGia;
                case EffectType.ThanThuong:
                    return EBuffAndDeBuff.ThanThuong;
                case EffectType.ThieuDot:
                    return EBuffAndDeBuff.ThieuDot;
                case EffectType.TichDoc:
                    return EBuffAndDeBuff.TichDoc;
                case EffectType.TichHoa:
                    return EBuffAndDeBuff.TichHoa;
                case EffectType.TichKim:
                    return EBuffAndDeBuff.TichKim;
                case EffectType.TichMoc:
                    return EBuffAndDeBuff.TichMoc;
                case EffectType.TichThuy:
                    return EBuffAndDeBuff.TichThuy;
                case EffectType.TriLieuLieu:
                    return EBuffAndDeBuff.TriLieu;
                case EffectType.TroiDu:
                    return EBuffAndDeBuff.TroiChan;
                case EffectType.TrungDoc:
                    return EBuffAndDeBuff.TrungDoc;
                default:
                    return EBuffAndDeBuff.None;
            }
        }
    }
}
