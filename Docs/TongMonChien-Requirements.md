# Yêu cầu tính năng Tông môn chiến

Trạng thái: **đã chốt toàn bộ quyết định mở (vòng hỏi đáp 29/09/2026, xem mục 7)**. Các giá trị ghi **[TẠM]** là giá trị chạy thử nằm trong hàm cấu hình riêng; công thức chính thức do chủ dự án bổ sung sau, không cần đổi schema/message.

## 1. Mục tiêu và thuật ngữ

- Tông môn chiến mở liên tục, mọi tông đều có thể tham gia trên cùng bản đồ; chưa có điều kiện thắng chung, kết thúc chiến hoặc reset lãnh thổ.
- Một tông có thể đồng thời sở hữu nhiều node thông thường.
- **Thành chính/tổng đàn**: node `Clan.NodeId`, duy nhất của mỗi tông, không thể bị chiếm. Người địch chỉ được đi xuyên qua thành, không dừng/chiếm (mục 3.1.4). Tông có `NodeId ≤ 0` (tông ẩn sau gộp dữ liệu) không có thành trên map nên không được tham chiến.
- **Node kiểm soát**: node có một tông sở hữu. Node không có chủ là node trung lập.
- **Đối thủ tại node**: người chơi **đang online và đang ở scene Tông môn chiến** hoặc con rối thuộc tông khác. Người/rối đang ở trận khác chưa sẵn sàng nhận trận mới. Người chơi offline/rời scene vẫn được lưu vị trí nhưng không chặn và không bị ghép trận.
- **Giao tranh**: một hay nhiều trận 1v1 tại cùng node. Icon giao tranh hiện khi ít nhất một trận tại node đang diễn ra.
- **HP chiến**: HP riêng trong tông môn chiến, không đồng nhất với `CharacterData.Stats.Hp` dùng ngoài chiến trường.
- Tài liệu dùng “tông môn” và “tộc trưởng/Leader” cho các thuật ngữ “tộc”, “tông”, “trưởng tộc” trong yêu cầu gốc.

## 2. Hiện trạng đã đối chiếu với code

| Thành phần | Hiện có | Khoảng trống cho tông môn chiến |
| --- | --- | --- |
| Client scene | `Assets/Scripts/AllMaps/TongMonChien.unity` giống `AllMaps.unity` từng byte | Chưa có UI chiến, màu chiếm node, icon giao tranh, icon thành theo chủ mới; chưa có trong `EditorBuildSettings.asset` |
| Client state | `Assets/Scripts/Network/ClientState/TongMonChien.cs` | `Handle` trống; chưa có lệnh vào/ra, di chuyển, nhận snapshot/delta, ghép trận |
| Server state | `backend/MTS/Network/ClientState/TongMonChien.cs` | `OnEnter`, `Handle`, callback tông môn và disconnect chưa xử lý |
| Dữ liệu sở hữu | `Clan.NodeId`, `ClanNode(NodeId, ClanId?)` | Chưa thấy dịch vụ đổi chủ node, trạng thái giao tranh, luật tái chiếm, đồng bộ map |
| Dữ liệu rối | `Puppet(ClanId, NodeId?, CharData)` | Chưa có cấp/cảnh giới/số lượng theo cấp tông, HP chiến, hành trình, điều rối, hồi HP |
| Di chuyển thường | `AllMaps` dùng BFS rồi cập nhật ngay node đích, trừ năng lượng và tăng tuổi | Không đáp ứng kiểm tra va chạm từng node hoặc di chuyển theo thời gian thực |
| PvP hiện tại | `CombatPvpManager` chuyển tiếp sự kiện trận giữa hai client | Chưa có chiến đấu với rối, hàng đợi phòng thủ, chuỗi trận tại node; kết quả trận hiện do client gửi lên nên cần quy tắc xác thực riêng cho chiếm đất |

Tận dụng graph node, hiển thị map, prefab nhân vật và giao diện combat hiện có nơi phù hợp. Trạng thái chiến và quyền sở hữu node phải do server quyết định, lưu bền và phát bản cập nhật cho client.

## 3. Quy tắc chức năng đã rõ

### 3.1. Bản đồ và chiếm node

1. Scene `TongMonChien` dùng cùng graph và ID node với `AllMaps`, nhưng dùng script điều khiển riêng để tách logic chiến khỏi logic bản đồ thường.
2. Mọi tông được tham gia, chưa đặt giới hạn cấp, số lần, phí hoặc khung giờ. Người chơi chỉ tham chiến khi thuộc tông và có ít nhất 1 HP chiến.
3. Người chơi có thể chọn node xa. Server tìm đường hợp lệ; khi qua node trung lập thì xử lý và chiếm từng node theo thứ tự. Không cho hủy lệnh di chuyển đang chạy. **Mỗi cạnh tốn cố định 5 giây** (`WarConfig.SecondsPerEdge`, dùng chung cho người và rối, server gửi cho client trong snapshot); client nội suy animation cho vừa thời điểm đến do server cấp. Không có ngoại lệ cưỡi kiếm: mọi đối tượng đi từng node. Chi phí năng lượng và tăng tuổi do di chuyển chiến bằng 0.
4. Chỉ được đi qua node của tông mình hoặc node trung lập vừa chiếm. Được tiến vào node địch để đánh chiếm, nhưng không đi xuyên qua node địch tới node sau khi node đó còn do địch giữ. Server kiểm tra lại quyền đi qua ở từng bước. Thành chính địch là ngoại lệ: chỉ đi xuyên trên route, không dừng, không chiếm, không bị người/rối tại đó chặn hay ghép trận.
5. Tại mỗi node đi qua, kiểm tra người chơi online và rối của tông khác. Nếu có đối thủ sẵn sàng thì ghép trận; nếu tất cả đối thủ đang đánh thì xếp chờ. Nếu còn đối thủ khác rảnh thì cho phép trận 1v1 song song tại cùng node.
6. Người/rối vào node trước được ưu tiên ghép trước. Người thắng giữ HP còn lại. Sau trận, server kiểm tra lại đối thủ còn ở node và ghép tiếp; không tự nhảy tới node đích xa khi chưa giải quyết node hiện tại.
7. Node thông thường không có đối thủ sau kiểm tra/chiến thắng được chiếm ngay, không cần giữ điểm trong một khoảng thời gian; có thể bị tái chiếm ngay. Mọi node không phải thành chính, **kể cả node loại `Stronghold` (S1..S29)**, đều chiếm được. Thành chính `Clan.NodeId` không thể bị chiếm hoặc đổi chủ.
8. Quyền sở hữu node thông thường là duy nhất tại một thời điểm. Không có thưởng khi chiếm hoặc phạt khi mất node trong phiên bản này.
9. Node có một hay nhiều trận đang diễn ra hiển thị icon giao tranh; khi trận cuối cùng kết thúc thì bỏ icon.
10. Node thuộc tông nào được tô theo màu đặc trưng trích từ ảnh icon tông đó. **Màu do client tính** từ icon đã tải: trung bình các pixel alpha > 0.5 có trọng số độ bão hoà, chuẩn hoá S ≥ 0.45 và V trong [0.55, 0.9]; không có ảnh, ảnh lỗi hoặc ảnh gần xám (S trung bình < 0.08) → palette 12 màu cố định theo `ClanId % 12`. Cùng ảnh cho cùng màu trên mọi client; server không lưu màu. Thành chính hiển thị icon của tông sở hữu cố định. Vị trí và HP của cả phe địch đều công khai. **Bản đồ chỉ hiển thị rối và người chơi đang online và đang trong Tông môn chiến** (mở scene chiến, đang đánh, hoặc vừa đánh xong đang quay lại scene); người chơi offline/rời scene bị ẩn nhưng server vẫn giữ vị trí và HP chiến của họ.

### 3.2. Kết quả giao tranh và vị trí

1. Người thua do hết HP được đưa về thành chính `Clan.NodeId` của mình. Chỉ cần hồi tới ít nhất 1 HP chiến là có thể tham chiến lại; chưa có thời gian chờ riêng.
2. Người bỏ chạy chịu kết quả thua và về node đồng minh gần nhất theo graph. Nếu node dự định về đã đổi chủ, server tìm lại node đồng minh gần nhất ở thời điểm áp kết quả. Nếu không còn node đồng minh hợp lệ ngoài thành chính thì về thành chính (thành chính luôn là node đồng minh). Người của tông được đứng/xuất phát và về tại thành chính của mình.
3. Trận người–người dùng PvP thời gian thực hiện có (`TongMonChienRealTime`); trận người–rối dùng PvE. Một đối tượng chỉ tham gia một trận tại một thời điểm. Thứ tự ghép tại mỗi node là FIFO theo thời điểm đến; cùng thời điểm thì người chơi trước rối, sau đó ID tăng dần (CharId cho người, PuppetId cho rối). Rối không bao giờ là bên tấn công: rối chỉ phòng thủ, trận người–rối luôn do người chơi đến node có rối.
4. Người thắng giữ HP chiến còn lại khi đánh đối thủ kế tiếp. Nếu một bên chết trước, bên đó thua. Nếu hai bên cùng chết mà không xác định được bên thua trước, kết quả trận là hòa; trận PvP hết giờ cũng tính hòa. **Hoà: cả hai bên giữ HP hiện tại và ở lại node** (bên có HP 0 vẫn bị đưa về thành chính như thua do hết HP). Owner chỉ được quyết định khi không còn trận nào tại node, theo luật **“ai còn đứng ở node”**: chỉ còn đúng một phe có người (đang ở scene, HP ≥ 1) hoặc rối tại node → phe đó chiếm/giữ node; còn ≥ 2 phe → tiếp tục ghép trận (FIFO); không còn ai → node thông thường chuyển trung lập.
5. Logout hoặc hủy trận khi đang đánh được tính thua ngay cho bên logout (người thua vì logout bị xử như bỏ chạy: về node đồng minh gần nhất); nếu không còn đối thủ cản, bên thắng có thể chiếm node thông thường. Khi logout giữa hành trình, người chơi trở về node an toàn ngay trước bước di chuyển đang thực hiện, không phải node bắt đầu cả lộ trình.
6. Tông bị giải tán được xử thua; server dừng hành trình và trận liên quan, chuyển toàn bộ node thường của tông về trung lập, xóa rối của tông và đưa người chơi về `AllMaps`. Đổi Leader không tự kết thúc tông chiến.
7. Đổi owner, vị trí người/rối và kết quả trận phải được cập nhật theo thứ tự nhất quán; client không tự tính hoặc tự gán owner.

### 3.3. HP chiến của người chơi

1. HP chiến tách biệt với HP ngoài chiến trường. Vào/ra chiến, đánh thường, dùng đan ở ngoài không được vô tình ghi đè HP chiến.
2. HP chiến khởi tạo từ một giá trị riêng, không lấy trực tiếp từ `Stats.Hp` hoặc `Stats.SinhMenh`. Tách các hàm `GetInitialHp`, `GetMaxHp`, `GetRegenPerMinute` trong `WarConfig`. **[TẠM]** Người và rối: HP khởi tạo = HP tối đa = 1000, hồi 10 HP/phút (1%/phút, đầy từ 0 sau 100 phút), cùng nhịp. HP hồi cả khi offline, tính lười theo thời gian server (`LastHpRecoveryAt`) mỗi lần đọc/ghi.
3. Chỉ có hai cách hồi HP chiến: hồi theo thời gian thực và dùng đan ngoài trận. Đan `EDanBuffType.Hp` cộng **giá trị đan × số lượng** vào HP chiến (clamp max), `HoiFullHp` hồi đầy HP chiến; hai loại này **chỉ tác động HP chiến, không đổi `Stats.Hp`**. Đan loại khác dùng trong scene chiến chạy đúng luật `UseDan` hiện có và không tăng HP chiến.
4. Server giữ HP chiến hiện tại, HP tối đa và mốc thời gian hồi; kiểm tra tồn kho và luật dùng đan hiện có, trừ đan và cập nhật HP trong cùng giao dịch.
5. Trong chiến đấu, HP chiến được đưa vào snapshot trận và cập nhật từ kết quả hợp lệ. Không dùng trực tiếp `CharacterData.Stats.Hp` của chế độ khác.

### 3.4. Con rối

1. Con rối thuộc một tông; có vị trí/hành trình trên bản đồ chiến, HP chiến riêng và trạng thái sẵn sàng/di chuyển/đang đánh/hồi phục.
2. Số rối theo cảnh giới Luyện Khí, Trúc Cơ, Kim Đan, Nguyên Anh, Hóa Thần và level/HP rối được tính trong các hàm cấu hình riêng để chủ dự án triển khai sau. **[TẠM]** Mỗi cấp tông được 1 rối Luyện Khí, tối đa 5 rối; nâng cấp tăng level rối 1 bậc, trần theo cấp tông; tạo/nâng cấp miễn phí (`GetCreateCost`/`GetUpgradeCost` trả 0). Rối được sinh là một `CharacterData` ảo từ `character-template.json` theo cảnh giới rối. Số rối chỉ tăng qua lệnh tạo của Leader nên không phát sinh qua restart.
3. Rối có Thọ nguyên vô tận; tạm chỉ trang bị `Cửu U Thứ`, chưa thêm kỹ năng, công pháp hoặc trang bị khác.
4. HP nền của rối phụ thuộc level rối và level tông, có thành phần ngẫu nhiên. Kết quả random phải lưu bền sau khi sinh; công thức chính thức do chủ dự án bổ sung sau.
5. HP rối hồi theo thời gian thực hoặc được cho dùng đan từ túi cá nhân của Leader (chỉ đan `Hp`/`HoiFullHp`). Rối chỉ được nhận lệnh điều đi tiếp khi HP đạt 100%; rối dưới 100% vẫn đứng tại node làm phòng thủ. Rối thua (HP 0) được đưa về thành chính của tông.
6. Chỉ Leader được tạo/nâng cấp/điều rối và dùng đan cho rối. **Rối chỉ được đi qua và dừng ở node tông mình đang sở hữu (kể cả thành chính), không chiếm node, không tự tấn công**; route chỉ đi qua node của tông. Rối đi từng node 5 giây/cạnh; server kiểm quyền và kiểm đường mỗi bước.
7. Một rối không thể có hai hành trình hoặc hai trận đồng thời. Nếu node kế tiếp trên route không còn thuộc tông khi rối chuẩn bị đến, rối dừng tại node hiện tại (node đã đến gần nhất). Nếu chính node rối đang đứng bị mất (chỉ xảy ra khi rối đã thua) thì rối về thành chính.

### 3.5. Rối: chi phí, HP, độ bền (30/09/2026 — thay các giá trị [TẠM] ở 3.4)

1. Số rối tối đa = level tông (không trần). Level tối đa mỗi rối = level tông × 3 (tông cấp 1 → rối lv3, cấp 2 → lv6…), trần 15 (Hóa Thần hậu kỳ).
2. Chi phí (trả bằng tài sản cá nhân của Tộc Chủ, giống mở giới hạn luyện hồn): tính theo điểm cống hiến = giá tiên ngọc × 2; trừ điểm cống hiến trước, thiếu bao nhiêu điểm thì bù tiên ngọc (1 tiên ngọc = 2 điểm, làm tròn lên); có popup xác nhận chi phí, giá đổi thì hỏi lại.
   - Tạo rối: 5.000 tiên ngọc (= 10.000 điểm cống hiến).
   - Nâng rối lên level L: 500 × L tiên ngọc (= 1.000 × L điểm).
3. HP rối = (level rối × 10)% HP chiến tối đa của người tạo/nâng tại thời điểm đó (lv1 = 10%, lv10 = 100%, lv15 = 150%). Nâng cấp tính lại HP tối đa và hồi đầy HP.
4. Độ bền 5/5: mỗi lần HP rối về 0 (thua do chết, kể cả hoà cùng chết) giảm 1; về 0 thì rối bị xoá, phải tạo rối mới. Nâng cấp hồi độ bền về 5/5. Độ bền hiển thị trong bảng Rối.

### 3.6. Thưởng tiên ngọc hằng ngày và mùa 2 tuần (30/09/2026 — thay thưởng tuần)

1. Tổng tiên ngọc/ngày của tông = số điểm (node thường) đã chiếm × 100 + số thành (Stronghold, không gồm thành chính) đã chiếm × 500.
2. Mỗi nhóm chức vụ nhận một phần tổng, **chia đều cho các thành viên đủ điều kiện trong nhóm** (cập nhật 30/09/2026): Tông chủ 50%, Phó tông chủ 20%, Trưởng lão 10%, Thành viên 20% (tổng 100%). Nhóm không có ai đủ điều kiện thì phần đó không phát.
3. Chốt 00:00 hằng ngày (UTC+7), chụp lãnh thổ lúc chốt, gửi qua thư. Điều kiện (01/10/2026): mọi thành viên còn trong tông lúc chốt đều nhận, không cần vào Tông môn chiến. Mỗi ngày chỉ chốt một lần (`WarSettlement`); lần chạy đầu sau khi triển khai chỉ đặt mốc.
4. Sự kiện kéo dài 2 tuần, lặp liên tục (mốc gốc 00:00 thứ Hai 28/09/2026 giờ VN, `WarConfig.SeasonAnchor`). Hết mùa: chốt thưởng ngày cuối trước, sau đó mọi trận kết thúc hoà, node thường về trung lập, người và rối về thành chính với HP đầy; rối giữ level và độ bền.
5. HUD hiển thị: Số điểm đã chiếm (x tiên ngọc/ngày), Số thành đã chiếm (x tiên ngọc/ngày), Tổng tiên ngọc/ngày, Thực nhận theo chức vụ (x tiên ngọc/ngày), thời gian tới lần phát thưởng và tới hết mùa.

## 4. Trách nhiệm client

| Khu vực | Yêu cầu |
| --- | --- |
| Vào/ra scene | Thêm `TongMonChien.unity` vào Build Settings, tạo luồng vào/ra và khôi phục state khi reconnect; tránh để logic `AllMaps.MapManager.Awake()` tự chuyển về state `AllMaps` trong scene chiến |
| Bản đồ | Dùng snapshot ban đầu và delta server để vẽ owner, màu node, icon tổng đàn/thành, trạng thái trận, vị trí người/rối, thời gian đến nơi |
| Di chuyển | Chỉ gửi node đích; vẽ lộ trình được server chấp thuận và cập nhật vị trí/chiếm từng node từ server; không hiện nút hủy/đổi đường; hiển thị lý do bị chặn |
| Giao tranh | Hiển thị trận khi server ghép; cập nhật icon node; hiển thị chuỗi trận sau khi thắng; xử lý trở về node/tổng đàn khi thua/bỏ chạy |
| Rối | Màn hình số lượng theo cảnh giới, HP/hồi phục, vị trí, hành trình; chỉ Leader thấy thao tác tạo/nâng cấp/điều rối/dùng đan; rối dưới 100% HP vẫn hiện là quân phòng thủ nhưng không được điều đi |
| HP chiến | Hiển thị HP chiến riêng trên UIHead (thay HP thường khi ở scene chiến, kèm tốc độ hồi, vd. `850/1000 (+10/phút)`), kết quả dùng đan; không ghi HP chiến vào HP thường |
| Hiển thị di chuyển | Người/rối đang đi tự quay mặt trái/phải theo hướng cạnh, chạy animation di chuyển, trên đầu hiện đếm ngược tới node kế tiếp (và tới đích nếu còn nhiều chặng) — áp dụng cho mọi đối tượng |
| Sau trận | Kết thúc trận quay về scene `TongMonChien` (không lùi về `AllMaps`) |
| Giao diện scene | Ẩn bảng NPC góc trái (`UINPCJiaoHu`) trong scene chiến, hiện lại khi rời scene |
| Đồng bộ | Khi mở scene/reconnect, nhận snapshot đầy đủ gồm vị trí và HP phe địch công khai; khi đang ở scene, áp delta có version/sequence, nếu thiếu delta thì tải lại snapshot |

## 5. Trách nhiệm server và lưu trữ

| Khu vực | Yêu cầu |
| --- | --- |
| Quyền tham chiến | Kiểm tra nhân vật thuộc tông, tông hợp lệ, vai trò, HP và trạng thái trận/hành trình; chống cùng tài khoản điều nhiều lệnh xung đột |
| Bản đồ chiến | Quản lý graph, quyền sở hữu node, tổng đàn, màu tông, trạng thái giao tranh; có khóa/giao dịch theo node để tránh hai phe cùng chiếm |
| Di chuyển | Giữ hành trình và thời điểm đến từng cạnh theo quãng đường/tốc độ `AllMaps.Player`; cùng một nguồn cấu hình tốc độ cho server và client, tránh lệch thời điểm; chi phí năng lượng/tuổi bằng 0; kiểm tra owner, đối thủ và rối sau mỗi bước; không nhận lệnh hủy/đổi đường; dừng nếu bước tiếp theo không còn hợp lệ |
| Ghép trận | Cấp `BattleId`, giữ trạng thái tham chiến cho mỗi đối tượng, hàng đợi FIFO tại node; ghép 1v1, cho phép nhiều trận song song nếu còn đối thủ rảnh; không ghép trùng một người/rối |
| Kết quả trận | Xác thực trận, phía thắng/thua, HP còn lại và nguyên nhân kết thúc; thay đổi vị trí, HP, hàng đợi và owner theo một luồng nhất quán. **PvP: server chờ báo cáo của cả hai client (timeout 30 giây). Khớp → áp dụng; mâu thuẫn → hoà; chỉ một bên báo vì bên kia mất kết nối/logout → bên mất kết nối thua. HP giữ lại = min(HP báo, HP trước trận).** PvE với rối: client người chơi báo, HP người = min(HP báo, HP trước trận), HP rối còn lại do client báo được clamp `[0, HP trước trận]` |
| HP chiến | Lưu riêng HP/mốc hồi cho người và rối, clamp vào `[0, max]`, ghi nhận tiêu thụ đan có giao dịch, tính hồi theo thời gian server kể cả lúc người chơi offline; hàm khởi tạo/tối đa/hồi dùng giá trị tạm riêng, công thức chính thức bổ sung sau |
| Rối | Đặt hàm riêng cho số lượng theo cấp tông/cảnh giới và công thức level/HP, sinh/lưu chỉ số random một lần, chỉ Leader ra lệnh và dùng đan cá nhân; xóa rối khi tông giải tán, kiểm lại quyền khi đổi Leader |
| Phát sự kiện | Snapshot vào scene và delta về di chuyển, owner, trận, HP, rối; công khai vị trí và HP của cả phe địch cho mọi người tham chiến |
| Vận hành | Khôi phục hành trình/trận/hồi HP sau restart, dọn trạng thái trận treo, log theo `BattleId/NodeId/CharId/PuppetId`, không để restart tạo thêm rối; chốt thưởng ngày 0h (UTC+7) và làm mới mùa 2 tuần chạy trong `TongMonChienService`, idempotent theo mốc (`WarSettlement`) |

### Mô hình dữ liệu (đã triển khai)

Không dùng `Progression.IdCurrentNode` hoặc `CharacterData.Stats.Hp` của thế giới thường làm trạng thái chiến. Bổ sung các bản ghi hoặc cấu trúc tương đương:

- `ClanNode` (bảng sẵn có, mở rộng): `NodeId`, `ClanId?` (null = trung lập), `Version`, `UpdatedAt`; thành chính không lưu ở đây mà luôn suy ra từ `Clan.NodeId`. `Clan.Puppets`/`Clan.ClanNodes` đã bỏ khỏi model `Clan` (cả client) để không serialize kèm tông.
- `WarParticipant`: `CharId`, `ClanId`, `NodeId`, `BattleHp`, `BattleHpMax`, `LastHpRecoveryAt`, `State`, `Route`, `NextArrivalAt`, `LastSafeNodeId`, `LastEnterAt` (lần vào scene gần nhất; không còn dùng làm điều kiện thưởng).
- `WarSettlement`: khoá (`Kind` = Daily/SeasonReset, `PeriodStart`), `SettledAt`, `ClanCount`, `MailCount`, `TotalMoneyVip` — mỗi ngày/mùa một bản ghi, chống xử lý lặp. `Puppet.Durability` lưu độ bền.
- `Puppet` (thay mô hình cũ `Puppet.CharData`): `Id`, `ClanId`, `Level` (cảnh giới suy ra từ level), `RandomSeed` sinh một lần, `NodeId`, HP chiến và mốc hồi, `State`, `Route`, `NextArrivalAt`. Không tạo `CharacterData` cho rối: khi ghép trận, server dựng snapshot từ nhân vật mẫu “Rối” (`TcpServer.Scarecrow`), đặt level/HP rối, thọ nguyên vô tận và chỉ trang bị `Cửu U Thứ`. Ngoại hình rối (`Gender`, skin, màu) do server random một lần khi tạo từ bộ skin tạo nhân vật và lưu bền (`Puppet.Gender`, `Puppet.AppearanceJson`), dùng cho cả marker trên bản đồ lẫn màn combat.
- `WarBattle`: `BattleId`, `NodeId`, hai bên tham chiến, trạng thái, thời điểm, kết quả, nguyên nhân kết thúc; phục vụ chống báo kết quả lặp.
- `WarConfig`: các hàm tách riêng cho số rối theo cấp tông/cảnh giới, level/HP rối; cấu hình thời gian đi, chi phí (nếu có), hồi HP và quy tắc trở về node; server phát phần hiển thị cần thiết cho client.

Các tên trên là gợi ý triển khai, không bắt buộc đặt tên y hệt. Có thể mở rộng `ClanNode` hiện có để lưu owner bền vững; không cần `WarSession` theo mùa. Không tạo bản ghi ownership thứ hai độc lập cho cùng node.

### Hợp đồng client–server (đã triển khai)

| Tag (giá trị) | Hướng | Nội dung |
| --- | --- | --- |
| `ChangeScene("TongMonChien")` | C→S | Server kiểm tra thành viên tông có thành (`NodeId > 0`) rồi chuyển state; về `AllMaps` bằng `ChangeScene("AllMaps")` |
| `WarGetSnapshot` 3901 / `WarSnapshot` 3902 | C↔S | Version, `SecondsPerEdge`, giờ server, tông (tên/icon/thành, `StrongholdCount`, `NodeCount`), node có chủ hoặc đang giao tranh, toàn bộ người/rối (vị trí, HP, nhịp hồi, hành trình), quyền Leader, giới hạn rối, tỉ lệ thưởng tuần (`StrongholdMoneyVip`, `NodeMoneyVip`) và `NextSettlementAt` |
| `WarMove` 3903 / `WarMoveResult` 3904 | C↔S | Node đích → route đã duyệt hoặc lý do từ chối; không có lệnh huỷ |
| `WarDeployPuppet` 3905 / `WarDeployPuppetResult` 3906 | C↔S | PuppetId + node đích (chỉ node của tông) |
| `WarUseHpPill` 3907 / `WarUseHpPillResult` 3908 | C↔S | PuppetId (0 = bản thân), DanId, Count → HP mới, tồn kho còn lại |
| `WarCreatePuppet` 3909 / 3910, `WarUpgradePuppet` 3911 / 3912 | C↔S | Leader tạo/nâng rối → `WarPuppetResult` |
| `WarDelta` 3913 | S→C | Version tăng đúng 1: tông, node (owner, đang giao tranh), đối tượng đổi, đối tượng bị xoá. Nhảy version → client tải lại snapshot |
| `WarBattleMatched` 3914 | S→C | BattleId, node, PvP/PvE, snapshot chiến của mình và đối thủ (HP/SinhMenh = HP chiến) |
| `WarReadyCombat` 3915, `Sync*` | C→S | Vào màn combat; PvP chuyển tiếp Sync* qua server như PvP luận bàn |
| `WarBattleReport` 3916 / `WarBattleEnded` 3917 | C↔S | Kết quả client báo (+ HP còn lại của rối); server trả kết quả đã xác thực, HP và node sau trận |
| `WarKicked` 3918 | S→C | Tông giải tán/rời tông: client rời scene chiến |

Kiểu dữ liệu nằm ở `MTS-Server/backend/MTS/Network/Models/TongMonChienMessages.cs` và bản sao y hệt thứ tự field ở `Assets/Scripts/Network/Models/TongMonChien/TongMonChienMessages.cs` (MemoryPack).

## 6. Luồng trạng thái cần kiểm thử

1. Vào chiến với ít nhất 1 HP chiến → nhận snapshot → xác định vị trí chiến độc lập với vị trí `AllMaps`; dưới 1 HP thì từ chối tham chiến cho đến khi hồi bằng thời gian thực hoặc đan ngoài trận.
2. Chọn node xa → server tìm route hợp lệ → xử lý từng node trung lập theo thứ tự → mỗi node trống lập tức thuộc tông người đến, không có thời gian giữ điểm và có thể tái chiếm ngay.
3. Node địch có người/rối rảnh → ghép 1v1 FIFO; khi đối thủ đều bận thì chờ, khi còn đối thủ rảnh thì mở trận song song; icon giao tranh hiện từ trận đầu đến khi trận cuối tại node kết thúc.
4. Thắng một trận → giữ HP còn lại → kiểm tra và ghép tiếp với đối thủ còn ở node; chỉ chiếm node thông thường sau khi không còn đối thủ cản. Thua do hết HP → về `Clan.NodeId`; bỏ chạy → về node đồng minh gần nhất ở thời điểm xử lý kết quả.
5. Hai bên cùng chết không xác định được ai thua trước hoặc PvP hết giờ → trận hòa; nếu node còn trận khác thì đợi tất cả kết thúc mới quyết định owner. Kiểm thử nhiều trận song song có kết quả khác nhau và quy tắc chuyển trung lập sau kết quả cuối.
6. Thành chính của mỗi tông luôn giữ owner và icon tông, không thể bị chiếm; người của tông được đứng/xuất phát/về ở thành mình. Người địch chỉ đi xuyên, không dừng, không bị chặn hoặc ghép trận tại thành.
7. Rối dưới 100% HP tiếp tục phòng thủ tại node nhưng không nhận lệnh di chuyển; đạt 100% mới được Leader điều đi. Chỉ Leader dùng đan trong túi cá nhân cho rối.
8. Logout giữa trận → bên logout thua ngay, bên còn lại có thể chiếm nếu hết đối thủ; logout giữa đường → trở về node an toàn ngay trước bước đang đi; tông giải tán → bên đó thua, node thường chuyển trung lập, rối bị xóa, người chơi rời chiến về `AllMaps`.
9. Reconnect, server restart, hai phe đến cùng node, node đổi chủ lúc đang di chuyển, hai trận kết thúc sát nhau: không nhân đôi rối/trận, không có hai owner và không mất HP/đan. Không có kết mùa hoặc reset lãnh thổ.
10. Chốt tuần: tông giữ 2 thành + 5 node → mỗi thành viên đủ điều kiện nhận 1 thư 1.250 tiên ngọc; thành viên chưa vào scene trong tuần hoặc đã rời tông không nhận; restart server quanh 0h thứ Hai không phát trùng.

## 7. Các quyết định đã chốt (29/09/2026)

| Chủ đề | Quyết định |
| --- | --- |
| Thời gian di chuyển | Cố định 5 giây/cạnh cho người và rối (`WarConfig.SecondsPerEdge`); không có ngoại lệ cưỡi kiếm; server gửi giá trị cho client |
| HP chiến | Người chơi: HP tối đa = `CharacterData.Stats.SinhMenh` (đồng bộ khi vào scene, xin snapshot, vào trận); HP hiện tại lưu riêng (`WarParticipant.Hp`), lần đầu đầy, hồi 1% tối đa/phút. Rối: (level × 10)% HP tối đa của người tạo/nâng; Độn tốc (TocDoBay) rối = (level × 10)% độn tốc người tạo/nâng (chốt lúc tạo/nâng, cột `Puppets.DonToc`; rối cũ chưa có giá trị dùng chỉ số mẫu cho tới lần nâng kế) |
| Tổng hợp kết quả tại node | “Ai còn đứng ở node” sau khi trận cuối kết thúc; hoà → hai bên giữ HP và ở lại; ≥ 2 phe → ghép tiếp; không còn ai → trung lập |
| Rối | Chỉ phòng thủ, không tự đánh, chỉ đi qua/dừng ở node của tông, không chiếm node; không có trận rối–rối |
| Tạo rối tạm | `CharacterData` ảo từ template, trang bị `Cửu U Thứ`; 1 rối/cấp tông, tối đa 5; miễn phí |
| Xác thực PvP | Hai client phải báo khớp (timeout 30 giây); mâu thuẫn → hoà; bên mất kết nối thua; HP giữ = min(HP báo, HP trước trận) |
| Màu tông | Client tính từ icon, fallback palette 12 màu theo `ClanId` |
| Quy tắc biên | Bỏ chạy mà không còn node đồng minh → về thành chính; đến cùng lúc → người trước rối, rồi ID tăng dần |
| Offline | Giữ vị trí chiến, không chặn; khi vào lại mà node đang đứng không thuộc tông → về node đồng minh gần nhất |
| Đan HP | `Hp`/`HoiFullHp` chỉ tác động HP chiến; đan khác theo `UseDan` hiện có; rối chỉ nhận đan HP |
| Node hợp lệ | Mọi node kể cả `Stronghold` đều chiếm được, trừ thành chính các tông; tông có `NodeId ≤ 0` không tham chiến |
| HP 0 khi vào scene | Vẫn được vào scene để xem và dùng đan; không được di chuyển, không tính là đối thủ cho tới khi ≥ 1 HP |
| Thời hạn trận | Không vào màn combat sau 30 giây → bên chưa vào thua; PvP chỉ một bên báo sau 30 giây → bên kia thua; trận quá 15 phút → hoà |
| Sau trận | Người thắng/hoà được tính là còn đứng ở node trong 90 giây chờ quay lại scene; chỉ bị ghép trận mới khi đã quay lại scene |
| Ghép trận | Chỉ ghép người chơi đã tải xong scene (đã xin snapshot); rối luôn sẵn sàng phòng thủ |
| Thưởng (30/09/2026, thay thưởng tuần) | Hằng ngày 0h: điểm 100/ngày, thành 500/ngày; mỗi nhóm chức vụ nhận % (Tông chủ 50, Phó tông chủ 20, Trưởng lão 10, Thành viên 20) chia đều cho toàn bộ thành viên trong nhóm (không cần tham gia), qua thư |
| Mùa (30/09/2026) | 2 tuần lặp liên tục; hết mùa làm mới đất, mọi người/rối về thành, HP đầy, rối giữ level/độ bền |
| Rối (30/09/2026) | Số rối = level tông; tạo 5.000 tiên ngọc, nâng 500 × level (điểm cống hiến ×2, trừ điểm trước bù tiên ngọc); HP = level×10% HP chiến người tạo/nâng; độ bền 5, nâng cấp hồi đầy |
| Hiển thị (30/09/2026) | HP chiến lên UIHead (bỏ thanh HP trên HUD); ẩn bảng NPC góc trái trong scene chiến; đếm ngược tới node kế cho mọi đối tượng đang đi; nhân vật quay mặt theo hướng đi; hết trận quay lại scene chiến |

Các công thức chính thức (HP, số rối, level rối) và chi phí tạo/nâng cấp rối do chủ dự án bổ sung sau trong `WarConfig`, không cần đổi schema/message.

## 8. Ghi chú triển khai

Xem `Docs/TongMonChien-Implementation.md` để biết danh sách file đã thêm/sửa ở client và server, các bước build, tạo migration và dựng scene trong Unity.
