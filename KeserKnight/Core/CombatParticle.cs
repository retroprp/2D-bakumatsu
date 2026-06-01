using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KeserKnight.Core
{
   
    public class CombatParticle
    {
        public PointF Position;     // Parçacığın tuval üzerindeki anlık X, Y konumu
        public PointF Velocity;     // Parçacığın X ve Y eksenindeki kare başı hız ivmesi
        public Color BaseColor;     // Parçacığın doğuş rengi (Örn: Sarı veya Turuncu)
        public float Size;          // Piksel bazlı boyutu
        public float LifeTime;      // Kalan ömrü (Sıfıra yaklaştıkça yok olur)
        public float MaxLifeTime;   // Toplam yaşam süresi (Transparanlık oranı için gerekli)
        public bool IsActive;       // Havuzda boşta mı yoksa ekranda yayında mı?

        public CombatParticle()
        {
            IsActive = false; // İlk doğduklarında hepsi uykuda (pasif) başlar 
        }

      
        public void Update()
        {
            if (!IsActive) return;

            // Konumu, hız vektörleri kadar ileri taşıyoruz
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;

            // Ömrünü tüket (Her Tick'te minik bir zaman azaltıyoruz)
            LifeTime -= 0.016f; // Yaklaşık 60 FPS simülasyonu için ideal saniye çarpanı

            // Ömrü bittiyse parçacığı silme, sadece havuza geri gönder 
            if (LifeTime <= 0)
            {
                IsActive = false;
            }
        }

       
        public void Draw(Graphics g)
        {
            if (!IsActive) return;

            // --- GRUP ÇALIŞMASI NOTU: TRANSPARANLIK (FADE OUT) MATEMATİĞİ ---
            // Kalan ömrün toplam ömre oranını bulup 0 ile 255 arasında bir Alpha (Görünürlük) değeri üretiyoruz.
            float lifeRatio = LifeTime / MaxLifeTime;
            if (lifeRatio < 0) lifeRatio = 0;
            if (lifeRatio > 1) lifeRatio = 1;

            int alpha = (int)(lifeRatio * 255);

            // Yeni transparan rengi oluşturup pikselleri basıyoruz
            using (SolidBrush pBrush = new SolidBrush(Color.FromArgb(alpha, BaseColor)))
            {
                g.FillRectangle(pBrush, Position.X, Position.Y, Size, Size);
            }
        }
    }
}
