# Tông môn chiến — ghi chú triển khai

Tài liệu đi kèm `TongMonChien-Requirements.md`. Mã đã viết nhưng **chưa được build/chạy thử** (môi trường làm việc không có dotnet/Unity và không truy cập được `Library/NetworkServer`). Cần build và kiểm thử theo mục 4 trước khi merge.

## 1. Kiến trúc

- **Server quyết định tất cả** (`MTS.Game.TMC.WarManager`, static, một khoá `Sync`): owner node, vị trí/hành trình, HP chiến, ghép trận, kết quả. Mọi thao tác chạy tuần tự dưới khoá và kết thúc bằng một `WarDelta` có version tăng 1, gửi cho mọi người đang ở scene.
- **Nhịp**: `TongMonChienService` gọi `WarManager.Tick()` mỗi 200 ms (đến node, hết hạn trận, hết thời gian chờ sau trận) và `Flush()` mỗi 1 giây (ghi DB các bản ghi đã đổi). Khi khởi động, `Initialize()` nạp lại toàn bộ và huỷ các trận đang chạy dở (hoà, `ServerRestart`).
- **Thứ tự khoá**: `WarManager.Sync` → `client.CharacterLock`/`client.StateLock`. Không bao giờ gọi `WarManager` khi đang giữ `CharacterLock`/`StateLock` (các chỗ đổi state chạy qua `Task.Run`).
- **Client** chỉ hiển thị: nội suy chuyển động trên cạnh theo `NextArrivalAt` và `SecondsPerEdge`, ngoại suy HP theo `RegenPerMinute`. Owner/vị trí luôn lấy từ server.

## 2. File thay đổi

### Server (`MTS-Server/backend/MTS`, nhánh `tong-mon-chien`)

| File | Nội dung |
| --- | --- |
| `Models/ClanNode.cs`, `Models/Puppet.cs` | Viết lại: owner node có version; rối không còn `CharData` |
| `Models/WarParticipant.cs`, `Models/WarBattle.cs` | Mới: trạng thái chiến của nhân vật, nhật ký trận |
| `Models/Clan.cs` | Bỏ `Puppets`, `ClanNodes` (không load/serialize kèm tông) |
| `Data/AppDbContext.cs` | DbSet + cấu hình 4 bảng, FK không navigation |
| `Game/TMC/WarConfig.cs` | **Toàn bộ số liệu cân bằng** (các hàm `[TẠM]` để thay công thức chính thức) |
| `Game/TMC/WarEnums.cs`, `WarUnit.cs` | Enum, lớp bọc người/rối, trạng thái node/tông/trận |
| `Game/TMC/WarManager*.cs` | Lõi: khởi tạo/lưu, phiên, di chuyển + luật node, trận, rối + đan |
| `Services/TongMonChienService.cs`, `Program.cs` | Background service |
| `Network/Models/TongMonChienMessages.cs`, `Network/MessageTag.cs` | Hợp đồng message (3901–3918) |
| `Network/ClientState/TongMonChien.cs`, `TongMonChienCombat.cs`, `MessageState.cs` | State scene chiến và state combat chiến |
| `Network/Core/SceneManager.cs` | `ChangeScene("TongMonChien")` |
| `Network/Core/SkillHandler.cs` | Trong scene chiến, `UseDanRequest` với đan Hp/HoiFullHp chỉ hồi HP chiến |
| `Game/Clan/ClanManager.cs` | Hook: tạo tông → `UpsertClan`; rời/kick → `OnMemberRemoved`; giải tán → `OnClanDeleted` |
| `Game/Combat/ETypeCombat.cs` | Thêm `TongMonChienPuppet` |

### Client (`MTS`, nhánh `tong-mon-chien`)

| File | Nội dung |
| --- | --- |
| `Network/Models/TongMonChien/TongMonChienMessages.cs` | Bản sao hợp đồng message |
| `Network/MessageTag.cs`, `Network/ClientState/MessageState.cs` | Tag 3901–3918, `TongMonChienCombat` |
| `Network/ClientState/TongMonChien.cs` | State scene chiến + lệnh gửi server |
| `Network/ClientState/TongMonChienCombat.cs` | State combat chiến (PvP relay Sync*, PvE với rối), không ghi `Stats.Hp` |
| `AllMaps/TongMonChien/TongMonChienController.cs` | Điều khiển scene: snapshot/delta, màu node, icon giao tranh, marker người/rối, click node, vào trận |
| `AllMaps/TongMonChien/TongMonChienHud.cs` | HUD dựng bằng code (HP chiến, dùng đan, bảng rối cho Leader, thoát) |
| `AllMaps/TongMonChien/ClanColorUtil.cs` | Màu tông từ icon + palette dự phòng |
| `AllMaps/TongMonChien/TongMonChienEntryButton.cs` | Nút vào chiến (gắn vào UI tông môn/AllMaps) |
| `AllMaps/MapManager.cs` | Cờ `isWarMode`: không chuyển state AllMaps, không kích hoạt sự kiện node/boss, click node → controller |
| `Combat/CombatManager.cs` | Kết quả trận với rối gửi kèm HP rối; màn kết quả “Bất phân thắng bại” cho hoà |
| `ETypeCombat.cs`, `EventList.cs` | `TongMonChienPuppet`, `RESULT_COMBAT_TONG_MON_CHIEN` |
| `Network/Models/Clans/ClanModels.cs` | Bỏ `Puppets`, `ClanNodes`, `Puppet`, `ClanNode` (khớp server) |
| `Editor/TongMonChienSceneSetup.cs` | Menu *Tools → Tông môn chiến → Thiết lập scene* |

## 3. Các bước cần làm trên máy có dotnet/Unity

1. **Server**: `dotnet build`, rồi `dotnet ef migrations add AddTongMonChien` và `dotnet ef database update`. Kiểm tra migration chỉ tạo `ClanNodes`, `Puppets`, `WarParticipants`, `WarBattles` (snapshot hiện tại chưa có `Puppet`/`ClanNode` cũ).
2. **Unity**: mở project, chạy *Tools → Tông môn chiến → Thiết lập scene* (bật `isWarMode`, gắn `TongMonChienController`, thêm scene vào Build Settings). Đọc log để tắt các hệ thống bản đồ thường còn trong scene (quest, NPC, sự kiện node) nếu gây nhiễu.
3. Gắn `TongMonChienEntryButton` vào một nút trong UI tông môn (hoặc gọi `TongMonChienEntryButton.Enter()`).
4. Tuỳ chọn: gán `puppetPrefab`, `fightingIconPrefab` cho controller; thay HUD bằng UI chính thức.

## 4. Kiểm thử gợi ý (2–3 tài khoản, 2 tông)

- Vào chiến lần đầu: HP 1000/1000 tại thành chính; vị trí độc lập với `IdCurrentNode`.
- Đi xa qua node trung lập: mỗi node đổi màu khi tới (5 giây/cạnh); không đi xuyên node địch; đi xuyên thành địch không dừng.
- A đứng ở node, B tới: ghép PvP; icon giao tranh hiện; thắng giữ HP; thua hết HP về thành; bỏ chạy về node đồng minh gần nhất.
- Hai client báo mâu thuẫn → hoà; một client tắt app giữa trận → bên đó thua sau khi server nhận disconnect.
- Leader tạo rối, điều rối (phải đầy HP), dùng đan cho rối; người địch đánh rối (PvE) — thắng thì rối về thành.
- Dùng đan Hp trong scene chiến: HP chiến tăng, `Stats.Hp` không đổi, đan bị trừ.
- Restart server khi đang đi/đang đánh: người đang đi dừng tại node, trận bị huỷ (hoà), không nhân đôi rối.
- Giải tán tông: node về trung lập, rối bị xoá, người trong scene bị đưa về AllMaps.

## 5. Giới hạn đã biết

- Chưa build/chạy thử; có thể còn lỗi biên dịch nhỏ.
- Chi phí tạo/nâng rối: nếu `WarConfig.GetCreatePuppetCost/GetUpgradePuppetCost` trả > 0 server sẽ từ chối — cần nối vào quỹ tông (`ClanManager`) khi chốt chi phí.
- Tồn kho đan được lưu theo nhịp lưu của `Client.SaveChanges` (gọi ngay sau khi dùng), HP chiến lưu theo `Flush` 1 giây: không cùng một transaction DB; cửa sổ lệch khi server sập là ~1 giây.
- PvP vẫn dựa trên báo cáo của hai client (đã yêu cầu khớp nhau); chưa mô phỏng lại sát thương ở server.
- HUD là bản tối thiểu dựng bằng code; marker rối mặc định dùng prefab người chơi.
