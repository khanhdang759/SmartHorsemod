# Smart Horse (SMAPI Mod) 🐎

Mod SMAPI viết bằng C# giúp nâng cấp con ngựa trong Stardew Valley:

-  **Chạy nhanh hơn** — chỉnh hệ số tốc độ trong config.
-  **Đi qua khe hẹp** — thu nhỏ hitbox ngựa để giảm kẹt ở lối đi 1 ô, góc cây, hàng rào.
-  **Tự nhặt đồ khi cưỡi ngựa** — quả mọng, nấm, đồ rơi tự động vào túi mà không cần xuống ngựa.
-  **Dễ điều khiển hơn** — va chạm mượt hơn, ít mắc kẹt vào vật cản.
-  **Gọi ngựa** — nhấn phím (mặc định `H`) để triệu hồi ngựa tới gần bạn, có thể bật/tắt.
-  **Cấu hình linh hoạt** — qua `config.json` hoặc [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) trong game.

## Yêu cầu

- Stardew Valley phiên bản **1.6.x**
- [SMAPI](https://smapi.io/) đã cài đặt
- .NET 6 SDK để build (nếu bạn tự build từ source)
- (Tùy chọn) [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) để chỉnh cấu hình qua giao diện trong game

## Cách build

1. Cài [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0).
2. Mở terminal tại thư mục project này (chứa `SmartHorse.csproj`).
3. Chạy:
   ```
   dotnet build
   ```
4. Gói `Pathoschild.Stardew.ModBuildConfig` sẽ tự động:
   - Tìm đường dẫn cài Stardew Valley trên máy bạn để tham chiếu `StardewValley.dll`, `StardewModdingAPI.dll`, `0Harmony.dll`, `xTile.dll`...
   - Sau khi build xong, tự copy toàn bộ mod (kèm `manifest.json`, `config.json`) vào thư mục `Mods/SmartHorse` trong thư mục cài Stardew Valley của bạn.
5. Nếu build không tự tìm thấy game, mở `SmartHorse.csproj` và bỏ comment dòng `<GamePath>` rồi điền đúng đường dẫn cài game của bạn.
6. Khởi động game qua SMAPI (`StardewModdingAPI.exe`) — mod sẽ tự nạp.

## Cấu hình (`config.json`)

| Khóa | Ý nghĩa | Mặc định |
|---|---|---|
| `SpeedMultiplier` | Hệ số nhân tốc độ ngựa | `1.5` |
| `MaxSpeed` | Tốc độ tối đa tuyệt đối | `14` |
| `EnableNarrowGapPassing` | Bật/tắt thu nhỏ hitbox để lách khe hẹp | `true` |
| `HitboxShrinkPixels` | Số pixel thu nhỏ mỗi bên hitbox (4-8 khuyến nghị) | `6` |
| `EnableAutoCollectWhileRiding` | Tự nhặt forage khi đang cưỡi ngựa | `true` |
| `AutoCollectRadius` | Bán kính quét nhặt đồ (theo ô) | `1` |
| `EnableCallHorse` | Bật/tắt gọi ngựa | `true` |
| `CallHorseKey` | Phím gọi ngựa | `"H"` |
| `CallHorseOutdoorsOnly` | Chỉ gọi được ngoài trời | `true` |
| `DebugLogging` | In log debug ra console SMAPI | `false` |

Nếu có Generic Mod Config Menu, bạn có thể chỉnh mọi thứ trực tiếp trong game (nhấn phím tắt/mở menu Options → Mod Options → Smart Horse) mà không cần sửa file tay.

## Cấu trúc project

```
SmartHorse/
├── manifest.json                     # Thông tin mod cho SMAPI
├── config.json                       # Cấu hình mặc định
├── SmartHorse.csproj                 # File build project
├── ModEntry.cs                       # Điểm khởi động, đăng ký patch + GMCM
├── ModConfig.cs                      # Định nghĩa các tùy chọn cấu hình
├── IGenericModConfigMenuApi.cs       # Interface tích hợp GMCM
├── Patches/
│   ├── HorseSpeedPatch.cs            # Patch Character.getMovementSpeed()
│   └── HorseCollisionPatch.cs        # Patch Character.GetBoundingBox() (khe hẹp)
└── Features/
    ├── AutoCollectFeature.cs         # Tự nhặt forage khi cưỡi ngựa
    └── CallHorseFeature.cs           # Gọi ngựa tới gần người chơi
```

## Ghi chú kỹ thuật quan trọng

Mã nguồn này patch vào các hàm nội bộ của game (`Character.getMovementSpeed()`,
`Character.GetBoundingBox()`) và dùng phương thức `Object.isForage()`. Đây là các API
ổn định qua nhiều bản 1.6.x, nhưng **tên hàm/field của game đôi khi thay đổi giữa các
bản cập nhật lớn**. Nếu sau khi build bạn gặp lỗi biên dịch do không tìm thấy
phương thức, cách xử lý nhanh:

1. Mở game bằng [dnSpy](https://github.com/dnSpy/dnSpy) hoặc dùng repo
   [decompiled 1.6](https://github.com/veywrn/StardewValley) để tra đúng tên hàm hiện tại
   trong class `StardewValley.Character` và `StardewValley.Object`.
2. Sửa lại tên hàm tương ứng trong `Patches/HorseSpeedPatch.cs`,
   `Patches/HorseCollisionPatch.cs` hoặc `Features/AutoCollectFeature.cs`.

Mình đã cố gắng dùng những API phổ biến nhất mà các mod ngựa nổi tiếng khác
(Faster Horse, Horse Master, Horse Overhaul...) cũng dựa vào, nên khả năng cần sửa là thấp,
nhưng không có gì đảm bảo 100% nếu bạn dùng bản game rất mới hoặc rất cũ.

## Gợi ý mở rộng

- Thêm hiệu ứng hạt (particle trail) khi ngựa chạy nhanh.
- Thêm animation "vẫy tay gọi ngựa" khi dùng tính năng gọi ngựa.
- Thêm hệ thống thể lực/stamina cho ngựa nếu muốn cân bằng lại tốc độ tăng thêm.