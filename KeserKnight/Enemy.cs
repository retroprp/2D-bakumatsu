//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace KeserKnight
//{
//    public class Enemy
//    {

//        public Rectangle Hitbox;
//        private int speed = 4;
//        private int direction = 1; // 1 = Sağ, -1 = Sol
//        private int leftBound;    // Devriye atacağı sol sınır
//        private int rightBound;   // Devriye atacağı sağ sınır

//        // Düşmanı oluştururken yerini ve devriye alanını belirliyoruz
//        public Enemy(int x, int y, int width, int height, int patrolRange)
//        {
//            Hitbox = new Rectangle(x, y, width, height);
//            leftBound = x - patrolRange;
//            rightBound = x + patrolRange;
//        }

//        // Düşmanın hareket mantığı (Yapaz Zeka)
//        public void Update()
//        {
//            // Belirlenen yöne doğru yürü
//            Hitbox.X += speed * direction;

//            // Sınırlara geldiğinde yön değiştir
//            if (Hitbox.X >= rightBound)
//            {
//                direction = -1; // Sola dön
//            }
//            else if (Hitbox.X <= leftBound)
//            {
//                direction = 1;  // Sağa dön
//            }
//        }

//        // Düşmanı ekrana çizme fonksiyonu
//        public void Draw(Graphics g)
//        {
//            // Şimdilik grafiğimiz olmadığı için düşmanı kırmızı bir kutu yapalım
//            g.FillRectangle(Brushes.Crimson, Hitbox);
//        }

//    }
//}
