using HarmonyLib;
using StardewModdingAPI;
using SmartHorse.Features;
using SmartHorse.Patches;

namespace SmartHorse
{
    public class ModEntry : Mod
    {
        public static ModConfig Config { get; private set; }
        public static IMonitor ModMonitor { get; private set; }

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Config = helper.ReadConfig<ModConfig>();

            var harmony = new Harmony(ModManifest.UniqueID);
            HorseSpeedPatch.Apply(harmony);
            HorseCollisionPatch.Apply(harmony);

            new AutoCollectFeature(helper, Monitor).Register();
            new CallHorseFeature(helper, Monitor).Register();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            Monitor.Log("Smart Horse đã được nạp thành công! 🐎 (mod bởi KhanhDang)", LogLevel.Info);
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            RegisterGenericModConfigMenu();
        }

        private void RegisterGenericModConfigMenu()
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null)
            {
                Monitor.Log("Không tìm thấy Generic Mod Config Menu - bạn vẫn có thể chỉnh sửa trực tiếp file config.json.", LogLevel.Info);
                return;
            }

            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            gmcm.AddSectionTitle(ModManifest, () => "🐎 Tốc độ");

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.SpeedMultiplier,
                setValue: value => Config.SpeedMultiplier = value,
                name: () => "Hệ số tốc độ ngựa",
                tooltip: () => "1.0 = tốc độ gốc, 2.0 = nhanh gấp đôi.",
                min: 1f, max: 3f, interval: 0.1f
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MaxSpeed,
                setValue: value => Config.MaxSpeed = value,
                name: () => "Tốc độ tối đa",
                tooltip: () => "Giới hạn tuyệt đối để tránh ngựa di chuyển quá nhanh gây giật hình.",
                min: 5, max: 20, interval: 1
            );

            gmcm.AddSectionTitle(ModManifest, () => "🌳 Khe hẹp & va chạm");

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableNarrowGapPassing,
                setValue: value => Config.EnableNarrowGapPassing = value,
                name: () => "Cho phép đi qua khe hẹp",
                tooltip: () => "Thu nhỏ hitbox ngựa để dễ lách qua khe 1 ô, góc cây, hàng rào."
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.HitboxShrinkPixels,
                setValue: value => Config.HitboxShrinkPixels = value,
                name: () => "Độ thu nhỏ hitbox (pixel)",
                tooltip: () => "Giá trị càng lớn càng dễ lách qua khe hẹp bị chặn cả 2 bên. Khuyến nghị 8-16, thử tăng dần nếu vẫn kẹt.",
                min: 0, max: 24, interval: 1
            );

            gmcm.AddSectionTitle(ModManifest, () => "🍇 Tự động nhặt đồ");

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableAutoCollectWhileRiding,
                setValue: value => Config.EnableAutoCollectWhileRiding = value,
                name: () => "Tự nhặt đồ khi cưỡi ngựa",
                tooltip: () => "Tự động nhặt quả mọng, nấm, đồ rơi mà không cần xuống ngựa."
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.AutoCollectRadius,
                setValue: value => Config.AutoCollectRadius = value,
                name: () => "Bán kính nhặt đồ (ô)",
                tooltip: () => "0 = chỉ ô ngựa đang đứng, 1 = quét thêm các ô xung quanh.",
                min: 0, max: 3, interval: 1
            );

            gmcm.AddSectionTitle(ModManifest, () => "🎒 Gọi ngựa");

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableCallHorse,
                setValue: value => Config.EnableCallHorse = value,
                name: () => "Bật tính năng gọi ngựa",
                tooltip: () => "Nhấn phím tắt để triệu hồi ngựa đến gần bạn."
            );

            gmcm.AddKeybind(
                mod: ModManifest,
                getValue: () => Config.CallHorseKey,
                setValue: value => Config.CallHorseKey = value,
                name: () => "Phím gọi ngựa"
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.CallHorseOutdoorsOnly,
                setValue: value => Config.CallHorseOutdoorsOnly = value,
                name: () => "Chỉ gọi được khi ở ngoài trời",
                tooltip: () => "Tắt tùy chọn này để gọi ngựa được cả trong nhà/hầm mỏ."
            );

            gmcm.AddSectionTitle(ModManifest, () => "⚙️ Khác");

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.DebugLogging,
                setValue: value => Config.DebugLogging = value,
                name: () => "Ghi log debug",
                tooltip: () => "Bật để in thêm thông tin ra console SMAPI khi cần kiểm tra lỗi."
            );
        }
    }
}
