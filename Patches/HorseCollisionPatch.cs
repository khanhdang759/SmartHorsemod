using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace SmartHorse.Patches
{
    internal static class HorseCollisionPatch
    {
        private static ModConfig Config => ModEntry.Config;
        private static IMonitor Monitor => ModEntry.ModMonitor;

        public static void Apply(Harmony harmony)
        {
            var targetMethod = AccessTools.Method(typeof(Character), nameof(Character.GetBoundingBox));
            if (targetMethod == null)
            {
                Monitor.Log("Không tìm thấy Character.GetBoundingBox() để patch va chạm ngựa.", LogLevel.Error);
                return;
            }

            harmony.Patch(
                original: targetMethod,
                postfix: new HarmonyMethod(typeof(HorseCollisionPatch), nameof(Postfix))
            );
        }

        private static void Postfix(Character __instance, ref Rectangle __result)
        {
            try
            {
                if (!Config.EnableNarrowGapPassing)
                    return;

                if (__instance is not Horse)
                    return;

                int shrink = Math.Max(0, Config.HitboxShrinkPixels);
                if (shrink == 0)
                    return;

                __result.Inflate(-shrink, -shrink);

                if (__result.Width < 4) __result.Width = 4;
                if (__result.Height < 4) __result.Height = 4;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Lỗi trong HorseCollisionPatch: {ex}", LogLevel.Error);
            }
        }
    }
}
