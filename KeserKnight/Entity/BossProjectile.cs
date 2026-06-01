using System;
using System.Drawing;

namespace KeserKnight.Combat
{
    public class BossProjectile
    {
        public Rectangle Hitbox { get; private set; }

        private int basePriceY;
        private int speedX = -10;
        private float waveTimer = 0f;

        private float waveSpeed = 0.08f;
        private float amplitude = 120f;
        private int waveDirection = 1;

        public BossProjectile(int x, int y, int width, int height, bool moveUpFirst)
        {
            Hitbox = new Rectangle(x, y, width, height);
            this.basePriceY = y;
            this.waveDirection = moveUpFirst ? 1 : -1;
        }

        public void Update()
        {
            waveTimer += waveSpeed;
            int nextX = Hitbox.X + speedX;
            int nextY = basePriceY - (int)(Math.Sin(waveTimer) * amplitude * waveDirection);
            Hitbox = new Rectangle(nextX, nextY, Hitbox.Width, Hitbox.Height);
        }

        public void Draw(Graphics g)
        {
            using (SolidBrush fireBrush = new SolidBrush(Color.FromArgb(255, 90, 0)))
                g.FillEllipse(fireBrush, Hitbox);
            using (SolidBrush coreBrush = new SolidBrush(Color.Gold))
                g.FillEllipse(coreBrush, Hitbox.X + 8, Hitbox.Y + 8, Hitbox.Width - 16, Hitbox.Height - 16);
            using (Pen glowPen = new Pen(Color.White, 2f))
                g.DrawEllipse(glowPen, Hitbox);
        }
    }
}