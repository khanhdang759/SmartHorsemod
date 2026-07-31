using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace SmartHorse.Patches
{
    internal static class HorseSpeedPatch
    {
        private static ModConfig Config => ModEntry.Config;
        private static IMonitor Monitor => ModEntry.ModMonitor;

        public static void Apply(Harmony harmony)
        {
            MethodInfo targetMethod = AccessTools.Method(typeof(Farmer), "getMovementSpeed");

            if (targetMethod == null)
            {
                Monitor.Log("Không tìm thấy Farmer.getMovementSpeed() trong phiên bản game này.", LogLevel.Error);
                return;
            }

            HarmonyMethod postfix;
            if (targetMethod.ReturnType == typeof(int))
                postfix = new HarmonyMethod(typeof(HorseSpeedPatch), nameof(PostfixInt));
            else if (targetMethod.ReturnType == typeof(float))
                postfix = new HarmonyMethod(typeof(HorseSpeedPatch), nameof(PostfixFloat));
            else
            {
                Monitor.Log($"Farmer.getMovementSpeed() trả về kiểu không xác định ({targetMethod.ReturnType}).", LogLevel.Error);
                return;
            }

            harmony.Patch(original: targetMethod, postfix: postfix);
        }

        private static void PostfixInt(Farmer __instance, ref int __result)
        {
            try
            {
                if (!__instance.isRidingHorse())
                    return;

                int scaled = (int)Math.Round(__result * Config.SpeedMultiplier);
                if (Config.SpeedMultiplier > 1f && scaled <= __result)
                    scaled = __result + 1;
                if (scaled > Config.MaxSpeed)
                    scaled = Config.MaxSpeed;

                __result = scaled;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Lỗi trong HorseSpeedPatch (int): {ex}", LogLevel.Error);
            }
        }

        private static void PostfixFloat(Farmer __instance, ref float __result)
        {
            try
            {
                if (!__instance.isRidingHorse())
                    return;

                float scaled = __result * Config.SpeedMultiplier;
                if (Config.SpeedMultiplier > 1f && scaled <= __result)
                    scaled = __result + 1f;
                if (scaled > Config.MaxSpeed)
                    scaled = Config.MaxSpeed;

                __result = scaled;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Lỗi trong HorseSpeedPatch (float): {ex}", LogLevel.Error);
            }
        }
    }
}
