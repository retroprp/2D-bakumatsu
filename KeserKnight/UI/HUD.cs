using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace KeserKnight.UI
{
    public static class GameUI
    {
        // =================================================================
        // 1. ANA MENÜ ÇİZİM MOTORU (RAM DOSTU)
        // =================================================================
        public static void DrawMainMenu(Graphics g, int menuSelection, Rectangle startBtn, Rectangle exitBtn)
        {
            g.Clear(Color.FromArgb(11, 11, 26));

            using (Font titleFont = new Font("Impact", 60, FontStyle.Bold))
            {
                string titleText = "SHOVEL KNIGHT";
                int titleX = (1920 - TextRenderer.MeasureText(titleText, titleFont).Width) / 2;
                g.DrawString(titleText, titleFont, Brushes.Gold, titleX, 200);
            }

            using (Font subTitleFont = new Font("Arial", 20, FontStyle.Italic))
            {
                string subTitleText = "C# REMAKE";
                int subTitleX = (1920 - TextRenderer.MeasureText(subTitleText, subTitleFont).Width) / 2;
                g.DrawString(subTitleText, subTitleFont, Brushes.White, subTitleX, 300);
            }

            // Çerçeve kalemlerini ve buton yazı tipini güvenli alana alıyoruz
            using (Font buttonFont = new Font("Arial", 18, FontStyle.Bold))
            using (Pen selectPen = new Pen(Color.White, 5))
            using (Pen cyanPen = new Pen(Color.Cyan, 1))
            using (Pen redPen = new Pen(Color.Red, 1))
            {
                g.FillRectangle(Brushes.DarkBlue, startBtn);
                if (menuSelection == 0) g.DrawRectangle(selectPen, startBtn);
                else g.DrawRectangle(cyanPen, startBtn);
                g.DrawString("OYUNA BAŞLA", buttonFont, Brushes.White, startBtn.X + 60, startBtn.Y + 15);

                g.FillRectangle(Brushes.DarkRed, exitBtn);
                if (menuSelection == 1) g.DrawRectangle(selectPen, exitBtn);
                else g.DrawRectangle(redPen, exitBtn);
                g.DrawString("ÇIKIŞ YAP", buttonFont, Brushes.White, exitBtn.X + 85, exitBtn.Y + 15);
            }
        }

        // =================================================================
        // 2. OYUN İÇİ ÜST PANEL HUD ÇİZİM MOTORU (FPS CANAVARI BİTTİ)
        // =================================================================
        public static void DrawHUD(Graphics g, int currentRoom, int maxHp, int currentHp, int totalGold, Image kalpDolu, Image kalpBos)
        {
            g.FillRectangle(Brushes.Black, 0, 0, 1920, 110);

            // Saniyede 60 kez yeni kalem ve font üretilmesini engelleyen kritik kafes
            using (Pen hudLinePen = new Pen(Color.FromArgb(50, 50, 60), 4))
            using (Pen whiteEllipsePen = new Pen(Color.White, 2))
            using (Font hudFont = new Font("Impact", 28, FontStyle.Regular))
            {
                g.DrawLine(hudLinePen, 0, 110, 1920, 110);

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

                int goldX = 850;
                g.FillEllipse(Brushes.Gold, goldX, startY + 22, 40, 40);
                g.DrawEllipse(whiteEllipsePen, goldX, startY + 22, 40, 40);
                g.DrawString("GOLD: " + totalGold, hudFont, Brushes.Gold, goldX + 60, startY + 17);

                string roomText = "STAGE: 0" + currentRoom;
                g.DrawString(roomText, hudFont, Brushes.White, 1650, startY + 17);
            }
        }

        // =================================================================
        // 3. ESC (PAUSE) DURAKLATMA MENÜSÜ ÇİZİM MOTORU (TAMAMEN TEMİZLENDİ)
        // =================================================================
        public static void DrawPauseScreen(Graphics g, int pauseSelection, Rectangle resumeBtn, Rectangle settingsBtn, Rectangle mainMenuBtn)
        {
            using (SolidBrush pauseOverlay = new SolidBrush(Color.FromArgb(180, Color.Black)))
            {
                g.FillRectangle(pauseOverlay, 0, 0, 1920, 1080);
            }

            using (Font pauseTitleFont = new Font("Impact", 55, FontStyle.Bold))
            {
                string pTitleText = "OYUN DURAKLATILDI";
                int pTitleX = (1920 - TextRenderer.MeasureText(pTitleText, pauseTitleFont).Width) / 2;
                g.DrawString(pTitleText, pauseTitleFont, Brushes.Gold, pTitleX, 280);
            }

            // Duraklatma menüsü buton fontu ve kalem sızıntılarını mühürlüyoruz
            using (Font btnFont = new Font("Arial", 18, FontStyle.Bold))
            using (Pen selectPen = new Pen(Color.White, 5))
            using (Pen cyanPen = new Pen(Color.Cyan, 1))
            using (Pen lightGrayPen = new Pen(Color.LightGray, 1))
            using (Pen redPen = new Pen(Color.Red, 1))
            {
                g.FillRectangle(Brushes.DarkBlue, resumeBtn);
                if (pauseSelection == 0) g.DrawRectangle(selectPen, resumeBtn);
                else g.DrawRectangle(cyanPen, resumeBtn);
                g.DrawString("DEVAM ET", btnFont, Brushes.White, resumeBtn.X + 85, resumeBtn.Y + 15);

                g.FillRectangle(Brushes.DarkSlateGray, settingsBtn);
                if (pauseSelection == 1) g.DrawRectangle(selectPen, settingsBtn);
                else g.DrawRectangle(lightGrayPen, settingsBtn);
                g.DrawString("AYARLAR", btnFont, Brushes.White, settingsBtn.X + 90, settingsBtn.Y + 15);

                g.FillRectangle(Brushes.DarkRed, mainMenuBtn);
                if (pauseSelection == 2) g.DrawRectangle(selectPen, mainMenuBtn);
                else g.DrawRectangle(redPen, mainMenuBtn);
                g.DrawString("ANA MENÜ", btnFont, Brushes.White, mainMenuBtn.X + 85, mainMenuBtn.Y + 15);
            }
        }

        // =================================================================
        // 4. ÖLÜM EKRANI ÇİZİM MOTORU (JİLET GİBİ)
        // =================================================================
        public static void DrawGameOverScreen(Graphics g)
        {
            using (SolidBrush alphaBrush = new SolidBrush(Color.FromArgb(180, Color.Black)))
            {
                g.FillRectangle(alphaBrush, 0, 0, 1920, 1080);
            }

            // Buradaki kontrolsüz fontları da kafesleyerek işi bitiriyoruz usta
            using (Font gameOverFont = new Font("Arial", 60, FontStyle.Bold))
            using (Font subFont = new Font("Arial", 25, FontStyle.Regular))
            {
                g.DrawString("GAME OVER", gameOverFont, Brushes.Red, (1920 - TextRenderer.MeasureText("GAME OVER", gameOverFont).Width) / 2, 400);
                g.DrawString("Yeniden Başlamak İçin 'R' Tuşuna Basın", subFont, Brushes.White, (1920 - TextRenderer.MeasureText("Yeniden Başlamak İçin 'R' Tuşuna Basın", subFont).Width) / 2, 550);
            }
        }
    }
}