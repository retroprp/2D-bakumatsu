using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace KeserKnight.UI
{
    public static class GameOverUI
    {
        public static void Draw(Graphics g)
        {
            // Yarı transparan arka plan karartması
            using (SolidBrush alphaBrush = new SolidBrush(Color.FromArgb(180, Color.Black)))
            {
                g.FillRectangle(alphaBrush, 0, 0, 1920, 1080);
            }

            // --- BELLEK KORUMA ALANI ---
            // Her iki yazı tipini de tek bir güvenli using bloğunda topluyoruz usta
            using (Font gameOverFont = new Font("Arial", 60, FontStyle.Bold))
            using (Font subFont = new Font("Arial", 25, FontStyle.Regular))
            {
                // GAME OVER Yazısı
                g.DrawString("GAME OVER", gameOverFont, Brushes.Red, (1920 - TextRenderer.MeasureText("GAME OVER", gameOverFont).Width) / 2, 400);

                // Yeniden Başlama İpucu
                g.DrawString("Yeniden Başlamak İçin 'R' Tuşuna Basın", subFont, Brushes.White, (1920 - TextRenderer.MeasureText("Yeniden Başlamak İçin 'R' Tuşuna Basın", subFont).Width) / 2, 550);
            }
        }
    }
}