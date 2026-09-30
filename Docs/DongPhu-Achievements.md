# Thành tựu Động phủ

## Luật
- Mỗi thành tựu gắn với một chỉ số, có nhiều mốc; chỉ số đếm **trọn đời**. Bảng hiện mốc đầu tiên chưa nhận của mỗi thành tựu; đạt thì bấm "Nhận thưởng".
- Chỉ số (`DongPhuStat`, server):

| Stat | Ý nghĩa | Ghi nhận khi |
| --- | --- | --- |
| WaterHelpGiven | Tưới nước hộ người khác | Tưới hộ thành công (người tưới) |
| WaterHelpReceived | Được tưới hộ | Tưới hộ thành công (chủ động phủ, kể cả offline) |
| LikeGiven | Like động phủ người khác | Like thành công |
| LikeReceived | Được like | Like thành công (chủ động phủ) |
| Plant | Gieo hạt | Gieo hạt thành công |
| Harvest | Thu hoạch | Thu hoạch thành công |
| Level | Cấp động phủ (1 Hoàng … 5 Thần) | Đọc trực tiếp cấp hiện tại |
| SeedBought | Tổng số hạt đã mua | Mua hạt (cộng số lượng) |
| Visit | Thăm động phủ người khác | Mỗi động phủ tối đa 1 lần/ngày (giữ trong bộ nhớ) |

- Dữ liệu cũ: LikeReceived = `DongPhu.LikeCount`, LikeGiven đếm từ `DongPhuLikes`, Level đọc trực tiếp; còn lại bắt đầu từ 0.
- Chỉ chủ động phủ xem/nhận thành tựu của mình. Nút Thành tựu có chấm đỏ khi có mốc nhận được.

## Cấu hình (server)
- File `MTS-Server/backend/MTS/Data/dong-phu-achievements.json` (copy ra thư mục build). Sửa bản đang chạy và lưu: server tự nạp lại sau ~1 giây; file lỗi thì giữ cấu hình cũ, log `DongPhu achievements: invalid ...`.
- `Id` thành tựu/mốc dùng để nhớ đã nhận: **không đổi** sau khi phát hành; `Target` các mốc tăng dần; `{target}` trong `Description` được thay bằng mục tiêu mốc.
- `Rewards[].type`: `MoneyVip {MoneyVip}`, `LinhDan {Id, Count}`, `Item {Item:{Id, Type, Count}}`, `Skill {Item:{...}}`, `RandomSkill {Tier, Type, Count}`, `RandomSpecialSkill {Tiers[], Type, TypeSkill?, Count}`.

## Giao diện (client)
- Bộ sprite "Nhiệm vụ" `Resources02/newui/task/new`: nền cuốn sách `mcs_RW_shubg`, băng tiêu đề `mcs_RW_biaoti_bg1`, gạch `mcs_RW_biaoti_line`, nút "Trở Về" `mcs_RW_fanhui_bg` + `mcs_RW_fanhui3`.
- Hàng thành tựu là dải lụa đổi màu theo trạng thái: xanh `mcs_RW_xuan_bl_bg` (đang làm), cam `mcs_RW_xuan_or_bg` + dấu 新 `mcs_RW_xuan_xin` (chờ nhận), lục `mcs_RW_zui_bg` + biểu tượng `mcs_RW_zui` "Hoàn thành" (đã nhận hết). Nút nhận và ô phần thưởng dùng lại của hàng mốc sự kiện.
- Dựng prefab: mở scene DongPhu → *Tools → Động phủ → Tạo prefab Thành tựu* → lưu scene. Sprite theo trạng thái chỉnh được trên component `DongPhuAchievementElement`.

## Kỹ thuật
- Server: `Game/DP/Achievements/*`, bảng `DongPhuAchievementProgresses` (CharId, Stat, Value, ClaimedTiers), hook trong `DongPhuState`/`DongPhuManager`; tag 4001 `GetDongPhuAchievements`, 4002 `ClaimDongPhuAchievement`, 4003 `DongPhuAchievementNotify`.
- Migration: `dotnet ef migrations add AddDongPhuAchievements`.

## Cân bằng hiện tại (30/09/2026, tiên ngọc x10)
Theo nhịp chơi đều ở Huyền Giai (tưới hộ 5/ngày, trồng/thu hoạch ~10/ngày, thăm 3–5/ngày); mốc 1→5 ≈ 3 ngày / 2 tuần / 1 tháng / 3 tháng / 6 tháng.

| Thành tựu | Mốc | Tiên ngọc | Thêm |
| --- | --- | --- | --- |
| Tưới Nước Hộ | 15 / 70 / 150 / 450 / 1000 | 1000 / 3000 / 6000 / 15000 / 30000 | M4 TT Địa Hạ–Trung, M5 TT Địa Thượng–Thiên Hạ |
| Được Tưới Hộ | 10 / 50 / 150 / 400 / 1000 | 1000 / 2500 / 5000 / 12000 / 25000 | như trên |
| Kết Giao Đạo Hữu (like) | 5 / 15 / 40 / 80 / 150 | 500 / 1500 / 3000 / 6000 / 12000 | M5 TT Địa Hạ–Trung |
| Danh Tiếng Động Phủ | 5 / 20 / 50 / 100 / 200 | 1000 / 3000 / 6000 / 12000 / 25000 | M4, M5 như Tưới hộ |
| Gieo Linh Chủng / Thu Hoạch | 20 / 100 / 300 / 1000 / 3000 | 1000 / 3000 / 6000 / 15000 / 30000 | M4, M5 như Tưới hộ |
| Động Phủ Thăng Giai | Hoàng → Thần | 3000 / 10000 / 20000 / 40000 / 80000 (~50% chi phí nâng) | M4, M5 như Tưới hộ |
| Thu Mua Hạt Giống | 50 / 300 / 1000 / 3000 / 8000 | 500 / 2000 / 5000 / 12000 / 25000 | M5 TT Địa Hạ–Trung |
| Du Ngoạn Động Phủ | 10 / 50 / 150 / 400 / 1000 | 500 / 1500 / 3000 / 8000 / 15000 | M5 TT Địa Hạ–Trung |

Tổng tiên ngọc nếu hoàn thành hết: ~506.000 (so với 305.000 để nâng Phàm → Thần). Phàm Giai chưa có ô đất/lượt tưới hộ nên đa số thành tựu cần Hoàng Giai.
