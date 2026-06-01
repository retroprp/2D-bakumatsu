using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace KeserKnight.Core
{
    public class ParallaxLayer
    {
        public Image Image { get; set; }
        public float SpeedFactor { get; set; }
        public int YOffset { get; set; }
        public bool TileHorizontal { get; set; }

        public ParallaxLayer(Image image, float speedFactor, int yOffset = 0, bool tileHorizontal = true)
        {
            Image = image;
            SpeedFactor = speedFactor;
            YOffset = yOffset;
            TileHorizontal = tileHorizontal;
        }

        public void Draw(Graphics g, float playerX, int targetWidth, int targetHeight)
        {
            if (Image == null) return;

            // En-Boy oranını koruyarak genişliği hesaplıyoruz (Ağaçlar sündürülmeden heybetli duracak)
            float scale = (float)targetHeight / Image.Height;
            int scaledWidth = (int)(Image.Width * scale);
            int scaledHeight = targetHeight;

            int scrollX = (int)(playerX * SpeedFactor);

            // WinForms'ta anlık ölçekleme, devasa şeffaf bitmap çizmekten çok daha hızlıdır!
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (TileHorizontal)
            {
                int startX = -(scrollX % scaledWidth);
                if (startX > 0) startX -= scaledWidth;

                // Resim geniş olduğu için döngü çok az çalışacak, FPS uçacak
                for (int x = startX; x < targetWidth; x += scaledWidth)
                {
                    g.DrawImage(Image, x, YOffset, scaledWidth, scaledHeight);
                }
            }
            else
            {
                g.DrawImage(Image, 0, 0, targetWidth, targetHeight);
            }
        }
    }
}