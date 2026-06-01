using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace KeserKnight.UI
{
    public static class PauseMenuUI
    {
        public static void Draw(Graphics g, int pauseSelection, Rectangle resumeBtn, Rectangle settingsBtn, Rectangle mainMenuBtn)
        {
            // Arka planı hafif karartmak için transparan perde
            using (SolidBrush pauseOverlay = new SolidBrush(Color.FromArgb(180, Color.Black)))
            {
                g.FillRectangle(pauseOverlay, 0, 0, 1920, 1080);
            }

            // "OYUN DURAKLATILDI" Başlığı
            using (Font pauseTitleFont = new Font("Impact", 55, FontStyle.Bold))
            {
                string pTitleText = "OYUN DURAKLATILDI";
                int pTitleX = (1920 - TextRenderer.MeasureText(pTitleText, pauseTitleFont).Width) / 2;
                g.DrawString(pTitleText, pauseTitleFont, Brushes.Gold, pTitleX, 280);
            }

            // --- BELLEK KORUMA ALANI ---
            // Tüm butonların yazı tiplerini ve çerçeve kalemlerini tek seferde kafesliyoruz usta
            using (Font btnFont = new Font("Arial", 18, FontStyle.Bold))
            using (Pen selectPen = new Pen(Color.White, 5))
            using (Pen cyanPen = new Pen(Color.Cyan, 1))
            using (Pen lightGrayPen = new Pen(Color.LightGray, 1))
            using (Pen redPen = new Pen(Color.Red, 1))
            {
                // Devam Et Butonu
                g.FillRectangle(Brushes.DarkBlue, resumeBtn);
                if (pauseSelection == 0) g.DrawRectangle(selectPen, resumeBtn);
                else g.DrawRectangle(cyanPen, resumeBtn);
                g.DrawString("DEVAM ET", btnFont, Brushes.White, resumeBtn.X + 85, resumeBtn.Y + 15);

                // Ayarlar Butonu
                g.FillRectangle(Brushes.DarkSlateGray, settingsBtn);
                if (pauseSelection == 1) g.DrawRectangle(selectPen, settingsBtn);
                else g.DrawRectangle(lightGrayPen, settingsBtn);
                g.DrawString("AYARLAR", btnFont, Brushes.White, settingsBtn.X + 90, settingsBtn.Y + 15);

                // Ana Menü Butonu
                g.FillRectangle(Brushes.DarkRed, mainMenuBtn);
                if (pauseSelection == 2) g.DrawRectangle(selectPen, mainMenuBtn);
                else g.DrawRectangle(redPen, mainMenuBtn);
                g.DrawString("ANA MENÜ", btnFont, Brushes.White, mainMenuBtn.X + 85, mainMenuBtn.Y + 15);
            }
        }
    }
}