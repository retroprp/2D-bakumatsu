using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace KeserKnight.UI
{
    public static class HUDRenderer
    {
        public static void Draw(Graphics g, int currentRoom, int maxHp, int currentHp, int totalGold, Image kalpDolu, Image kalpBos)
        {
            // Siyah Üst HUD Barı
            g.FillRectangle(Brushes.Black, 0, 0, 1920, 110);

            // --- BELLEK KORUMA ALANI ---
            // Saniyede 60 kez yeni kalem ve font üretilmesini engelleyen kafes
            using (Pen hudLinePen = new Pen(Color.FromArgb(50, 50, 60), 4))
            using (Pen whiteEllipsePen = new Pen(Color.White, 2))
            using (Font hudFont = new Font("Impact", 28, FontStyle.Regular))
            {
                g.DrawLine(hudLinePen, 0, 110, 1920, 110);

                // Kalp Can Göstergeleri Çizimi
                int startX = 50; int startY = 15; int boxSize = 80; int gap = 12;
                for (int i = 0; i < maxHp; i++)
                {
                    int currentHeartX = startX + (i * (boxSize + gap));
                    if (i < currentHp)
                    {
                        if (kalpDolu != null) g.DrawImage(kalpDolu, currentHeartX, startY, boxSize, boxSize);
                        else g.FillRectangle(Brushes.Crimson, currentHeartX, startY, boxSize, boxSize);
                    }
                    else
                    {
                        if (kalpBos != null) g.DrawImage(kalpBos, currentHeartX, startY, boxSize, boxSize);
                        else g.FillRectangle(Brushes.DimGray, currentHeartX, startY, boxSize, boxSize);
                    }
                }

                // Altın (Gold) Göstergesi
                int goldX = 850;
                g.FillEllipse(Brushes.Gold, goldX, startY + 22, 40, 40);
                g.DrawEllipse(whiteEllipsePen, goldX, startY + 22, 40, 40);
                g.DrawString("GOLD: " + totalGold, hudFont, Brushes.Gold, goldX + 60, startY + 17);

                // Bölüm/Oda Bilgisi
                string roomText = "STAGE: 0" + currentRoom;
                g.DrawString(roomText, hudFont, Brushes.White, 1650, startY + 17);
            }
        }
    }
}