using System;
using System.Drawing;

namespace KeserKnight.Entity
{
    public class BreakableBlock
    {
        public Rectangle Hitbox { get; private set; }
        public int Health { get; private set; } = 30; // 3 vuruşta kırılır (Oyuncu hasarı 10)
        public bool IsBroken => Health <= 0;

        // Shovel Knight tarzı turuncu/sarı blok rengi
        private readonly Color blockColor = Color.FromArgb(230, 140, 40);

        public BreakableBlock(int x, int y, int width, int height)
        {
            Hitbox = new Rectangle(x, y, width, height);
        }

        public void TakeDamage(int damage)
        {
            if (IsBroken) return;
            Health -= damage;
        }

        public void Draw(Graphics g)
        {
            if (IsBroken) return;

            // Bloğun dış gövdesi (Shovel Knight Turuncusu)
            using (SolidBrush brush = new SolidBrush(blockColor))
            {
                g.FillRectangle(brush, Hitbox);
            }

            // Çatlak pikselleri ve jilet gibi retro çerçevesi
            using (Pen pen = new Pen(Color.FromArgb(80, 40, 10), 4f))
            {
                g.DrawRectangle(pen, Hitbox);

                // Canı azaldıkça blok üzerinde dinamik çatlaklar çizdiriyoruz usta
                if (Health <= 20)
                {
                    g.DrawLine(pen, Hitbox.X + 10, Hitbox.Y + 10, Hitbox.Right - 10, Hitbox.Bottom - 10);
                }
                if (Health <= 10)
                {
                    g.DrawLine(pen, Hitbox.Right - 15, Hitbox.Y + 10, Hitbox.X + 15, Hitbox.Bottom - 10);
                }
            }
        }
    }
}