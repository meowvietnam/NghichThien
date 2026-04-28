using UnityEngine;
[CreateAssetMenu(fileName = nameof(DiaSatNgungKet), menuName = "Skill/Thần Thông/" + nameof(DiaSatNgungKet))]

public class DiaSatNgungKet : ThanThong
{
    [SerializeField]
    private int max = 10;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        HapThuVoiGioiHan(caster);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }
    void HapThuVoiGioiHan(CharacterCombat caster)
    {
        int count = 0;
        bool coPhanTuTho = true;

        // Vòng lặp sẽ chạy tiếp NẾU tìm thấy Thổ VÀ chưa quá giới hạn 10 lần
        while (coPhanTuTho && count < max)
        {
            var dicHapThu = caster.DictHapThuLinhKhi(1);

            // Tạm thời đặt là false, nếu trong danh sách có Thổ thì mới bật lại thành true
            coPhanTuTho = false;

            foreach (var item in dicHapThu)
            {
                // Thực hiện hấp thụ linh khí như bình thường
                caster.HapThuLinhKhi(item.Key, item.Value);

                // Kiểm tra nếu là hệ Thổ
                if (item.Key == ELinhKhi.Tho)
                {
                    coPhanTuTho = true;
                }
            }

            // Nếu vòng lặp foreach vừa rồi có hệ Thổ, ta tăng biến đếm lên
            if (coPhanTuTho)
            {
                count++;
            }
        }
    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ một điểm linh khí. Nếu là {GetTextLinhKhi(ELinhKhi.Tho)} hệ linh khí thì tiếp tục lặp lại quá trình. Tối đa {GetColoredText($"{max} lần", "red")}";

    }
}
