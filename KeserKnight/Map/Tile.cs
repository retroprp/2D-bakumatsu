using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeserKnight;

namespace KeserKnight.Map
{
    public class Tile
    {
        public enum TileType { Grass, Wall, Spike, Ladder }
        public TileType Type { get; set; }

        public Rectangle Hitbox { get; set; }
        public bool IsSolid { get; set; } // İçinden geçilebilir mi? (Merdiven geçilebilir, duvar geçilemez)

        public Tile(int x, int y, int width, int height, TileType type)
        {
            Hitbox = new Rectangle(x, y, width, height);
            Type = type;

            // Diken ve merdivenlerin içinden geçilebilmeli (Fiziği bozmamalı, tetikleyici olmalı)
            IsSolid = (type == TileType.Grass || type == TileType.Wall);
        }

        // Bloğu kendi türüne ait imajla ekrana basma fonksiyonu
        //public void Draw(Graphics g)
        //{
        //    Bitmap tileImage = null;

        //    switch (Type)
        //    {
        //        case TileType.Grass: tileImage = SpriteManager.BrickTile; break; // Görseldeki çimenli tuğla zemin
        //        case TileType.Wall: tileImage = SpriteManager.BrickTile; break;  // Duvar bloğu
        //        case TileType.Spike: tileImage = SpriteManager.SpikesTile; break; // Diken bloğu
        //                                                                          // case TileType.Ladder: tileImage = SpriteManager.LadderTile; break; // Merdiven (Gerekirse eklenecek)
        //    }

        //    if (tileImage != null)
        //    {
        //        // Doku kaymalarını önlemek için imajı tam hitbox boyutuna esneterek/sığdırarak çiziyoruz
        //        g.DrawImage(tileImage, Hitbox);
        //    }
        //    else
        //    {
        //        // Fallback: Görsel yüklenmediyse renkli kutu çiz
        //        Brush b = Type == TileType.Spike ? Brushes.Red : Brushes.DarkSlateGray;
        //        g.FillRectangle(b, Hitbox);
        //    }
        //}
    }
}
