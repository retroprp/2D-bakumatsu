using System;
using System.Drawing;

namespace KeserKnight.Entity
{
    public class TimedBlock
    {
        public Rectangle Hitbox { get; private set; }
        public bool IsActive { get; private set; } = true;

        private int globalTimer = 0;
        private int switchInterval = 75; // Ritm hızı - ideal usta!
        private bool startInverted = false; // Başlangıçta kapalı mı başlayacak?

        public TimedBlock(int x, int y, int width, int height, int offsetTicks = 0)
        {
            Hitbox = new Rectangle(x, y, width, height);

            // Eğer offset varsa, bu bloğu ikinci gruptan sayıyoruz usta
            if (offsetTicks > 0)
            {
                this.startInverted = true;
                IsActive = false;
            }
        }

        public void Update(Player player)
        {
            globalTimer++;

            // Modulo (%) matematiği ile sonsuz ve pürüzsüz bir git-gel ritmi kuruyoruz
            int currentPhase = (globalTimer / switchInterval) % 2;

            if (!startInverted)
            {
                // 1. Grup Bloklar (Görseldeki 1. ve 3. Bloklar gibi): İlk fazda aktif, ikinci fazda pasif
                IsActive = (currentPhase == 0);
            }
            else
            {
                // 2. Grup Bloklar (Yeni işaretlediğin 2. Blok ve 4. Blok): İlk fazda pasif, ikinci fazda aktif
                IsActive = (currentPhase == 1);
            }
        }

        public void Draw(Graphics g)
        {
            if (!IsActive)
            {
                // Oyuncu nereye zıplayacağını görsün diye hafif retro neon izi usta
                using (Pen ghostPen = new Pen(Color.FromArgb(40, Color.Cyan), 2f))
                {
                    g.DrawRectangle(ghostPen, Hitbox);
                }
                return;
            }

            // Aktifken parıldayan Shovel Knight cam/taş blok tasarımı usta
            using (SolidBrush blockBrush = new SolidBrush(Color.FromArgb(140, 180, 230)))
            using (Pen borderPen = new Pen(Color.White, 3f))
            {
                g.FillRectangle(blockBrush, Hitbox);
                g.DrawRectangle(borderPen, Hitbox);

                // İç retro detay çizgileri
                g.DrawLine(borderPen, Hitbox.X + 10, Hitbox.Y + 10, Hitbox.Right - 10, Hitbox.Y + 10);
                g.DrawLine(borderPen, Hitbox.X + 10, Hitbox.Y + 10, Hitbox.X + 10, Hitbox.Bottom - 10);
            }
        }
    }
}