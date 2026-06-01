using System;
using System.Drawing;

namespace KeserKnight.Combat
{
    public class DeathExplosion
    {
        public Point Position { get; private set; }
        public int Radius { get; private set; } = 10;
        private int maxRadius;
        public bool IsFinished { get; private set; } = false;
        private Random rand = new Random();

        public DeathExplosion(int x, int y)
        {
            Position = new Point(x, y);
            maxRadius = rand.Next(40, 75); // Rastgele büyüklükte patlama halkaları
        }

        public void Update()
        {
            Radius += 4; // Halkaların büyüme hızı
            if (Radius >= maxRadius) IsFinished = true;
        }

        public void Draw(Graphics g)
        {
            if (IsFinished) return;

            // Dış halka parlaması (Retro Turuncu/Sarı/Beyaz katmanları)
            using (SolidBrush yellowBrush = new SolidBrush(Color.FromArgb(255, 230, 50)))
            using (Pen firePen = new Pen(Color.FromArgb(255, 60, 0), 3f))
            {
                g.DrawEllipse(firePen, Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
                g.FillEllipse(yellowBrush, Position.X - (Radius / 2), Position.Y - (Radius / 2), Radius, Radius);
            }
        }
    }
}