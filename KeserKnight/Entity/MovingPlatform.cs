using System;
using System.Drawing;

namespace KeserKnight.Entity
{
    public class MovingPlatform
    {
        public Rectangle Hitbox { get; private set; }

        // Hareket tipi kontrolcüsü
        public enum MovementType { Horizontal, Vertical }
        public MovementType Type { get; private set; }

        private int startCoord;
        private int endCoord;
        private int speed = 4;
        private int direction = 1;

        public MovingPlatform(int x, int y, int width, int height, int travelDistance, MovementType type)
        {
            Hitbox = new Rectangle(x, y, width, height);
            this.Type = type;

            if (type == MovementType.Horizontal)
            {
                startCoord = x;
                endCoord = x + travelDistance;
            }
            else
            {
                startCoord = y;
                endCoord = y + travelDistance;
            }
        }

        public void Update(Player player)
        {
            int dx = 0;
            int dy = 0;

            if (Type == MovementType.Horizontal)
            {
                int nextX = Hitbox.X + (speed * direction);
                if (nextX >= endCoord) { nextX = endCoord; direction = -1; }
                else if (nextX <= startCoord) { nextX = startCoord; direction = 1; }

                dx = nextX - Hitbox.X;
                Hitbox = new Rectangle(nextX, Hitbox.Y, Hitbox.Width, Hitbox.Height);
            }
            else // Vertical (Dikey asansör hareketi)
            {
                int nextY = Hitbox.Y + (speed * direction);
                // Dikey eksende aşağı/yukarı sınır kontrolü
                if (nextY >= endCoord) { nextY = endCoord; direction = -1; }
                else if (nextY <= startCoord) { nextY = startCoord; direction = 1; }

                dy = nextY - Hitbox.Y;
                Hitbox = new Rectangle(Hitbox.X, nextY, Hitbox.Width, Hitbox.Height);
            }

            // Oyuncu platformun üstünde mi kontrolü
            bool isPlayerOnTop = player.Hitbox.X + player.Hitbox.Width > Hitbox.X &&
                                 player.Hitbox.X < Hitbox.Right &&
                                 Math.Abs(player.Hitbox.Bottom - Hitbox.Y) < 8 &&
                                 player.VerticalVelocity >= 0;

            if (isPlayerOnTop)
            {
                // Yatayda hareket ettir
                player.X += dx;
                // Dikeyde asansörle birlikte yukarı/aşağı taşı usta
                player.Y += dy;

                // Karakteri platformun tam üst çizgisine mühürle
                player.Y = Hitbox.Y - player.Hitbox.Height;
            }
        }

        public void Draw(Graphics g)
        {
            // Shovel Knight yeşil/çimenli mekanik hareketli zemin tasarımı
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 165, 60)))
            {
                g.FillRectangle(brush, Hitbox);
            }
            using (Pen pen = new Pen(Color.FromArgb(50, 35, 15), 4f))
            {
                g.DrawRectangle(pen, Hitbox);
            }
        }
    }
}