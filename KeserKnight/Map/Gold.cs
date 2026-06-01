using System;
using System.Drawing;

namespace KeserKnight.Map
{
    public class Gold
    {
        // --- PRO ÖZELLİKLER ---
        public Rectangle Hitbox { get; set; }
        public int Value { get; set; } // Altının değeri (Örn: Sarı 10 altın, Mavi 50 altın)
        public Color GoldColor { get; set; }

        // İleride buraya dönen altın animasyonu spriteları koymak istersen hazır altyapı usta
        public Image Texture { get; set; }

        public Gold(int x, int y, int value, Color color, Image texture = null)
        {
            // Altınlar küçük parıldayan kutular olacak (20x20 piksel idealdir)
            Hitbox = new Rectangle(x, y, 20, 20);
            Value = value;
            GoldColor = color;
            this.Texture = texture;
        }

        // Altını ekrana çizme fonksiyonu (Sanal Tuval Motoruna %100 Uyumlu)
        public void Draw(Graphics g)
        {
            if (Texture != null)
            {
                // Eğer bir altın görseli yüklenirse onu bas usta
                g.DrawImage(Texture, Hitbox);
            }
            else
            {
                // Görsel yoksa arkadaşının yazdığı o şık elips parıldama efekti devreye girer
                using (SolidBrush brush = new SolidBrush(GoldColor))
                {
                    g.FillEllipse(brush, Hitbox); // Yuvarlak altın parası efekti
                }
                g.DrawEllipse(Pens.White, Hitbox); // Dışına tatlı bir parlama çerçevesi
            }
        }
    }
}