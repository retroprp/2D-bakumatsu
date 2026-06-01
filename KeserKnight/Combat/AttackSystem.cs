using System;
using System.Collections.Generic;
using System.Drawing;
using KeserKnight.Entity;

namespace KeserKnight.Combat
{
    public class AttackSystem
    {
        // 1. Saldırı Girdisini Dinleyip Saldırıyı Başlatma
        public void HandleAttackInput(Player player)
        {
            if (!player.IsAttacking)
            {
                player.IsAttacking = true;
                player.AttackTimer = 0;
            }
        }

        // 2. Aktif Saldırının Hitbox Alanını Baktığı Yöne Göre Hesaplama
        public void UpdateAttackHitbox(Player player)
        {
            if (!player.IsAttacking)
            {
                player.AttackHitbox = Rectangle.Empty;
                return;
            }

            // KESİN SENKRONİZASYON FİXİ:
            // Işınlanmayı ve ıska geçmeyi engellemek için doğrudan player.Hitbox'ın anlık güncel sınırlarını okuyoruz usta.
            var pHitbox = player.Hitbox;

            if (player.CurrentDirection == Player.Direction.Left)
            {
                player.AttackHitbox = new Rectangle(pHitbox.X - 80, pHitbox.Y + 10, 80, pHitbox.Height - 20);
            }
            else
            {
                player.AttackHitbox = new Rectangle(pHitbox.Right, pHitbox.Y + 10, 80, pHitbox.Height - 20);
            }
        }

        // 3. Düşmanların Vurulma Durumunu ve Can Kayıplarını Kontrol Etme
        public void CheckEnemyCollisions(Player player, List<Enemy> enemies)
        {
            if (!player.IsAttacking || player.AttackHitbox.IsEmpty) return;

            // Listeden eleman silinebileceği için döngüyü tersten işletiyoruz usta
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];

                // Hem player.AttackHitbox hem de enemy.Hitbox artık property üzerinden %100 gerçek koordinatları veriyor.
                if (player.AttackHitbox.IntersectsWith(enemy.Hitbox))
                {
                    // Düşman darbe aldı: Listeden kaldır
                    enemies.RemoveAt(i);

                    // Başarılı vuruş sonrası saldırı durumunu kapatıyoruz
                    player.IsAttacking = false;
                    player.AttackHitbox = Rectangle.Empty;
                    break;
                }
            }
        }
    }
}