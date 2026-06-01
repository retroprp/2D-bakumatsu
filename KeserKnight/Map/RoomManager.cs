using System;
using System.Collections.Generic;
using System.Drawing;
using KeserKnight.Entity;

namespace KeserKnight.Map
{
    public class RoomManager
    {
        public int CurrentRoom { get; private set; } = 1;
        private readonly int mapWidth;
        private readonly int mapHeight;

        public RoomManager(int targetWidth, int targetHeight)
        {
            this.mapWidth = targetWidth;
            this.mapHeight = targetHeight;
        }

        public void Reset() { CurrentRoom = 1; }

        public bool Update(Player player, List<Rectangle> platforms, List<Enemy> enemies, List<Gold> roomGolds, List<BreakableBlock> roomBlocks, List<TimedBlock> timedBlocks, List<MovingPlatform> movingPlatforms, out List<Rectangle> roomLadders)
        {
            roomLadders = new List<Rectangle>();

            if (player.X > mapWidth)
            {
                GameMap.SaveRoomState(CurrentRoom, enemies, roomGolds, roomBlocks);
                CurrentRoom++;

                if (CurrentRoom == 6) player.Y = 250 - player.Height - 5;
                else if (CurrentRoom == 7) player.Y = 450 - player.Height - 5;
                else if (CurrentRoom == 8) player.Y = 850 - player.Height - 5;
                // Oda 9'un yeni sol zemin yüksekliği usta
                else if (CurrentRoom == 9) player.Y = 850 - player.Height - 5;
                else if (CurrentRoom == 10) player.Y = 880 - player.Height - 5;
                else if (CurrentRoom == 11) player.Y = 650 - player.Height - 5;

                player.X = 10;
                GameMap.LoadRoom(CurrentRoom, platforms, enemies, roomGolds, roomBlocks, timedBlocks, movingPlatforms, out roomLadders);
                return true;
            }
            else if (player.X + player.Width < 0)
            {
                if (CurrentRoom > 1)
                {
                    GameMap.SaveRoomState(CurrentRoom, enemies, roomGolds, roomBlocks);
                    CurrentRoom--;

                    if (CurrentRoom == 5) player.Y = 250 - player.Height - 5;
                    else if (CurrentRoom == 6) player.Y = 750 - player.Height - 5;
                    else if (CurrentRoom == 7) player.Y = 450 - player.Height - 5;
                    else if (CurrentRoom == 8) player.Y = 850 - player.Height - 5;
                    else if (CurrentRoom == 9) player.Y = 450 - player.Height - 5; // Ondan dokuza dönerse sağ yüksek kata koy
                    else if (CurrentRoom == 10) player.Y = 280 - player.Height - 5;

                    player.X = mapWidth - player.Width - 15;
                    GameMap.LoadRoom(CurrentRoom, platforms, enemies, roomGolds, roomBlocks, timedBlocks, movingPlatforms, out roomLadders);
                    return true;
                }
                else player.X = 0;
            }
            return false;
        }
    }
}