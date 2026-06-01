using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace KeserKnight.Entity
{
    public class Enemy
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        public int X
        {
            get => _x;
            set { _x = value; UpdateHitbox(); }
        }

        public int Y
        {
            get => _y;
            set { _y = value; UpdateHitbox(); }
        }

        public int Width => _width;
        public int Height => _height;

        public Rectangle Hitbox => IsDead ? Rectangle.Empty : _hitbox;
        private Rectangle _hitbox;

        private int speed = 4;
        private int direction = 1;
        private int leftBound;
        private int rightBound;

        public Image Texture { get; set; }

        private int detectionRange = 400;
        private int attackRange = 120;
        private int attackCooldown = 0;
        public bool IsEnemyAttacking = false;
        public Rectangle EnemyAttackHitbox { get; private set; }
        public int EnemyAttackTimer = 0;

        public int Health { get; set; } = 20;
        public bool IsHurt { get; set; } = false;
        public int HurtTimer { get; set; } = 0;
        public bool IsDead { get; set; } = false;
        public bool IsGrounded { get; set; } = false;
        public float DeathRotation { get; set; } = 0f;

        private float velocityX;
        private float velocityY;

        public Enemy(int x, int y, int width, int height, int patrolRange, Image texture = null)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            leftBound = x - patrolRange;
            rightBound = x + patrolRange;
            this.Texture = texture ?? KeserKnight.Properties.Resources.anadusman;
            this.Health = 20;

            UpdateHitbox();
        }

        private void UpdateHitbox()
        {
            // Pogo optimizasyonu için yatay boşluk payı artırıldı ve tepe noktası biraz aşağı çekildi
            int paddingX = 14;
            int topOffset = 12;
            _hitbox = new Rectangle(_x + paddingX, _y + topOffset, _width - (paddingX * 2), _height - topOffset);
        }

        public void TakeDamage(int damage, int hitDirection)
        {
            if (IsDead) return;

            Health -= damage;
            IsHurt = true;
            HurtTimer = 15;

            velocityX = hitDirection * 12;
            velocityY = 0;

            if (Health <= 0)
            {
                IsDead = true;
                IsHurt = false;
                velocityY = -22;
                velocityX = hitDirection * 5;
                DeathRotation = 90f;
            }
        }

        public void Update(List<Rectangle> platforms, Player player)
        {
            if (IsDead)
            {
                if (!IsGrounded)
                {
                    velocityY += 1.2f;
                    _x += (int)velocityX;
                    _y += (int)velocityY;
                    UpdateHitbox();

                    if (Form1.GlobalPlatforms != null)
                    {
                        foreach (var platform in Form1.GlobalPlatforms)
                        {
                            if (this._hitbox.IntersectsWith(platform) && velocityY > 0)
                            {
                                _y = platform.Top - _height + 14;
                                velocityY = 0;
                                velocityX = 0;
                                IsGrounded = true;
                                UpdateHitbox();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    HurtTimer++;
                }
                return;
            }

            if (IsHurt)
            {
                HurtTimer--;
                _x += (int)velocityX;
                UpdateHitbox();
                velocityX *= 0.80f;

                if (HurtTimer <= 0) IsHurt = false;
                return;
            }

            if (attackCooldown > 0) attackCooldown--;

            bool isSamePlatform = Math.Abs((_y + _height) - (player.Y + player.Height)) < 60;
            int distanceToPlayer = Math.Abs((_x + _width / 2) - (player.X + player.Width / 2));

            if (isSamePlatform && distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer <= attackRange)
                {
                    if (attackCooldown == 0 && !IsEnemyAttacking)
                    {
                        IsEnemyAttacking = true;
                        EnemyAttackTimer = 12;
                        attackCooldown = 60;

                        if (player.X + player.Width / 2 > _x + _width / 2) direction = 1;
                        else direction = -1;

                        if (direction == 1)
                            EnemyAttackHitbox = new Rectangle(_x + _width, _y + 20, attackRange, _height - 40);
                        else
                            EnemyAttackHitbox = new Rectangle(_x - attackRange, _y + 20, attackRange, _height - 40);
                    }
                }
                else
                {
                    IsEnemyAttacking = false;
                    if (player.X + player.Width / 2 > _x + _width / 2)
                    {
                        _x += speed;
                        direction = 1;
                    }
                    else
                    {
                        _x -= speed;
                        direction = -1;
                    }
                }
            }
            else
            {
                IsEnemyAttacking = false;
                _x += speed * direction;

                if (_x >= rightBound) direction = -1;
                else if (_x <= leftBound) direction = 1;
            }

            if (IsEnemyAttacking)
            {
                EnemyAttackTimer--;
                if (EnemyAttackTimer <= 0)
                {
                    IsEnemyAttacking = false;
                    EnemyAttackHitbox = Rectangle.Empty;
                }
            }

            UpdateHitbox();
        }

        public void Update(List<Rectangle> platforms)
        {
        }

        public void Draw(Graphics g)
        {
            if (IsDead)
            {
                var state = g.Save();
                g.TranslateTransform(_x + _width / 2, _y + _height / 2);
                g.RotateTransform(DeathRotation);

                int alpha = 255 - (HurtTimer * 3);
                if (alpha < 0) alpha = 0;

                if (Texture != null)
                {
                    using (System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes())
                    {
                        float[][] ptsArray = {
                            new float[] {1, 0, 0, 0, 0},
                            new float[] {0, 1, 0, 0, 0},
                            new float[] {0, 0, 1, 0, 0},
                            new float[] {0, 0, 0, (float)alpha/255f, 0},
                            new float[] {0, 0, 0, 0, 1}
                        };
                        ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(ptsArray));
                        g.DrawImage(Texture, new Rectangle(-_width / 2, -_height / 2, _width, _height), 0, 0, Texture.Width, Texture.Height, GraphicsUnit.Pixel, ia);
                    }
                }
                else
                {
                    using (SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(alpha, Color.Crimson)))
                    {
                        g.FillRectangle(fadeBrush, -_width / 2, -_height / 2, _width, _height);
                    }
                }

                g.Restore(state);
                return;
            }

            if (Texture != null)
            {
                Rectangle drawRect = new Rectangle(_x, _y, _width, _height);

                if (direction == -1)
                {
                    GraphicsState state = g.Save();
                    g.TranslateTransform(drawRect.X + drawRect.Width, drawRect.Y);
                    g.ScaleTransform(-1, 1);
                    g.DrawImage(Texture, 0, 0, drawRect.Width, drawRect.Height);
                    g.Restore(state);
                }
                else
                {
                    g.DrawImage(Texture, drawRect);
                }

                if (IsHurt && HurtTimer % 4 < 2)
                {
                    using (SolidBrush flashBrush = new SolidBrush(Color.FromArgb(150, Color.Red)))
                    {
                        g.FillRectangle(flashBrush, drawRect);
                    }
                }
            }
            else
            {
                g.FillRectangle(IsHurt ? Brushes.Red : Brushes.Crimson, Hitbox);
            }
        }
    }
}