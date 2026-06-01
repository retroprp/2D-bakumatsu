using System;
using System.Drawing;

namespace KeserKnight.Entity
{
    public class RoyalShieldKnight
    {
        private int baseOriginX;
        private Random rand = new Random();

        // Gorsel Baglantilari
        private Image idleSheet = Properties.Resources.Idle;
        private Image walkSheet = Properties.Resources.Walk;
        private Image deparSheet = Properties.Resources.Depar;
        private Image shieldBashSheet = Properties.Resources.Shield_Bash;
        private Image shieldSlamSheet = Properties.Resources.Shield_Slam;

        // Temel Boss Nitelikleri
        public Rectangle Hitbox { get; private set; }
        public int MaxHealth { get; private set; } = 150;
        public int CurrentHealth { get; private set; } = 150;
        public bool IsDead { get; private set; } = false;
        public int HurtTimer { get; private set; } = 0;
        public int BlockTimer { get; private set; } = 0;

        // Form1 Erisim Koordinatlari
        public int X
        {
            get => Hitbox.X;
            set => Hitbox = new Rectangle(value, Hitbox.Y, Hitbox.Width, Hitbox.Height);
        }
        public int Y
        {
            get => Hitbox.Y;
            set => Hitbox = new Rectangle(Hitbox.X, value, Hitbox.Width, Hitbox.Height);
        }
        public int Width => Hitbox.Width;
        public int Height => Hitbox.Height;

        // Yon ve Hareket Sabitleri
        public bool FacingLeft { get; private set; } = true;
        private int walkSpeed = 4;
        private int chargeSpeed = 19;
        private int knockbackVelocity = 0;

        // Durum Makinesi Safhalari
        public enum BossState { WalkTowardPlayer, ShieldBashCharge, ShieldBashStrike, ShieldSlamCharge, ShieldSlamStrike, ChargePrep, ChargeRunning, ChargeRecovery, Stunned, DefenseStance, Dead }
        public BossState CurrentState { get; private set; } = BossState.WalkTowardPlayer;

        private int stateTimer = 0;
        private int cooldownTimer = 20;
        private int attackRange = 150;
        private int targetWidth = 1920;

        // Saldiri Alanlari
        public Rectangle BashHitbox { get; private set; }
        public Rectangle SlamHitbox { get; private set; }

        public bool IsStriking { get; private set; } = false;
        public bool IsSlamming { get; private set; } = false;
        public bool IsCharging { get; private set; } = false;

        public bool SequenceFinished { get; private set; } = false;

        public RoyalShieldKnight(int x, int y, int width, int height)
        {
            Hitbox = new Rectangle(x, y, width, height);
            this.baseOriginX = x;
        }

        public void Update(Player player)
        {
            if (SequenceFinished) return;

            if (HurtTimer > 0) HurtTimer--;
            if (BlockTimer > 0) BlockTimer--;

            if (knockbackVelocity > 0)
            {
                int direction = FacingLeft ? 1 : -1;
                Hitbox = new Rectangle(Hitbox.X + (direction * knockbackVelocity), Hitbox.Y, Hitbox.Width, Hitbox.Height);
                knockbackVelocity--;
            }

            stateTimer++;

            switch (CurrentState)
            {
                case BossState.Dead:
                    IsStriking = false; IsSlamming = false; IsCharging = false;
                    BashHitbox = Rectangle.Empty; SlamHitbox = Rectangle.Empty;
                    if (stateTimer >= 60) SequenceFinished = true;
                    return;

                case BossState.WalkTowardPlayer:
                    BashHitbox = Rectangle.Empty; SlamHitbox = Rectangle.Empty;
                    IsStriking = false; IsSlamming = false; IsCharging = false;

                    FacingLeft = (player.Hitbox.X + player.Hitbox.Width / 2) < (Hitbox.X + Hitbox.Width / 2);
                    int distanceToPlayer = Math.Abs((player.Hitbox.X + player.Hitbox.Width / 2) - (Hitbox.X + Hitbox.Width / 2));

                    if (distanceToPlayer > attackRange)
                    {
                        if (cooldownTimer > 0)
                        {
                            cooldownTimer--;
                        }
                        else
                        {
                            if (stateTimer > 30 && rand.Next(100) < 4)
                            {
                                stateTimer = 0;
                                CurrentState = BossState.ChargePrep;
                                break;
                            }

                            int moveDir = FacingLeft ? -walkSpeed : walkSpeed;
                            Hitbox = new Rectangle(Hitbox.X + moveDir, Hitbox.Y, Hitbox.Width, Hitbox.Height);
                        }
                    }
                    else
                    {
                        stateTimer = 0;
                        int choice = rand.Next(100);
                        if (choice < 25)
                            CurrentState = BossState.DefenseStance;
                        else if (choice < 60)
                            CurrentState = BossState.ShieldSlamCharge;
                        else
                            CurrentState = BossState.ShieldBashCharge;
                    }
                    break;

                case BossState.DefenseStance:
                    FacingLeft = (player.Hitbox.X + player.Hitbox.Width / 2) < (Hitbox.X + Hitbox.Width / 2);
                    BlockTimer = 5;
                    if (stateTimer >= 45)
                    {
                        stateTimer = 0;
                        CurrentState = rand.Next(100) < 50 ? BossState.ChargePrep : BossState.ShieldSlamCharge;
                    }
                    break;

                case BossState.Stunned:
                    IsStriking = false; IsSlamming = false; IsCharging = false;
                    BashHitbox = Rectangle.Empty; SlamHitbox = Rectangle.Empty;
                    if (stateTimer >= 45)
                    {
                        stateTimer = 0;
                        cooldownTimer = 25;
                        CurrentState = BossState.WalkTowardPlayer;
                    }
                    break;

                case BossState.ShieldSlamCharge:
                    if (stateTimer >= 15)
                    {
                        stateTimer = 0;
                        CurrentState = BossState.ShieldSlamStrike;
                    }
                    break;

                case BossState.ShieldSlamStrike:
                    IsSlamming = true;
                    int slamShift = FacingLeft ? -5 : 5;
                    Hitbox = new Rectangle(Hitbox.X + slamShift, Hitbox.Y, Hitbox.Width, Hitbox.Height);

                    if (FacingLeft)
                        SlamHitbox = new Rectangle(Hitbox.X - 70, Hitbox.Y + 20, 70, Hitbox.Height - 20);
                    else
                        SlamHitbox = new Rectangle(Hitbox.Right, Hitbox.Y + 20, 70, Hitbox.Height - 20);

                    if (stateTimer >= 12)
                    {
                        stateTimer = 0;
                        SlamHitbox = Rectangle.Empty;
                        IsSlamming = false;
                        CurrentState = BossState.Stunned;
                    }
                    break;

                case BossState.ShieldBashCharge:
                    if (stateTimer >= 20)
                    {
                        stateTimer = 0;
                        CurrentState = BossState.ShieldBashStrike;
                    }
                    break;

                case BossState.ShieldBashStrike:
                    IsStriking = true;
                    int dashSpeed = FacingLeft ? -10 : 10;
                    Hitbox = new Rectangle(Hitbox.X + dashSpeed, Hitbox.Y, Hitbox.Width, Hitbox.Height);

                    if (FacingLeft)
                        BashHitbox = new Rectangle(Hitbox.X - 45, Hitbox.Y, 45, Hitbox.Height);
                    else
                        BashHitbox = new Rectangle(Hitbox.Right, Hitbox.Y, 45, Hitbox.Height);

                    if (stateTimer >= 18)
                    {
                        stateTimer = 0;
                        BashHitbox = Rectangle.Empty;
                        IsStriking = false;
                        CurrentState = BossState.Stunned;
                    }
                    break;

                case BossState.ChargePrep:
                    if (stateTimer >= 22)
                    {
                        stateTimer = 0;
                        CurrentState = BossState.ChargeRunning;
                    }
                    break;

                case BossState.ChargeRunning:
                    IsCharging = true;
                    int runDir = FacingLeft ? -chargeSpeed : chargeSpeed;
                    Hitbox = new Rectangle(Hitbox.X + runDir, Hitbox.Y, Hitbox.Width, Hitbox.Height);

                    bool hitLeftWall = Hitbox.Left <= 10;
                    bool hitRightWall = Hitbox.Right >= (targetWidth - 10);

                    if (hitLeftWall || hitRightWall)
                    {
                        stateTimer = 0;
                        IsCharging = false;
                        BlockTimer = 15;
                        knockbackVelocity = 6;

                        if (hitLeftWall) Hitbox = new Rectangle(12, Hitbox.Y, Hitbox.Width, Hitbox.Height);
                        if (hitRightWall) Hitbox = new Rectangle(targetWidth - Hitbox.Width - 12, Hitbox.Y, Hitbox.Width, Hitbox.Height);

                        CurrentState = BossState.ChargeRecovery;
                    }
                    break;

                case BossState.ChargeRecovery:
                    IsCharging = false;
                    if (stateTimer >= 55)
                    {
                        stateTimer = 0;
                        cooldownTimer = 45;
                        CurrentState = BossState.WalkTowardPlayer;
                    }
                    break;
            }

            if (Hitbox.X < 10) Hitbox = new Rectangle(10, Hitbox.Y, Hitbox.Width, Hitbox.Height);
            if (Hitbox.Right > 1910) Hitbox = new Rectangle(1910 - Hitbox.Width, Hitbox.Y, Hitbox.Width, Hitbox.Height);
        }

        public bool TakeDamage(int amount, bool hitFromFront)
        {
            if (IsDead || HurtTimer > 0) return false;

            if (CurrentState == BossState.Stunned || CurrentState == BossState.ChargeRecovery)
            {
                CurrentHealth -= amount;
                HurtTimer = 15;
                knockbackVelocity = 1;
                return true;
            }

            if (hitFromFront && (CurrentState == BossState.DefenseStance || CurrentState == BossState.ShieldBashCharge || CurrentState == BossState.ShieldSlamCharge || CurrentState == BossState.ChargePrep))
            {
                BlockTimer = 10;
                knockbackVelocity = 2;
                return false;
            }

            CurrentHealth -= amount;
            HurtTimer = 15;
            knockbackVelocity = 7;

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                IsDead = true;
                CurrentState = BossState.Dead;
                stateTimer = 0;
            }

            return true;
        }

        public void Draw(Graphics g)
        {
            if (SequenceFinished) return;
            if (HurtTimer > 0 && (HurtTimer % 4 < 2)) return;

            // Duruma gore gorsel secimi
            Image activeSheet = idleSheet;
            if (CurrentState == BossState.WalkTowardPlayer && cooldownTimer == 0)
            {
                activeSheet = walkSheet;
            }
            else if (CurrentState == BossState.ChargePrep || CurrentState == BossState.ChargeRunning || CurrentState == BossState.ShieldBashCharge)
            {
                activeSheet = deparSheet;
            }
            else if (CurrentState == BossState.ShieldBashStrike)
            {
                activeSheet = shieldBashSheet;
            }
            else if (CurrentState == BossState.ShieldSlamStrike)
            {
                activeSheet = shieldSlamSheet;
            }

            if (activeSheet != null)
            {
                if (FacingLeft)
                {
                    System.Drawing.Drawing2D.GraphicsState state = g.Save();
                    g.TranslateTransform(Hitbox.X + Hitbox.Width, Hitbox.Y);
                    g.ScaleTransform(-1, 1);
                    g.DrawImage(activeSheet, 0, 0, Hitbox.Width, Hitbox.Height);
                    g.Restore(state);
                }
                else
                {
                    g.DrawImage(activeSheet, Hitbox);
                }
            }
            else
            {
                Brush bodyBrush = Brushes.DarkSlateGray;
                if (CurrentState == BossState.DefenseStance) bodyBrush = Brushes.SteelBlue;
                if (CurrentState == BossState.ShieldBashCharge) bodyBrush = Brushes.Crimson;
                if (CurrentState == BossState.ShieldSlamCharge) bodyBrush = Brushes.DarkOrange;
                if (CurrentState == BossState.ChargePrep) bodyBrush = Brushes.Purple;
                if (CurrentState == BossState.ChargeRunning) bodyBrush = Brushes.DarkRed;
                if (CurrentState == BossState.ChargeRecovery) bodyBrush = Brushes.Gold;
                if (CurrentState == BossState.Stunned) bodyBrush = Brushes.LightGray;

                if (CurrentState == BossState.Dead)
                {
                    bodyBrush = (stateTimer % 4 < 2) ? Brushes.White : Brushes.SteelBlue;
                }
                g.FillRectangle(bodyBrush, Hitbox);
            }

            int eyeW = 15; int eyeH = 6;
            int eyeX = FacingLeft ? Hitbox.X + 15 : Hitbox.Right - 30;
            int eyeY = Hitbox.Y + 20;
            g.FillRectangle(Brushes.Red, eyeX, eyeY, eyeW, eyeH);

            int shieldW = 18; int shieldH = Hitbox.Height - 30;
            int shieldX = FacingLeft ? Hitbox.X - 5 : Hitbox.Right - 13;
            int shieldY = Hitbox.Y + 20;

            if (CurrentState == BossState.ShieldSlamCharge) shieldY = Hitbox.Y - 15;
            else if (CurrentState == BossState.ShieldSlamStrike) shieldY = Hitbox.Y + 40;
            else if (CurrentState == BossState.ChargePrep || CurrentState == BossState.ChargeRunning) shieldX = FacingLeft ? Hitbox.X - 12 : Hitbox.Right + 2;
            else if (CurrentState == BossState.Stunned) shieldY = Hitbox.Y + 50;

            Brush shieldBrush = BlockTimer > 0 ? Brushes.White : Brushes.Silver;
            g.FillRectangle(shieldBrush, shieldX, shieldY, shieldW, shieldH);
            g.DrawRectangle(Pens.Black, shieldX, shieldY, shieldW, shieldH);

            if (CurrentState == BossState.ChargeRecovery || CurrentState == BossState.Stunned || CurrentState == BossState.Dead)
            {
                int starY = Hitbox.Y - 20;
                g.FillRectangle(Brushes.White, Hitbox.X + 20, starY, 8, 8);
                g.FillRectangle(Brushes.White, Hitbox.X + 40, starY - 10, 8, 8);
                g.FillRectangle(Brushes.White, Hitbox.X + 60, starY, 8, 8);
            }

            if (CurrentState == BossState.ShieldBashStrike && IsStriking)
            {
                int lineX = FacingLeft ? Hitbox.X - 30 : Hitbox.Right + 10;
                g.DrawLine(Pens.White, lineX, Hitbox.Y + 40, lineX + 20, Hitbox.Y + 40);
            }

            if (CurrentState == BossState.ShieldSlamStrike && IsSlamming)
            {
                int shockX = FacingLeft ? Hitbox.X - 35 : Hitbox.Right + 15;
                g.DrawLine(Pens.Orange, shockX, Hitbox.Bottom, shockX, Hitbox.Bottom - 60);
            }

            int barWidth = 500; int barHeight = 20; int barX = (1920 - barWidth) / 2; int barY = 120;
            g.FillRectangle(Brushes.Black, barX, barY, barWidth, barHeight);
            float healthRatio = (float)CurrentHealth / MaxHealth;
            int currentBarWidth = (int)(barWidth * healthRatio);
            using (SolidBrush healthBrush = new SolidBrush(Color.FromArgb(200, 180, 40))) g.FillRectangle(healthBrush, barX, barY, currentBarWidth, barHeight);
            using (Pen borderPen = new Pen(Color.White, 3f)) g.DrawRectangle(borderPen, barX, barY, barWidth, barHeight);
            using (Font font = new Font("Impact", 16, FontStyle.Regular)) g.DrawString("ROYAL SHIELD KNIGHT", font, Brushes.White, barX, barY - 30);
        }
    }
}