using System;
using System.Collections.Generic;
using System.Drawing;
using KeserKnight.Entity;

namespace KeserKnight.Core
{
    public class PhysicsEngine
    {
        public void Update(Player player, List<Rectangle> platforms)
        {
            // 1. Hızları ve Yerçekimini Güncelle
            player.VerticalVelocity += player.Gravity;
            if (player.VerticalVelocity > 25) player.VerticalVelocity = 25; // Terminal Velocity

            // 2. Karakterin gitmek istediği bir sonraki X ve Y koordinatını hesapla
            int nextX = player.X;
            bool canMoveHorizontally = true;

            // Yerdeyken çömeliyorsa yatay hareketi kitle
            if (player.IsCrouching && !player.IsJumping)
            {
                canMoveHorizontally = false;
            }

            if (canMoveHorizontally)
            {
                if (player.MoveLeft && !player.MoveRight)
                {
                    nextX -= player.Speed;
                    player.CurrentDirection = Player.Direction.Left;
                }
                else if (player.MoveRight && !player.MoveLeft)
                {
                    nextX += player.Speed;
                    player.CurrentDirection = Player.Direction.Right;
                }
                else if (player.MoveLeft && player.MoveRight)
                {
                    if (player.CurrentDirection == Player.Direction.Left) nextX -= player.Speed;
                    else nextX += player.Speed;
                }
            }

            float nextY = player.Y + player.VerticalVelocity;

            // 3. Karakteri hedef koordinatına taşı
            player.X = nextX;
            player.Y = (int)nextY;

            player.IsJumping = true;

            // 4. ÇARPIŞMA ÇÖZÜMLEME MOTORU
            foreach (var platform in platforms)
            {
                if (player.Hitbox.IntersectsWith(platform))
                {
                    Rectangle overlap = Rectangle.Intersect(player.Hitbox, platform);

                    // Dikey Çarpışma (Zemin ve Tavan)
                    if (overlap.Width > overlap.Height)
                    {
                        if (player.VerticalVelocity >= 0 && player.Hitbox.Bottom - overlap.Height <= platform.Top)
                        {
                            // İŞTE ÇÖZÜM BURADA: Y eksenini Hitbox'a göre değil, orijinal resim boyuna göre ayarlıyoruz!
                            player.Y = platform.Top - player.Height;
                            player.VerticalVelocity = 0;
                            player.IsJumping = false;
                        }
                        else if (player.VerticalVelocity < 0 && player.Hitbox.Top + overlap.Height >= platform.Bottom)
                        {
                            // Tavana çarpma durumunda ofseti dinamik hesapla
                            int topOffset = player.Hitbox.Top - player.Y;
                            player.Y = platform.Bottom + 1 - topOffset;
                            player.VerticalVelocity = 0;
                        }
                    }
                    // Yatay Çarpışma (Duvarlar)
                    else
                    {
                        // Padding değerini dinamik olarak hesapla (sabit 30 pikselden kurtulduk)
                        int leftOffset = player.Hitbox.Left - player.X;

                        if (player.Hitbox.Center().X < platform.Center().X)
                        {
                            // Soldan duvara çarpma
                            player.X = platform.Left - player.Hitbox.Width - leftOffset;
                        }
                        else
                        {
                            // Sağdan duvara çarpma
                            player.X = platform.Right - leftOffset + 1;
                        }
                    }
                }
            }

            // 5. EKRANIN ÜST SINIR LİMİT KORUMASI
            ApplyScreenLimits(player);
        }

        private void ApplyScreenLimits(Player player)
        {
            if (player.Y < 110)
            {
                player.Y = 110;
                player.VerticalVelocity = 0;
            }
        }
    }

    // Dikdörtgenin merkezini bulmak için yardımcı eklenti
    public static class RectangleExtensions
    {
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }
    }
}