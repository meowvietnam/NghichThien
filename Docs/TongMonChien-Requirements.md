# Yêu cầu tính năng Tông môn chiến

Trạng thái: **đã cập nhật theo 13 câu trả lời và các điểm chốt bổ sung của chủ dự án; còn các quyết định ở mục 7**. Các điểm ghi **[CẦN CHỐT]** không được xem là quy tắc đã duyệt. Không tự suy diễn công thức thời gian, HP hoặc hồi phục.

## 1. Mục tiêu và thuật ngữ

- Tông môn chiến mở liên tục, mọi tông đều có thể tham gia trên cùng bản đồ; chưa có điều kiện thắng chung, kết thúc chiến hoặc reset lãnh thổ.
- Một tông có thể đồng thời sở hữu nhiều node thông thường.
- **Thành chính/tổng đàn**: node `Clan.NodeId`, duy nhất của mỗi tông, không thể bị chiếm. Cần phân biệt hành vi “đi qua” với “đi vào” thành địch ở mục 7.
- **Node kiểm soát**: node có một tông sở hữu. Node không có chủ là node trung lập.
- **Đối thủ tại node**: người chơi online hoặc con rối thuộc tông khác. Người/rối đang ở trận khác chưa sẵn sàng nhận trận mới.
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
3. Người chơi có thể chọn node xa. Server tìm đường hợp lệ; khi qua node trung lập thì xử lý và chiếm từng node theo thứ tự. Không cho hủy lệnh di chuyển đang chạy. Thời gian đi mỗi cạnh dùng tốc độ di chuyển hiện có của `AllMaps.Player` theo quãng đường trên map; server cần tính thời điểm đến nhất quán với hiển thị client. Chi phí năng lượng và tăng tuổi do di chuyển chiến tạm bằng 0.
4. Chỉ được đi qua node của tông mình hoặc node trung lập vừa chiếm. Được tiến vào node địch để đánh chiếm, nhưng không đi xuyên qua node địch tới node sau khi node đó còn do địch giữ. Server kiểm tra lại quyền đi qua ở từng bước. Thành chính địch là ngoại lệ: chỉ đi xuyên trên route, không dừng, không chiếm, không bị người/rối tại đó chặn hay ghép trận.
5. Tại mỗi node đi qua, kiểm tra người chơi online và rối của tông khác. Nếu có đối thủ sẵn sàng thì ghép trận; nếu tất cả đối thủ đang đánh thì xếp chờ. Nếu còn đối thủ khác rảnh thì cho phép trận 1v1 song song tại cùng node.
6. Người/rối vào node trước được ưu tiên ghép trước. Người thắng giữ HP còn lại. Sau trận, server kiểm tra lại đối thủ còn ở node và ghép tiếp; không tự nhảy tới node đích xa khi chưa giải quyết node hiện tại.
7. Node thông thường không có đối thủ sau kiểm tra/chiến thắng được chiếm ngay, không cần giữ điểm trong một khoảng thời gian; có thể bị tái chiếm ngay. Thành chính `Clan.NodeId` không thể bị chiếm hoặc đổi chủ.
8. Quyền sở hữu node thông thường là duy nhất tại một thời điểm. Không có thưởng khi chiếm hoặc phạt khi mất node trong phiên bản này.
9. Node có một hay nhiều trận đang diễn ra hiển thị icon giao tranh; khi trận cuối cùng kết thúc thì bỏ icon.
10. Node thuộc tông nào được tô theo màu đặc trưng trích từ ảnh icon tông đó. Thành chính hiển thị icon của tông sở hữu cố định. Vị trí và HP của cả phe địch đều công khai.

### 3.2. Kết quả giao tranh và vị trí

1. Người thua do hết HP được đưa về thành chính `Clan.NodeId` của mình. Chỉ cần hồi tới ít nhất 1 HP chiến là có thể tham chiến lại; chưa có thời gian chờ riêng.
2. Người bỏ chạy chịu kết quả thua và về node đồng minh gần nhất theo graph. Nếu node dự định về đã đổi chủ, server tìm lại node đồng minh gần nhất ở thời điểm áp kết quả. Nếu không còn node đồng minh hợp lệ, dùng thành chính của mình **[đề xuất, cần xác nhận]**. Người của tông được đứng/xuất phát và về tại thành chính của mình.
3. Trận người–người dùng PvP thời gian thực hiện có (`TongMonChienRealTime`); trận người–rối dùng PvE. Một đối tượng chỉ tham gia một trận tại một thời điểm. Thứ tự ghép tại mỗi node là FIFO theo thời điểm đến, sau đó dùng thứ tự ổn định để xử lý trường hợp đến cùng thời điểm **[quy tắc phụ cần chốt]**.
4. Người thắng giữ HP chiến còn lại khi đánh đối thủ kế tiếp. Nếu một bên chết trước, bên đó thua. Nếu hai bên cùng chết mà không xác định được bên thua trước, kết quả trận là hòa; đợi tất cả trận đang diễn ra tại node kết thúc rồi mới quyết định owner. Trận PvP hết giờ cũng tính hòa. Node thông thường trở thành trung lập khi kết quả cuối cùng tại node vẫn là hòa **[CẦN CHỐT cách tổng hợp nhiều kết quả trận]**.
5. Logout hoặc hủy trận khi đang đánh được tính thua ngay cho bên logout; nếu không còn đối thủ cản, bên thắng có thể chiếm node thông thường. Khi logout giữa hành trình, người chơi trở về node an toàn ngay trước bước di chuyển đang thực hiện, không phải node bắt đầu cả lộ trình.
6. Tông bị giải tán được xử thua; server dừng hành trình và trận liên quan, chuyển toàn bộ node thường của tông về trung lập, xóa rối của tông và đưa người chơi về `AllMaps`. Đổi Leader không tự kết thúc tông chiến.
7. Đổi owner, vị trí người/rối và kết quả trận phải được cập nhật theo thứ tự nhất quán; client không tự tính hoặc tự gán owner.

### 3.3. HP chiến của người chơi

1. HP chiến tách biệt với HP ngoài chiến trường. Vào/ra chiến, đánh thường, dùng đan ở ngoài không được vô tình ghi đè HP chiến.
2. HP chiến khởi tạo từ một giá trị riêng, không lấy trực tiếp từ `Stats.Hp` hoặc `Stats.SinhMenh`. Tách các hàm tính HP khởi tạo, HP tối đa và tốc độ hồi; hiện dùng giá trị tạm riêng để chạy thử, công thức chính thức sẽ triển khai sau **[CẦN CHỐT giá trị tạm và công thức]**. HP có thể hồi khi người chơi offline, dựa trên thời gian server.
3. Chỉ có hai cách hồi HP chiến: hồi theo thời gian thực và dùng đan ngoài trận. Cho phép dùng mọi loại đan ngoài trận theo luật dùng đan hiện có; đan không phải loại hồi HP không được tự tăng HP chiến.
4. Server giữ HP chiến hiện tại, HP tối đa và mốc thời gian hồi; kiểm tra tồn kho và luật dùng đan hiện có, trừ đan và cập nhật HP trong cùng giao dịch.
5. Trong chiến đấu, HP chiến được đưa vào snapshot trận và cập nhật từ kết quả hợp lệ. Không dùng trực tiếp `CharacterData.Stats.Hp` của chế độ khác.

### 3.4. Con rối

1. Con rối thuộc một tông; có vị trí/hành trình trên bản đồ chiến, HP chiến riêng và trạng thái sẵn sàng/di chuyển/đang đánh/hồi phục.
2. Số rối theo cảnh giới Luyện Khí, Trúc Cơ, Kim Đan, Nguyên Anh, Hóa Thần và level/HP rối được tính trong các hàm cấu hình riêng để chủ dự án triển khai sau. Chưa chốt bảng số lượng hoặc công thức; hàm phải có giao diện rõ, có giá trị tạm để tính năng chạy, và không phát sinh số rối bất định qua restart.
3. Rối có Thọ nguyên vô tận; tạm chỉ trang bị `Cửu U Thứ`, chưa thêm kỹ năng, công pháp hoặc trang bị khác.
4. HP nền của rối phụ thuộc level rối và level tông, có thành phần ngẫu nhiên. Kết quả random phải lưu bền sau khi sinh; công thức chính thức do chủ dự án bổ sung sau.
5. HP rối hồi theo thời gian thực hoặc được cho dùng đan từ túi cá nhân của Leader. Rối chỉ được nhận lệnh điều đi tiếp khi HP đạt 100%; rối dưới 100% vẫn đứng tại node làm phòng thủ.
6. Chỉ Leader được tạo/nâng cấp/điều rối và dùng đan cho rối. Rối được điều tới node của tông mình, đi từng node theo cùng nguyên tắc thời gian với người chơi; server kiểm quyền và kiểm đường mỗi bước.
7. Một rối không thể có hai hành trình hoặc hai trận đồng thời. Khi tông đổi chủ node giữa lúc rối đang đi, server phải tính lại tính hợp lệ và xử lý theo quy tắc **[CẦN CHỐT]**.

## 4. Trách nhiệm client

| Khu vực | Yêu cầu |
| --- | --- |
| Vào/ra scene | Thêm `TongMonChien.unity` vào Build Settings, tạo luồng vào/ra và khôi phục state khi reconnect; tránh để logic `AllMaps.MapManager.Awake()` tự chuyển về state `AllMaps` trong scene chiến |
| Bản đồ | Dùng snapshot ban đầu và delta server để vẽ owner, màu node, icon tổng đàn/thành, trạng thái trận, vị trí người/rối, thời gian đến nơi |
| Di chuyển | Chỉ gửi node đích; vẽ lộ trình được server chấp thuận và cập nhật vị trí/chiếm từng node từ server; không hiện nút hủy/đổi đường; hiển thị lý do bị chặn |
| Giao tranh | Hiển thị trận khi server ghép; cập nhật icon node; hiển thị chuỗi trận sau khi thắng; xử lý trở về node/tổng đàn khi thua/bỏ chạy |
| Rối | Màn hình số lượng theo cảnh giới, HP/hồi phục, vị trí, hành trình; chỉ Leader thấy thao tác tạo/nâng cấp/điều rối/dùng đan; rối dưới 100% HP vẫn hiện là quân phòng thủ nhưng không được điều đi |
| HP chiến | Hiển thị HP chiến riêng, tốc độ/mốc hồi và kết quả dùng đan; không ghi HP chiến vào HP thường |
| Đồng bộ | Khi mở scene/reconnect, nhận snapshot đầy đủ gồm vị trí và HP phe địch công khai; khi đang ở scene, áp delta có version/sequence, nếu thiếu delta thì tải lại snapshot |

## 5. Trách nhiệm server và lưu trữ

| Khu vực | Yêu cầu |
| --- | --- |
| Quyền tham chiến | Kiểm tra nhân vật thuộc tông, tông hợp lệ, vai trò, HP và trạng thái trận/hành trình; chống cùng tài khoản điều nhiều lệnh xung đột |
| Bản đồ chiến | Quản lý graph, quyền sở hữu node, tổng đàn, màu tông, trạng thái giao tranh; có khóa/giao dịch theo node để tránh hai phe cùng chiếm |
| Di chuyển | Giữ hành trình và thời điểm đến từng cạnh theo quãng đường/tốc độ `AllMaps.Player`; cùng một nguồn cấu hình tốc độ cho server và client, tránh lệch thời điểm; chi phí năng lượng/tuổi bằng 0; kiểm tra owner, đối thủ và rối sau mỗi bước; không nhận lệnh hủy/đổi đường; dừng nếu bước tiếp theo không còn hợp lệ |
| Ghép trận | Cấp `BattleId`, giữ trạng thái tham chiến cho mỗi đối tượng, hàng đợi FIFO tại node; ghép 1v1, cho phép nhiều trận song song nếu còn đối thủ rảnh; không ghép trùng một người/rối |
| Kết quả trận | Xác thực trận, phía thắng/thua, HP còn lại và nguyên nhân kết thúc; thay đổi vị trí, HP, hàng đợi và owner theo một luồng nhất quán. PvP hiện tại tin dữ liệu kết quả client nên cần gia cố trước khi kết quả ảnh hưởng đất chiếm |
| HP chiến | Lưu riêng HP/mốc hồi cho người và rối, clamp vào `[0, max]`, ghi nhận tiêu thụ đan có giao dịch, tính hồi theo thời gian server kể cả lúc người chơi offline; hàm khởi tạo/tối đa/hồi dùng giá trị tạm riêng, công thức chính thức bổ sung sau |
| Rối | Đặt hàm riêng cho số lượng theo cấp tông/cảnh giới và công thức level/HP, sinh/lưu chỉ số random một lần, chỉ Leader ra lệnh và dùng đan cá nhân; xóa rối khi tông giải tán, kiểm lại quyền khi đổi Leader |
| Phát sự kiện | Snapshot vào scene và delta về di chuyển, owner, trận, HP, rối; công khai vị trí và HP của cả phe địch cho mọi người tham chiến |
| Vận hành | Khôi phục hành trình/trận/hồi HP sau restart, dọn trạng thái trận treo, log theo `BattleId/NodeId/CharId/PuppetId`, không để restart tạo thêm rối; chiến luôn mở, không có tác vụ kết mùa/reset lãnh thổ |

### Mô hình dữ liệu đề xuất

Không dùng `Progression.IdCurrentNode` hoặc `CharacterData.Stats.Hp` của thế giới thường làm trạng thái chiến. Bổ sung các bản ghi hoặc cấu trúc tương đương:

- `WarNode`: `NodeId`, `OwnerClanId?`, `IsStronghold`, version; thành chính gắn cố định với `Clan.NodeId`, không cho đổi owner, node thông thường chỉ có tối đa một owner.
- `WarParticipant`: `CharId`, `ClanId`, `NodeId`, `BattleHp`, `BattleHpMax`, `LastHpRecoveryAt`, `State`, `Route`, `NextArrivalAt`, `LastSafeNodeId`.
- `WarPuppet`: ID riêng, `ClanId`, cảnh giới/level, chỉ số đã sinh, `NodeId`, HP chiến và mốc hồi, `State`, `Route`, `NextArrivalAt`; liên kết nhân vật ảo nếu tiếp tục dùng `Puppet.CharData`.
- `WarBattle`: `BattleId`, `NodeId`, hai bên tham chiến, trạng thái, thời điểm, kết quả, nguyên nhân kết thúc; phục vụ chống báo kết quả lặp.
- `WarConfig`: các hàm tách riêng cho số rối theo cấp tông/cảnh giới, level/HP rối; cấu hình thời gian đi, chi phí (nếu có), hồi HP và quy tắc trở về node; server phát phần hiển thị cần thiết cho client.

Các tên trên là gợi ý triển khai, không bắt buộc đặt tên y hệt. Có thể mở rộng `ClanNode` hiện có để lưu owner bền vững; không cần `WarSession` theo mùa. Không tạo bản ghi ownership thứ hai độc lập cho cùng node.

### Hợp đồng client–server đề xuất

| Lệnh/sự kiện | Nội dung tối thiểu |
| --- | --- |
| `EnterWar` / `ExitWar` | Xác thực quyền, đổi state, trả snapshot và vị trí hợp lệ |
| `GetWarSnapshot` | Version, node owner/màu/icon, toàn bộ vị trí và HP người/rối phe địch, trận đang diễn ra, HP và hành trình của mình |
| `MoveWarCharacter` | Node đích; server trả route đã duyệt, sự kiện đến/chiếm từng node, lý do từ chối/dừng; không có lệnh hủy |
| `DeployWarPuppet` | ID rối, node đích thuộc tông; server kiểm quyền Leader, HP 100%, route và thời gian; không có lệnh hủy |
| `UseWarHpPill` | Đối tượng nhận, đan và số lượng; trả HP mới, tồn kho mới, mốc hồi |
| `WarBattleMatched` / `WarBattleChanged` / `WarBattleEnded` | BattleId, node, loại đối thủ, snapshot chiến, kết quả và vị trí sau trận |
| `WarNodeChanged` / `WarPresenceChanged` | Delta có version; owner, tình trạng giao tranh, người/rối tới/rời node |

Tên message chỉ là đề xuất. Các kiểu truyền qua mạng cần thống nhất giữa hai assembly, version schema và kiểm tra kích thước frame; dùng `MemoryPack` hoặc serializer hiện có nhất quán cho cả tính size và ghi payload.

## 6. Luồng trạng thái cần kiểm thử

1. Vào chiến với ít nhất 1 HP chiến → nhận snapshot → xác định vị trí chiến độc lập với vị trí `AllMaps`; dưới 1 HP thì từ chối tham chiến cho đến khi hồi bằng thời gian thực hoặc đan ngoài trận.
2. Chọn node xa → server tìm route hợp lệ → xử lý từng node trung lập theo thứ tự → mỗi node trống lập tức thuộc tông người đến, không có thời gian giữ điểm và có thể tái chiếm ngay.
3. Node địch có người/rối rảnh → ghép 1v1 FIFO; khi đối thủ đều bận thì chờ, khi còn đối thủ rảnh thì mở trận song song; icon giao tranh hiện từ trận đầu đến khi trận cuối tại node kết thúc.
4. Thắng một trận → giữ HP còn lại → kiểm tra và ghép tiếp với đối thủ còn ở node; chỉ chiếm node thông thường sau khi không còn đối thủ cản. Thua do hết HP → về `Clan.NodeId`; bỏ chạy → về node đồng minh gần nhất ở thời điểm xử lý kết quả.
5. Hai bên cùng chết không xác định được ai thua trước hoặc PvP hết giờ → trận hòa; nếu node còn trận khác thì đợi tất cả kết thúc mới quyết định owner. Kiểm thử nhiều trận song song có kết quả khác nhau và quy tắc chuyển trung lập sau kết quả cuối.
6. Thành chính của mỗi tông luôn giữ owner và icon tông, không thể bị chiếm; người của tông được đứng/xuất phát/về ở thành mình. Người địch chỉ đi xuyên, không dừng, không bị chặn hoặc ghép trận tại thành.
7. Rối dưới 100% HP tiếp tục phòng thủ tại node nhưng không nhận lệnh di chuyển; đạt 100% mới được Leader điều đi. Chỉ Leader dùng đan trong túi cá nhân cho rối.
8. Logout giữa trận → bên logout thua ngay, bên còn lại có thể chiếm nếu hết đối thủ; logout giữa đường → trở về node an toàn ngay trước bước đang đi; tông giải tán → bên đó thua, node thường chuyển trung lập, rối bị xóa, người chơi rời chiến về `AllMaps`.
9. Reconnect, server restart, hai phe đến cùng node, node đổi chủ lúc đang di chuyển, hai trận kết thúc sát nhau: không nhân đôi rối/trận, không có hai owner và không mất HP/đan. Không có kết mùa, reset hoặc thưởng/phạt chiếm node.

## 7. Các quyết định còn cần chốt

| Ưu tiên | Câu hỏi | Tác động |
| --- | --- | --- |
| Bắt buộc | Tốc độ `AllMaps.Player` là tốc độ cơ sở chung cho người và rối trong chiến; có dùng ngoại lệ cưỡi kiếm (hiện bỏ qua node giữa trên client) hay bắt buộc mọi đối tượng đi từng node? | Lịch trình server và hiển thị client |
| Bắt buộc | Giá trị tạm riêng của HP khởi tạo/tối đa/tốc độ hồi là bao nhiêu cho người và rối? Công thức chính thức sẽ bổ sung sau; rối dùng cùng nhịp hồi hay nhịp khác? | Lưu HP, tính hồi offline và kiểm thử định lượng |
| Bắt buộc | Khi tất cả trận tại node đã kết thúc, nếu có trận hòa lẫn trận thắng/thua thì tổng hợp kết quả và quyết định owner thế nào? Hết giờ hòa có giữ HP hiện tại hay xử lý như bỏ chạy? | Quy tắc hòa, concurrency và chống race condition |
| Cần chốt trước UI | Màu trích từ ảnh tông được tính/lưu khi tạo hoặc đổi icon theo thuật toán nào; ảnh quá nhạt/trong suốt xử lý ra sao? | Đồng nhất màu giữa các client và độ tương phản |
| Có thể để cấu hình | Nếu không còn node đồng minh nào ngoài thành chính sau khi bỏ chạy, có dùng thành chính làm điểm về không? Hai đối tượng đến cùng thời điểm thì tie-break FIFO bằng ID tăng dần được không? | Quy tắc biên và kiểm thử ổn định |

Số rối và công thức level/HP rối là phần chủ dự án chủ động triển khai sau: để sẵn các hàm cấu hình có giá trị tạm xác định, không khóa bảng chính thức trong tài liệu này. Khi các quyết định bắt buộc ở trên được trả lời, thay các dấu **[CẦN CHỐT]** bằng luật cụ thể rồi mới khóa schema/message và tiêu chí nghiệm thu định lượng.
