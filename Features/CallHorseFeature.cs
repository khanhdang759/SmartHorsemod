using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace SmartHorse.Features
{
    internal class CallHorseFeature
    {
        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        private ModConfig Config => ModEntry.Config;

        public CallHorseFeature(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            _monitor = monitor;
        }

        public void Register()
        {
            _helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Config.EnableCallHorse)
                return;

            if (!Context.IsWorldReady || !Context.IsPlayerFree)
                return;

            if (e.Button != Config.CallHorseKey)
                return;

            CallHorseToPlayer(Game1.player);
        }

        private void CallHorseToPlayer(Farmer player)
        {
            try
            {
                if (Config.CallHorseOutdoorsOnly && !player.currentLocation.IsOutdoors)
                {
                    Game1.showRedMessage("Không thể gọi ngựa khi đang ở trong nhà!");
                    return;
                }

                Horse horse = FindOwnedHorse(player);
                if (horse == null)
                {
                    Game1.showRedMessage("Bạn chưa có ngựa nào, hoặc ngựa đang ở trong chuồng chưa đặt tên.");
                    return;
                }

                if (player.mount == horse)
                    return;

                Vector2 targetTile = FindNearbyOpenTile(player);

                if (horse.currentLocation != player.currentLocation)
                {
                    horse.currentLocation?.characters.Remove(horse);
                    player.currentLocation.addCharacter(horse);
                    horse.currentLocation = player.currentLocation;
                }

                horse.setTileLocation(targetTile);
                horse.Halt();
                horse.faceDirection(2);

                player.currentLocation.playSound("dwop");

                if (Config.DebugLogging)
                    _monitor.Log($"[SmartHorse] Đã gọi ngựa '{horse.Name}' tới {targetTile}.", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Lỗi khi gọi ngựa: {ex}", LogLevel.Error);
            }
        }

        private Horse FindOwnedHorse(Farmer player)
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (NPC npc in location.characters)
                {
                    if (npc is Horse horse && horse.ownerId.Value == player.UniqueMultiplayerID)
                        return horse;
                }
            }
            return null;
        }

        private Vector2 FindNearbyOpenTile(Farmer player)
        {
            Vector2 playerTile = player.Tile;
            GameLocation location = player.currentLocation;

            Vector2[] candidates =
            {
                new Vector2(playerTile.X, playerTile.Y + 1),
                new Vector2(playerTile.X + 1, playerTile.Y),
                new Vector2(playerTile.X - 1, playerTile.Y),
                new Vector2(playerTile.X, playerTile.Y - 1),
                new Vector2(playerTile.X + 1, playerTile.Y + 1),
                new Vector2(playerTile.X - 1, playerTile.Y + 1),
            };

            foreach (Vector2 tile in candidates)
            {
                if (location.isTilePassable(new xTile.Dimensions.Location((int)tile.X, (int)tile.Y), Game1.viewport))
                    return tile;
            }

            return candidates[0];
        }
    }
}
