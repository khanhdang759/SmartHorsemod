using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SmartHorse.Features
{
    internal class AutoCollectFeature
    {
        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        private ModConfig Config => ModEntry.Config;

        private const int ScanIntervalTicks = 10;

        public AutoCollectFeature(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            _monitor = monitor;
        }

        public void Register()
        {
            _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Config.EnableAutoCollectWhileRiding)
                return;

            if (!e.IsMultipleOf(ScanIntervalTicks))
                return;

            Farmer player = Game1.player;
            if (player == null || !player.isRidingHorse())
                return;

            GameLocation location = player.currentLocation;
            if (location == null)
                return;

            Vector2 centerTile = player.Tile;
            int radius = Math.Max(0, Config.AutoCollectRadius);

            var tilesToCheck = new List<Vector2>();
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    tilesToCheck.Add(new Vector2(centerTile.X + dx, centerTile.Y + dy));
                }
            }

            foreach (Vector2 tile in tilesToCheck)
            {
                TryCollectForageAt(location, player, tile);
            }
        }

        private void TryCollectForageAt(GameLocation location, Farmer player, Vector2 tile)
        {
            try
            {
                if (!location.Objects.TryGetValue(tile, out StardewValley.Object obj) || obj == null)
                    return;

                bool isForageItem = obj.isForage();
                if (!isForageItem)
                    return;

                Item item = obj.getOne();
                bool added = player.addItemToInventoryBool(item);

                if (added)
                {
                    location.Objects.Remove(tile);
                    location.playSound("harvest");

                    if (Config.DebugLogging)
                        _monitor.Log($"[SmartHorse] Tự động nhặt '{item.DisplayName}' tại {tile}.", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Lỗi khi tự động nhặt đồ lúc cưỡi ngựa: {ex}", LogLevel.Error);
            }
        }
    }
}
