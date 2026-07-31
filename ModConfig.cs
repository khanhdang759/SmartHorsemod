using StardewModdingAPI;

namespace SmartHorse
{
    public class ModConfig
    {
        public float SpeedMultiplier { get; set; } = 1.5f;
        public int MaxSpeed { get; set; } = 14;

        public bool EnableNarrowGapPassing { get; set; } = true;
        public int HitboxShrinkPixels { get; set; } = 10;

        public bool EnableAutoCollectWhileRiding { get; set; } = true;
        public int AutoCollectRadius { get; set; } = 1;

        public bool EnableCallHorse { get; set; } = true;
        public SButton CallHorseKey { get; set; } = SButton.H;
        public bool CallHorseOutdoorsOnly { get; set; } = true;

        public bool DebugLogging { get; set; } = false;
    }
}
