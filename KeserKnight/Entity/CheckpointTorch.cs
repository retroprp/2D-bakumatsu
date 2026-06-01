using System;
using System.Drawing;

namespace KeserKnight.Entity
{
    public class CheckpointTorch
    {
        public Rectangle Hitbox { get; private set; }
        private float glowTimer = 0f;

        public CheckpointTorch(int x, int y, int width, int height)
        {
            Hitbox = new Rectangle(x, y, width, height);
        }

        public void Update()
        {
            // Meşale alevinin retro parlaması için sinüs dalgası zamanlayıcısı
            glowTimer += 0.1f;
        }

        public void Draw(Graphics g)
        {
            // 1. Altın Şamdan Tabanı
            using (SolidBrush goldBrush = new SolidBrush(Color.FromArgb(230, 180, 30)))
            {
                // Ayak
                g.FillRectangle(goldBrush, Hitbox.X + (Hitbox.Width / 2) - 8, Hitbox.Y + 30, 16, Hitbox.Height - 30);
                // Çanak
                g.FillRectangle(goldBrush, Hitbox.X, Hitbox.Y + 15, Hitbox.Width, 15);
            }

            // 2. Parlayan Alev (Sinüs dalgalı büyüme/küçülme efekti)
            int glowSize = (int)(25 + Math.Sin(glowTimer) * 5);
            int flameX = Hitbox.X + (Hitbox.Width / 2) - (glowSize / 2);
            int flameY = Hitbox.Y - 10;

            using (SolidBrush flameBrush = new SolidBrush(Color.FromArgb(255, 100, 0)))
            using (SolidBrush coreBrush = new SolidBrush(Color.Gold))
            {
                g.FillEllipse(flameBrush, flameX, flameY, glowSize, glowSize + 8);
                g.FillEllipse(coreBrush, flameX + 5, flameY + 6, glowSize - 10, glowSize - 4);
            }
        }
    }
}