using CreateCharacter;
using Sirenix.OdinInspector;
using UnityEngine;
using ThanThong = Skill.ThanThong;

namespace ComboSkillResource.NPCData
{
    public partial class NPCData : ScriptableObject
    {
        public virtual ETypeVisualChar eTypeVisualChar => ETypeVisualChar.Character;

        [Button("Renew All Keys")]
        private void RenewAllKeys()
        {
            // Làm mới tất cả các khóa trong charStats
            RenewKeysForCharacterStats(charStats);

            // Làm mới tất cả các khóa trong charStatsCreateRef
            RenewKeysForCharacterStats(charStatsCreateRef);
        }

        private void RenewKeysForCharacterStats(CharacterStatsSecure stats)
        {
            if (stats == null) return;

            // Làm mới các SecureInt và SecureFloat
            stats.idChar.RenewKeys();
            stats.percentKim.RenewKeys();
            stats.percentMoc.RenewKeys();
            stats.percentThuy.RenewKeys();
            stats.percentHoa.RenewKeys();
            stats.percentTho.RenewKeys();
            stats.percentMa.RenewKeys();

            stats.tuChat.RenewKeys();
            stats.ngoTinh.RenewKeys();
            stats.thanThuc.RenewKeys();
            stats.tocDoBay.RenewKeys();
            stats.linhThach.RenewKeys();
            stats.hp.RenewKeys();
            stats.hpMax.RenewKeys();
            stats.thoNguyen.RenewKeys();
            stats.tamCanh.RenewKeys();
            stats.idLinhCan.RenewKeys();

            stats.ngoDaoDiem.RenewKeys();
            stats.danDoc.RenewKeys();
            stats.level.RenewKeys();
            stats.tuoiTac.RenewKeys();
            stats.currentExp.RenewKeys();

            stats.idNode.RenewKeys();

            // Làm mới các danh sách SecureInt
            foreach (var item in stats.ngoDaoSaves)
            {
                item.RenewKeys();
            }

            foreach (var item in stats.thanThongOwner)
            {
                item.RenewKeys();
            }

            foreach (var item in stats.thanThongEquip)
            {
                item.RenewKeys();
            }

            foreach (var data in stats.camNgoSaves)
            {
                data.id.RenewKeys();
                data.value.RenewKeys();
            }

            foreach (var data in stats.congPhapOwner)
            {
                data.id.RenewKeys();
                data.value.RenewKeys();
            }

            foreach (var data in stats.relationShipNPC)
            {
                data.id.RenewKeys();
                data.value.RenewKeys();
            }

            foreach (var data in stats.danDuocCombat)
            {
                data.id.RenewKeys();
                data.value.RenewKeys();
            }

            // Làm mới các công pháp used
            stats.donThuatUsed.RenewKeys();
            stats.phuTu1Used.RenewKeys();
            stats.phuTu2Used.RenewKeys();
            stats.phuTu3Used.RenewKeys();
            stats.phuTu4Used.RenewKeys();
            stats.chuTu1Used.RenewKeys();
            stats.chuTu2Used.RenewKeys();
        }

        private void OnValidate()
        {
            RenewAllKeys();
        }

        protected virtual string[] Surnames => new[]
        {
            "Diệp", "Tiêu", "Lâm", "Sở", "Hàn", "Thần", "Lục", "Tô", "Tần", "Mặc",
            "Cố", "Thẩm", "Phương", "Bạch", "Long", "Phượng", "Vân", "Tống", "Khương", "Vũ",
            "Trình", "Bùi", "Liễu", "Tạ", "Viên", "Dương", "Chu", "Phó", "Trang", "Hoắc"
        };

        protected virtual string[] MiddleNames => new[]
        {
            "Thiên", "Thanh", "Vũ", "Vân", "Nhược", "Tuyết", "Mặc", "Tử", "Minh", "Tinh",
            "Linh", "Trường", "Nhất", "Cửu", "Vô", "Thái", "Mộng", "Kiếm", "Ẩn", "Dật",
            "Tĩnh", "Thần", "Hạo", "Giai", "Trường", "Tuyên", "Đông", "Tây", "Bắc", "Nam"
        };

        protected virtual string[] LastNames => new[]
        {
            "Phàm", "Phong", "Trần", "Hiên", "Tuyệt", "Dao", "Thần", "Vũ", "Nguyệt", "Kiếm",
            "Tâm", "Viêm", "Tôn", "Hoàng", "Không", "Ảnh", "Sát", "Ngạo", "Thương", "Lỗi",
            "Sương", "Yên", "Tịch", "Hùng", "Hải", "Sơn", "Lâm", "Hỏa", "Thổ", "Kim"
        };

        public virtual string GenerateName()
        {
            string ho = Surnames[Random.Range(0, Surnames.Length)];
            string dem = MiddleNames[Random.Range(0, MiddleNames.Length)];
            string ten = LastNames[Random.Range(0, LastNames.Length)];

            // Tỉ lệ 30% tên chỉ có 2 chữ (Họ + Tên) cho đa dạng
            if (Random.value > 0.7f)
            {
                return ho + " " + ten;
            }

            return ho + " " + dem + " " + ten;
        }

        [SerializeField] public CharacterStatsSecure charStats;
        [SerializeField] public CharacterStatsSecure charStatsCreateRef;
        [SerializeField] public ComboSkillResource.ComboWithType comboChar;

        public ComboSkillResource.Combo CurrentCombo =>
            comboChar.comboWithDaiCanhGioi[ThanThong.GetDaiCanhGioi(charStats.level) - 1];

        [Button]
        public void LogData()
        {
            Debug.Log(charStats.percentKim.Value);
            Debug.Log("skin tao " + charStats.Skins.Count);
        }

        [Button]
        public virtual void SetBaseStats(GameManager gameConfig)
        {
            if (charStatsCreateRef.skinList.Count <= 0)
            {
                var skinRandom = CharacterGraphic.RandomSkinStatic(charStatsCreateRef.Gender, Random.Range(0, 2) == 0);
                foreach (var kvp in skinRandom.skins)
                {
                    charStatsCreateRef.skinList.Add(new CharacterStatsSecure.SkinData
                        { key = kvp.Key, value = kvp.Value });
                }

                foreach (var kvp in skinRandom.colors)
                {
                    charStatsCreateRef.colorList.Add(new CharacterStatsSecure.ColorData
                        { key = kvp.Key, value = kvp.Value });
                }
            }

            if (string.IsNullOrEmpty(charStatsCreateRef.nameUser))
            {
                charStatsCreateRef.nameUser = GenerateName();
            }

            charStats = charStatsCreateRef.Clone();
            for (int i = 0; i < charStats.level; i++)
            {
                charStats.hpMax += gameConfig.inforLevels[i].hpMaxBuff;
                charStats.hp = new SecureInt(charStats.hpMax);
                charStats.thanThuc += gameConfig.inforLevels[i].thanThucBuff;
                charStats.tocDoBay += gameConfig.inforLevels[i].donTocBuff;
                charStats.thoNguyen += gameConfig.inforLevels[i].thoNguyenBuff;
            }

            int daiCanhGioi = ThanThong.GetDaiCanhGioi(charStats.level);
            int minRandom = 0;
            int maxRandom = 0;
            if (daiCanhGioi == 1)
            {
                minRandom = 15;
                maxRandom = 65;
            }
            else if (daiCanhGioi == 2)
            {
                minRandom = 35;
                maxRandom = 125;
            }
            else if (daiCanhGioi == 3)
            {
                minRandom = 65;
                maxRandom = 150;
            }
            else if (daiCanhGioi == 4)
            {
                minRandom = 100;
                maxRandom = 200;
            }
            else if (daiCanhGioi == 5)
            {
                minRandom = 500;
                maxRandom = 1000;
            }

            charStats.tuoiTac = Random.Range(minRandom, maxRandom);
            charStats.ngoDaoSaves.Clear();
            charStats.congPhapOwner.Clear();
            charStats.thanThongOwner.Clear();
            charStats.thanThongEquip.Clear();
            foreach (var item in CurrentCombo.ngoDaos)
            {
                charStats.ngoDaoSaves.Add(item.Id);
            }

            foreach (var item in CurrentCombo.congPhapIds)
            {
                charStats.congPhapOwner.Add(new CharacterStatsSecure.DataSave { id = item, value = 5 });
            }

            foreach (var item in CurrentCombo.thanThongIds)
            {
                charStats.thanThongEquip.Add(item);
                charStats.thanThongOwner.Add(item);
            }

            charStats.chuTu1Used = charStats.congPhapOwner.Count > 0 ? charStats.congPhapOwner[0].id : -1;
            charStats.phuTu1Used = charStats.congPhapOwner.Count > 1 ? charStats.congPhapOwner[1].id : -1;
            charStats.phuTu2Used = charStats.congPhapOwner.Count > 2 ? charStats.congPhapOwner[2].id : -1;
            charStats.phuTu3Used = charStats.congPhapOwner.Count > 3 ? charStats.congPhapOwner[3].id : -1;
            charStats.phuTu4Used = charStats.congPhapOwner.Count > 4 ? charStats.congPhapOwner[4].id : -1;
            charStats.chuTu2Used = charStats.congPhapOwner.Count > 5 ? charStats.congPhapOwner[5].id : -1;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // Nếu charStatsCreateRef nằm trong script này
            UnityEditor.AssetDatabase.SaveAssets(); // Lưu lại file vật lý
#endif
        }
    }
}