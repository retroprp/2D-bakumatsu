using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace KeserKnight.UI
{
    public static class MainMenuUI
    {
        public static void Draw(Graphics g, int menuSelection, Rectangle startBtn, Rectangle exitBtn)
        {
            g.Clear(Color.FromArgb(11, 11, 26));

            // Büyük Başlık
            using (Font titleFont = new Font("Impact", 60, FontStyle.Bold))
            {
                string titleText = "SHOVEL KNIGHT";
                int titleX = (1920 - TextRenderer.MeasureText(titleText, titleFont).Width) / 2;
                g.DrawString(titleText, titleFont, Brushes.Gold, titleX, 200);
            }

            // Alt Başlık
            using (Font subTitleFont = new Font("Arial", 20, FontStyle.Italic))
            {
                string subTitleText = "C# REMAKE";
                int subTitleX = (1920 - TextRenderer.MeasureText(subTitleText, subTitleFont).Width) / 2;
                g.DrawString(subTitleText, subTitleFont, Brushes.White, subTitleX, 300);
            }

            // --- BELLEK KORUMA ALANI ---
            // Buton fontu ve beyaz çerçeve kalemini bir kereliğine güvenli alana alıyoruz usta
            using (Font buttonFont = new Font("Arial", 18, FontStyle.Bold))
            using (Pen selectPen = new Pen(Color.White, 5))
            using (Pen cyanPen = new Pen(Color.Cyan, 1))
            using (Pen redPen = new Pen(Color.Red, 1))
            {
                // Oyuna Başla Butonu
                g.FillRectangle(Brushes.DarkBlue, startBtn);
                if (menuSelection == 0) g.DrawRectangle(selectPen, startBtn);
                else g.DrawRectangle(cyanPen, startBtn);
                g.DrawString("OYUNA BAŞLA", buttonFont, Brushes.White, startBtn.X + 60, startBtn.Y + 15);

                // Çıkış Yap Butonu
                g.FillRectangle(Brushes.DarkRed, exitBtn);
                if (menuSelection == 1) g.DrawRectangle(selectPen, exitBtn);
                else g.DrawRectangle(redPen, exitBtn);
                g.DrawString("ÇIKIŞ YAP", buttonFont, Brushes.White, exitBtn.X + 85, exitBtn.Y + 15);
            }
        }
    }
}