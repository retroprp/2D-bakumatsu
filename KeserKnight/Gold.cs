//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace KeserKnight
//{
//    public class Gold
//    {
//        public Rectangle Hitbox { get; set; }
//        public int Value { get; set; } // Altının değeri (Örn: Sarı 10 altın, Mavi 50 altın)
//        public Color GoldColor { get; set; }

//        public Gold(int x, int y, int value, Color color)
//        {
//            // Altınlar küçük parıldayan kutular olacak (20x20 piksel idealdir)
//            Hitbox = new Rectangle(x, y, 20, 20);
//            Value = value;
//            GoldColor = color;
//        }

//        public void Draw(Graphics g)
//        {
//            // Altını yuvarlak veya elmas gibi parıldayan bir kutu olarak çiziyoruz
//            using (SolidBrush brush = new SolidBrush(GoldColor))
//            {
//                g.FillEllipse(brush, Hitbox); // Yuvarlak altın parası efekti
//            }
//            g.DrawEllipse(Pens.White, Hitbox); // Dışına tatlı bir parlama çerçevesi
//        }
//    }
//}
