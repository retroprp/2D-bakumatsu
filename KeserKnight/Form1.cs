using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using KeserKnight.Entity;
using KeserKnight.Map;
using KeserKnight.UI;
using KeserKnight.Core;
using KeserKnight.Combat;

namespace KeserKnight
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        // --- 🎥 PÜRÜZSÜZ RETRO KAYMA MOTORU DEĞİŞKENLERİ ---
        private float cameraX = 0;
        private float cameraY = 0;
        private float targetCameraX = 0;
        private float targetCameraY = 0;
        private bool isCameraScrolling = false;
        private float cameraScrollSpeed = 0.08f; // Klasik Shovel Knight/Mega Man yumuşaklığı için ideal ritim

        private int transitionDirectionX = 0;
        private int transitionDirectionY = 0;
        private Bitmap oldRoomSnapshot = null;
        private Bitmap newRoomSnapshot = null;

        private Bitmap virtualCanvas;
        private Graphics canvasGraphics;
        private int targetWidth = 1920;
        private int targetHeight = 1080;

        Player player;
        PhysicsEngine physicsEngine;
        AttackSystem attackSystem;
        RoomManager roomManager;
        InputManager inputManager;

        List<Rectangle> platforms = new List<Rectangle>();
        List<Enemy> enemies = new List<Enemy>();
        List<Gold> roomGolds = new List<Gold>();
        List<BreakableBlock> breakableBlocks = new List<BreakableBlock>();
        List<TimedBlock> timedBlocks = new List<TimedBlock>();
        List<MovingPlatform> movingPlatforms = new List<MovingPlatform>();
        List<Rectangle> roomLadders = new List<Rectangle>();

        CheckpointTorch checkpointTorch;
        GuardianGriffon miniBoss;
        RoyalShieldKnight shieldKnight;
        CrimsonKnight crimsonKnight;

        public static List<Rectangle> GlobalPlatforms;
        bool isGameOver = false;
        int totalGold = 0;
        DateTime lastF11Time = DateTime.MinValue;
        int menuSelection = 0;
        int pauseSelection = 0;
        bool isGodMode = false;
        bool isFlyMode = false;

        int lastCheckpointRoom = 1;
        int lastCheckpointX = 300;
        int lastCheckpointY = 750;

        private DateTime gameStartTime;
        private TimeSpan finalCompletionTime;
        private int totalDeaths = 0;
        private bool isEndingTriggered = false;
        private int endingTimer = 0;
        private int fadeAlpha = 0;

        private bool hasSaveFile = false;

        private int[] selectAncorRooms = { 3, 6, 9, 12, 13, 14, 15 };
        private int selectRoomIndex = 0;

        // --- 🛡️ BİRBİRİNDEN BAĞIMSIZ TUŞ COOLDOWN SAYAÇLARI ---
        private int lAttackCooldownTimer = 0;

        // --- FINAL BOSS ARENA ÇÖKME (COLLAPSE) SİSTEMİ ---
        private int cameraShakeTimer = 0;
        private int collapseVisualY = 870;
        private bool arenaBroken = false;

        private List<ParallaxLayer> currentBgLayers;
        public enum GameState { MainMenu, LevelSelect, Playing, Paused, Victory }
        public GameState currentGameState = GameState.MainMenu;

        Rectangle continueButton = new Rectangle(810, 420, 300, 60);
        Rectangle startButton = new Rectangle(810, 500, 300, 60);
        Rectangle exitButton = new Rectangle(810, 600, 300, 60);
        Rectangle resumeButton = new Rectangle(810, 450, 300, 60);
        Rectangle settingsButton = new Rectangle(810, 540, 300, 60);
        Rectangle mainMenuButton = new Rectangle(810, 630, 300, 60);

        Image playerImage = Properties.Resources.anakarakter;
        Image kalpDolu = Properties.Resources.kalp_dolu;
        Image kalpBos = Properties.Resources.kalp_bos;

        public Form1()
        {
            InitializeComponent();
            GameMap.InitializeWorld();
            virtualCanvas = new Bitmap(targetWidth, targetHeight);
            canvasGraphics = Graphics.FromImage(virtualCanvas);
            SetScreenMode(true);

            player = new Player(300, 750, 130, 130,
                Properties.Resources.idle_sheet,
                Properties.Resources.ninja_run,
                Properties.Resources.ninja_jump,
                Properties.Resources.ninja_crouch,
                Properties.Resources.ninja_attack);

            physicsEngine = new PhysicsEngine();
            attackSystem = new AttackSystem();
            roomManager = new RoomManager(targetWidth, targetHeight);
            inputManager = new InputManager();
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.Focus();

            hasSaveFile = File.Exists("savegame.dat");
            if (hasSaveFile)
            {
                startButton = new Rectangle(810, 510, 300, 60);
            }

            SyncLoadRoom();
        }

        private void LoadRoomBackground(int roomNumber)
        {
            if (currentBgLayers == null) currentBgLayers = new List<ParallaxLayer>();
            currentBgLayers.Clear();

            if (roomNumber >= 1 && roomNumber <= 7)
            {
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.mist_bg_base, 0.0f, 0, false));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.mist_bg_trees, 0.2f, 100));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.mist_fg_tree, 0.4f, 50));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.mist_fg_rocks, 0.6f, targetHeight - 300));
            }
            else if (roomNumber == 8)
            {
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.mist_bg_base, 0.0f, 0, false));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.mist_bg_trees, 0.1f, 150));
            }
            else if (roomNumber == 9 || roomNumber == 10)
            {
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.bg_sky, 0.0f, 0, false));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.bg_mountains_far, 0.1f, 200));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.mist_fg_rocks, 0.3f, targetHeight - 400));
            }
            else if (roomNumber == 11 || roomNumber == 12)
            {
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.bg_sky, 0.0f, 0, false));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.bg_mountains_far, 0.1f, 100));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.bg_mountains_town, 0.2f, 150));
            }
            else if (roomNumber >= 13)
            {
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.bg_sky, 0.0f, 0, false));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.bg_city_buildings, 0.3f, 0));
                if (roomNumber >= 14) currentBgLayers.Add(new ParallaxLayer(Properties.Resources.dec_city_props, 0.5f, 50));
                currentBgLayers.Add(new ParallaxLayer(Properties.Resources.bg_clouds, 0.7f, 30));
            }
        }

        void SyncLoadRoom()
        {
            List<Rectangle> loadedLadders = new List<Rectangle>();
            GameMap.LoadRoom(roomManager.CurrentRoom, platforms, enemies, roomGolds, breakableBlocks, timedBlocks, movingPlatforms, out loadedLadders);

            roomLadders.Clear();
            if (loadedLadders != null)
            {
                roomLadders.AddRange(loadedLadders);
            }

            if (roomManager.CurrentRoom == 9)
            {
                checkpointTorch = new CheckpointTorch(200, 770, 35, 80);
                lastCheckpointRoom = 9; lastCheckpointX = 200; lastCheckpointY = 750;
                SaveProgress();
            }
            else if (roomManager.CurrentRoom == 11)
            {
                checkpointTorch = new CheckpointTorch(350, 570, 35, 80);
                lastCheckpointRoom = 11; lastCheckpointX = 250; lastCheckpointY = 550;
                SaveProgress();
            }
            else if (roomManager.CurrentRoom == 14)
            {
                checkpointTorch = new CheckpointTorch(740, 570, 35, 140);
                lastCheckpointRoom = 14; lastCheckpointX = 740; lastCheckpointY = 550;
                SaveProgress();
            }
            else
            {
                checkpointTorch = null;
            }

            LoadRoomBackground(roomManager.CurrentRoom);

            miniBoss = GameMap.GetBossInstance(roomManager.CurrentRoom);
            shieldKnight = (roomManager.CurrentRoom == 12) ? new RoyalShieldKnight(1400, 700, 90, 130) : null;
            crimsonKnight = (roomManager.CurrentRoom == 15) ? new CrimsonKnight(1500, 740, 90, 130) : null;
        }

        void SaveProgress()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("savegame.dat"))
                {
                    sw.WriteLine(lastCheckpointRoom);
                    sw.WriteLine(lastCheckpointX);
                    sw.WriteLine(lastCheckpointY);
                    sw.WriteLine(totalGold);
                    sw.WriteLine(totalDeaths);
                }
                hasSaveFile = true;
            }
            catch { }
        }

        void LoadProgress()
        {
            try
            {
                if (File.Exists("savegame.dat"))
                {
                    using (StreamReader sr = new StreamReader("savegame.dat"))
                    {
                        lastCheckpointRoom = int.Parse(sr.ReadLine());
                        lastCheckpointX = int.Parse(sr.ReadLine());
                        lastCheckpointY = int.Parse(sr.ReadLine());
                        totalGold = int.Parse(sr.ReadLine());
                        totalDeaths = int.Parse(sr.ReadLine());
                    }
                    gameStartTime = DateTime.Now;
                    ResetGame();
                    currentGameState = GameState.Playing;
                }
            }
            catch { }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                ResetGame();
                return;
            }

            if (currentGameState == GameState.Victory)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                {
                    currentGameState = GameState.MainMenu;
                    menuSelection = 0;
                    totalDeaths = 0;
                    hasSaveFile = File.Exists("savegame.dat");
                    if (hasSaveFile) startButton = new Rectangle(810, 510, 300, 60);
                    else startButton = new Rectangle(810, 500, 300, 60);
                    this.Invalidate();
                }
                return;
            }

            if (e.KeyCode == Keys.F11)
            {
                if ((DateTime.Now - lastF11Time).TotalMilliseconds < 800) return;
                lastF11Time = DateTime.Now;
                if (this.FormBorderStyle == FormBorderStyle.None) SetScreenMode(false); else SetScreenMode(true);
                return;
            }
            if (e.KeyCode == Keys.G) { if (currentGameState == GameState.Playing) { isGodMode = !isGodMode; UpdateWindowTitle(); } return; }
            if (e.KeyCode == Keys.H) { if (currentGameState == GameState.Playing) { isFlyMode = !isFlyMode; player.VerticalVelocity = 0; UpdateWindowTitle(); } return; }

            if (isGameOver && e.KeyCode != Keys.R) return;

            if (currentGameState == GameState.MainMenu)
            {
                if (e.KeyCode == Keys.L)
                {
                    currentGameState = GameState.LevelSelect;
                    selectRoomIndex = 0;
                    this.Invalidate();
                    return;
                }
                inputManager.HandleMenuInput(e, ref menuSelection, () => { if (File.Exists("savegame.dat")) File.Delete("savegame.dat"); hasSaveFile = false; startButton = new Rectangle(810, 500, 300, 60); lastCheckpointRoom = 1; lastCheckpointX = 300; lastCheckpointY = 750; gameStartTime = DateTime.Now; ResetGame(); currentGameState = GameState.Playing; this.Focus(); });
                this.Invalidate();
                return;
            }

            if (currentGameState == GameState.LevelSelect)
            {
                if (e.KeyCode == Keys.Escape) { currentGameState = GameState.MainMenu; this.Invalidate(); return; }

                if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D) selectRoomIndex = (selectRoomIndex + 1) % selectAncorRooms.Length;
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A) selectRoomIndex = (selectRoomIndex - 1 + selectAncorRooms.Length) % selectAncorRooms.Length;

                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                {
                    int targetRoom = selectAncorRooms[selectRoomIndex];
                    gameStartTime = DateTime.Now;
                    ResetGameToRoom(targetRoom);
                    currentGameState = GameState.Playing;
                }
                this.Invalidate();
                return;
            }

            if (currentGameState != GameState.Playing) return;
            if (player.KnockbackTimer > 0) return;

            bool isSpaceGroup = (e.KeyCode == Keys.Space || e.KeyCode == Keys.Z || e.KeyCode == Keys.J);
            bool isLKey = (e.KeyCode == Keys.L);

            if (isSpaceGroup && player.AttackCooldownTimer > 0) return;
            if (isLKey && lAttackCooldownTimer > 0) return;

            if (isSpaceGroup || isLKey)
            {
                inputManager.HandleGameKeyDown(e, player, attackSystem, isGameOver, () => ResetGame());
            }

            if (player.IsAttacking)
            {
                if (isSpaceGroup && player.AttackCooldownTimer == 0)
                {
                    player.AttackCooldownTimer = 78;
                }
                else if (isLKey && lAttackCooldownTimer == 0)
                {
                    lAttackCooldownTimer = 12;
                }
            }
        }

        void ResetGameToRoom(int roomNumber)
        {
            isGameOver = false;
            totalGold = 0;
            GameMap.ResetWorld();
            roomManager.Reset();

            typeof(RoomManager).GetProperty("CurrentRoom").SetValue(roomManager, roomNumber);

            int startY = 750;
            if (roomNumber == 3) startY = 650 - player.Height - 10;
            else if (roomNumber == 6) startY = 250 - player.Height - 10;
            else if (roomNumber == 9) startY = 850 - player.Height - 10;
            else if (roomNumber == 12) startY = 840 - player.Height - 10;
            else if (roomNumber == 13) startY = 480 - player.Height - 10;
            else if (roomNumber == 14) startY = 870 - player.Height - 10;
            else if (roomNumber == 15) startY = 870 - player.Height - 10;

            player.Reset(100, startY);
            player.CurrentHealth = 5;
            SyncLoadRoom();
            this.Focus();
        }

        private void UpdateWindowTitle()
        {
            string title = "KeserKnight";
            if (isGodMode) title += " - [OLUMSUZLUK AKTIF]";
            if (isFlyMode) title += " - [UCMA AKTIF]";
            this.Text = title;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (currentGameState == GameState.Playing && player.KnockbackTimer <= 0)
                inputManager.HandleGameKeyUp(e, player);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            float scaleX = (float)this.ClientSize.Width / targetWidth; float scaleY = (float)this.ClientSize.Height / targetHeight;
            Point virtualClickPoint = new Point((int)(e.X / scaleX), (int)(e.Y / scaleY));

            if (currentGameState == GameState.MainMenu)
            {
                if (hasSaveFile && continueButton.Contains(virtualClickPoint))
                {
                    LoadProgress();
                }
                else if (startButton.Contains(virtualClickPoint))
                {
                    if (File.Exists("savegame.dat")) File.Delete("savegame.dat");
                    hasSaveFile = false;
                    startButton = new Rectangle(810, 500, 300, 60);
                    lastCheckpointRoom = 1; lastCheckpointX = 300; lastCheckpointY = 750;
                    gameStartTime = DateTime.Now;
                    ResetGame();
                    currentGameState = GameState.Playing;
                    this.Focus();
                }
                else if (exitButton.Contains(virtualClickPoint)) Application.Exit();
            }
            else if (currentGameState == GameState.Paused)
            {
                if (resumeButton.Contains(virtualClickPoint)) currentGameState = GameState.Playing;
                else if (mainMenuButton.Contains(virtualClickPoint)) { currentGameState = GameState.MainMenu; menuSelection = 0; }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentGameState != GameState.Playing) { this.Invalidate(); return; }
            if (isGameOver) return;

            // --- 🎥 PÜRÜZSÜZ RETRO KAMERA KAYDIRMA MOTORU (TICK DÖNGÜSÜ) ---
            if (isCameraScrolling)
            {
                cameraX += (targetCameraX - cameraX) * cameraScrollSpeed;
                cameraY += (targetCameraY - cameraY) * cameraScrollSpeed;

                player.X += transitionDirectionX * 14;
                player.Y += transitionDirectionY * 10;
                player.Update();

                if (Math.Abs(targetCameraX - cameraX) < 12 && Math.Abs(targetCameraY - cameraY) < 12)
                {
                    isCameraScrolling = false;
                    cameraX = 0; cameraY = 0;
                    targetCameraX = 0; targetCameraY = 0;

                    if (transitionDirectionX == 1) player.X = 20;
                    else if (transitionDirectionX == -1) player.X = targetWidth - player.Width - 20;
                    else if (transitionDirectionY == 1) player.Y = 20;
                    else if (transitionDirectionY == -1) player.Y = targetHeight - player.Height - 20;

                    if (oldRoomSnapshot != null) { oldRoomSnapshot.Dispose(); oldRoomSnapshot = null; }
                    if (newRoomSnapshot != null) { newRoomSnapshot.Dispose(); newRoomSnapshot = null; }

                    SyncLoadRoom();
                }

                this.Invalidate();
                return;
            }

            // --- 🧭 ODA DEĞİŞİM SINIR TETİKLEYİCİLERİ (BURADA OLMALI) ---
            int nextRoom = roomManager.CurrentRoom;
            transitionDirectionX = 0;
            transitionDirectionY = 0;

            if (player.X + player.Width > targetWidth) { nextRoom = roomManager.CurrentRoom + 1; targetCameraX = targetWidth; targetCameraY = 0; transitionDirectionX = 1; }
            else if (player.X < 0) { nextRoom = roomManager.CurrentRoom - 1; targetCameraX = -targetWidth; targetCameraY = 0; transitionDirectionX = -1; }
            else if (player.Y + player.Height > targetHeight) { nextRoom = roomManager.CurrentRoom + 3; targetCameraX = 0; targetCameraY = targetHeight; transitionDirectionY = 1; }
            else if (player.Y < 0) { nextRoom = roomManager.CurrentRoom - 3; targetCameraX = 0; targetCameraY = -targetHeight; transitionDirectionY = -1; }

            if (nextRoom != roomManager.CurrentRoom && nextRoom > 0 && nextRoom <= 15)
            {
                oldRoomSnapshot = new Bitmap(virtualCanvas);
                GameMap.SaveRoomState(roomManager.CurrentRoom, enemies, roomGolds, breakableBlocks);

                isCameraScrolling = true;
                cameraX = 0; cameraY = 0;
                typeof(RoomManager).GetProperty("CurrentRoom").SetValue(roomManager, nextRoom);
                SyncLoadRoom();

                PaintEventArgs fakePaint = new PaintEventArgs(Graphics.FromImage(virtualCanvas), this.ClientRectangle);
                Form1_Paint(this, fakePaint);
                newRoomSnapshot = new Bitmap(virtualCanvas);

                this.Invalidate();
                return;
            }

            // --- 🛡️ NORMAL OYUN VE FİZİK DÖNGÜSÜ ---
            if (lAttackCooldownTimer > 0) lAttackCooldownTimer--;

            bool downPressedGlobal = (GetAsyncKeyState((int)Keys.S) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Down) & 0x8000) != 0;
            bool upPressedGlobal = (GetAsyncKeyState((int)Keys.W) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Up) & 0x8000) != 0;

            if (isFlyMode)
            {
                int flySpeed = 12;
                if ((GetAsyncKeyState((int)Keys.W) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Up) & 0x8000) != 0) player.Y -= flySpeed;
                if ((GetAsyncKeyState((int)Keys.S) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Down) & 0x8000) != 0) player.Y += flySpeed;
                if ((GetAsyncKeyState((int)Keys.A) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Left) & 0x8000) != 0) player.X -= flySpeed;
                if ((GetAsyncKeyState((int)Keys.D) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Right) & 0x8000) != 0) player.X += flySpeed;
                player.VerticalVelocity = 0;

                List<Rectangle> tempLadders;
                if (roomManager.Update(player, platforms, enemies, roomGolds, breakableBlocks, timedBlocks, movingPlatforms, out tempLadders))
                {
                    roomLadders = tempLadders;
                }
                foreach (var tb in timedBlocks) tb.Update(player);
                if (checkpointTorch != null) checkpointTorch.Update();
                this.Invalidate(); return;
            }

            if (player.KnockbackTimer > 0) { player.MoveLeft = false; player.MoveRight = false; }
            else { player.MoveLeft = (GetAsyncKeyState((int)Keys.A) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Left) & 0x8000) != 0; player.MoveRight = (GetAsyncKeyState((int)Keys.D) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Right) & 0x8000) != 0; }

            bool isClimbing = false; bool isOnLadder = false;
            foreach (var ladder in roomLadders) { if (player.Hitbox.IntersectsWith(ladder)) { isOnLadder = true; break; } }
            if (isOnLadder && player.KnockbackTimer <= 0)
            {
                if (upPressedGlobal) { player.Y -= 7; player.VerticalVelocity = 0; isClimbing = true; }
                else if (downPressedGlobal) { player.Y += 7; player.VerticalVelocity = 0; }
                else if (player.VerticalVelocity > 0) player.VerticalVelocity = 0;
            }

            List<Rectangle> tempPhysicalPlatforms = new List<Rectangle>();
            if (!isClimbing)
            {
                tempPhysicalPlatforms.AddRange(platforms);
                foreach (var block in breakableBlocks) { if (!block.IsBroken) tempPhysicalPlatforms.Add(block.Hitbox); }
                foreach (var tBlock in timedBlocks) { if (tBlock.IsActive) tempPhysicalPlatforms.Add(tBlock.Hitbox); }
                foreach (var mp in movingPlatforms) tempPhysicalPlatforms.Add(mp.Hitbox);
            }
            else { foreach (var platform in platforms) { if (platform.Y >= player.Hitbox.Bottom - 5) tempPhysicalPlatforms.Add(platform); } }
            Form1.GlobalPlatforms = tempPhysicalPlatforms;

            player.IsCrouching = downPressedGlobal;
            if (player.IsCrouching && player.VerticalVelocity == 0) { player.MoveLeft = false; player.MoveRight = false; }

            player.Update();
            physicsEngine.Update(player, tempPhysicalPlatforms);

            foreach (var platform in tempPhysicalPlatforms)
            {
                if (platform.Height > 150 && player.Hitbox.IntersectsWith(platform))
                {
                    if (player.Hitbox.Right > platform.Left && player.Hitbox.Left < platform.Left) { player.X = platform.Left - player.Width + 30; }
                    else if (player.Hitbox.Left < platform.Right && player.Hitbox.Right > platform.Right) { player.X = platform.Right - 30; }
                }
            }

            if (player.Y > 1020) { if (isGodMode) { player.X = 150; player.Y = 100; player.VerticalVelocity = 0; } else { isGameOver = true; this.Invalidate(); return; } }

            foreach (var tBlock in timedBlocks) tBlock.Update(player);
            foreach (var mp in movingPlatforms) mp.Update(player);
            if (checkpointTorch != null) checkpointTorch.Update();

            if (player.AttackCooldownTimer > 0) player.AttackCooldownTimer--;

            attackSystem.UpdateAttackHitbox(player);

            bool pogoTriggeredThisTick = false;
            if (player.IsAttacking && !player.AttackHitbox.IsEmpty)
            {
                int facing = (player.CurrentDirection == Player.Direction.Right) ? 1 : -1;
                for (int b = breakableBlocks.Count - 1; b >= 0; b--) { var block = breakableBlocks[b]; if (!block.IsBroken && player.AttackHitbox.IntersectsWith(block.Hitbox)) { block.TakeDamage(10); player.VerticalVelocity = -15; if (player.CurrentDirection == Player.Direction.Left) player.X += 40; else player.X -= 40; break; } }

                foreach (var enemy in enemies)
                {
                    if (!enemy.IsDead && player.AttackHitbox.IntersectsWith(enemy.Hitbox))
                    {
                        if (player.Hitbox.Bottom <= enemy.Hitbox.Y + 45 && player.VerticalVelocity > 0 && downPressedGlobal)
                        {
                            enemy.TakeDamage(10, facing);
                            player.VerticalVelocity = -42;
                            player.IsJumping = true;
                            pogoTriggeredThisTick = true;
                            player.AttackCooldownTimer = 0;
                        }
                        else
                        {
                            enemy.TakeDamage(10, facing);
                            player.VerticalVelocity = -15;
                            if (player.CurrentDirection == Player.Direction.Left) player.X += 40; else player.X -= 40;
                        }
                        break;
                    }
                }
            }

            if (miniBoss != null)
            {
                miniBoss.Update(player);
                if (!miniBoss.IsDead)
                {
                    if (player.IsAttacking && !player.AttackHitbox.IsEmpty && player.AttackHitbox.IntersectsWith(miniBoss.Hitbox)) { miniBoss.TakeDamage(10); player.VerticalVelocity = -15; if (player.CurrentDirection == Player.Direction.Left) player.X += 40; else player.X -= 40; }
                    if (miniBoss.IsSwiping && !miniBoss.SwipeHitbox.IsEmpty && miniBoss.SwipeHitbox.IntersectsWith(player.Hitbox) && !isGodMode && !pogoTriggeredThisTick) { if (player.TakeDamage()) isGameOver = true; }
                    foreach (var proj in miniBoss.Projectiles) { if (proj.Hitbox.IntersectsWith(player.Hitbox) && !isGodMode) { if (player.TakeDamage()) isGameOver = true; break; } }
                    if (player.Hitbox.IntersectsWith(miniBoss.Hitbox) && !isGodMode && !pogoTriggeredThisTick) { if (player.TakeDamage()) isGameOver = true; }
                }
                else if (miniBoss.SequenceFinished && platforms.Count > 0)
                {
                    for (int p = platforms.Count - 1; p >= 0; p--) { if (platforms[p].X == 1750 && platforms[p].Width == 170) { platforms.RemoveAt(p); break; } }
                    Random prizeRand = new Random(); for (int i = 0; i < 5; i++) roomGolds.Add(new Gold(miniBoss.Hitbox.X + prizeRand.Next(-100, 100), miniBoss.Hitbox.Y + prizeRand.Next(50, 200), (i % 2 == 0) ? 50 : 10, (i % 2 == 0) ? Color.Cyan : Color.Gold)); miniBoss = null;
                }
            }

            if (shieldKnight != null)
            {
                shieldKnight.Update(player);
                if (!shieldKnight.IsDead)
                {
                    if (player.IsAttacking && !player.AttackHitbox.IsEmpty && player.AttackHitbox.IntersectsWith(shieldKnight.Hitbox))
                    {
                        int bossCenter = shieldKnight.Hitbox.X + shieldKnight.Hitbox.Width / 2;
                        bool hitFromFront = (player.X < bossCenter && shieldKnight.FacingLeft) || (player.X > bossCenter && !shieldKnight.FacingLeft);

                        if (player.Hitbox.Bottom <= shieldKnight.Hitbox.Y + 45 && player.VerticalVelocity > 0 && downPressedGlobal)
                        {
                            shieldKnight.TakeDamage(12, hitFromFront);
                            player.VerticalVelocity = -44;
                            player.IsJumping = true;
                            pogoTriggeredThisTick = true;
                            player.AttackCooldownTimer = 0;
                        }
                        else
                        {
                            shieldKnight.TakeDamage(10, hitFromFront);
                            player.VerticalVelocity = -15;
                            if (player.CurrentDirection == Player.Direction.Left) player.X += 40; else player.X -= 40;
                        }
                    }

                    if (shieldKnight.IsStriking && !shieldKnight.BashHitbox.IsEmpty && shieldKnight.BashHitbox.IntersectsWith(player.Hitbox) && !pogoTriggeredThisTick) { if (!isGodMode && player.TakeDamage()) isGameOver = true; }
                    if (shieldKnight.IsSlamming && !shieldKnight.SlamHitbox.IsEmpty && shieldKnight.SlamHitbox.IntersectsWith(player.Hitbox) && !pogoTriggeredThisTick) { if (!isGodMode && player.TakeDamage()) isGameOver = true; }
                    if (player.Hitbox.IntersectsWith(shieldKnight.Hitbox) && !isGodMode && !pogoTriggeredThisTick) { if (player.TakeDamage()) isGameOver = true; }
                }
                else if (shieldKnight.SequenceFinished && platforms.Count > 0)
                {
                    Random prizeRand = new Random(); for (int i = 0; i < 5; i++) roomGolds.Add(new Gold(shieldKnight.Hitbox.X + prizeRand.Next(-100, 100), shieldKnight.Hitbox.Y + prizeRand.Next(50, 150), (i % 2 == 0) ? 50 : 10, (i % 2 == 0) ? Color.Cyan : Color.Gold)); shieldKnight = null;
                }
            }

            if (crimsonKnight != null)
            {
                crimsonKnight.Update(player);

                if (crimsonKnight.CurrentState == CrimsonKnight.BossState.PhaseTransition)
                {
                    cameraShakeTimer = 5;
                    if (!arenaBroken)
                    {
                        arenaBroken = true;
                        platforms.RemoveAll(p => p.Y == 870 && p.Width == 1920);
                        platforms.Add(new Rectangle(300, 870, 1320, 210));
                    }
                    if (collapseVisualY < 1200) collapseVisualY += 8;
                }
                else
                {
                    cameraShakeTimer = 0;
                }

                if (!crimsonKnight.IsDead)
                {
                    if (player.IsAttacking && !player.AttackHitbox.IsEmpty && player.AttackHitbox.IntersectsWith(crimsonKnight.Hitbox))
                    {
                        if (player.Hitbox.Bottom <= crimsonKnight.Hitbox.Y + 45 && player.VerticalVelocity > 0 && downPressedGlobal)
                        {
                            crimsonKnight.TakeDamage(15);
                            player.VerticalVelocity = -42;
                            player.IsJumping = true;
                            pogoTriggeredThisTick = true;
                            player.AttackCooldownTimer = 0;
                        }
                        else
                        {
                            crimsonKnight.TakeDamage(10);
                            player.VerticalVelocity = -15;
                            if (player.CurrentDirection == Player.Direction.Left) player.X += 45; else player.X -= 45;
                        }
                    }

                    if (crimsonKnight.IsAttacking && !crimsonKnight.AttackHitbox.IsEmpty && crimsonKnight.AttackHitbox.IntersectsWith(player.Hitbox) && !pogoTriggeredThisTick) { if (!isGodMode && player.TakeDamage()) isGameOver = true; }

                    if (player.Hitbox.IntersectsWith(crimsonKnight.Hitbox) && !pogoTriggeredThisTick) { if (!isGodMode && player.TakeDamage()) isGameOver = true; }
                }
                else
                {
                    if (!isEndingTriggered)
                    {
                        isEndingTriggered = true; endingTimer = 0; fadeAlpha = 0;
                        finalCompletionTime = DateTime.Now - gameStartTime;
                    }
                    endingTimer++;
                    if (endingTimer <= 30) { this.Invalidate(); return; }

                    if (crimsonKnight.SequenceFinished)
                    {
                        fadeAlpha += 5;
                        if (fadeAlpha >= 255)
                        {
                            fadeAlpha = 255; currentGameState = GameState.Victory; crimsonKnight = null; isEndingTriggered = false;
                        }
                    }
                }
            }

            List<Rectangle> currentRoomLadders;
            bool roomChanged = roomManager.Update(player, platforms, enemies, roomGolds, breakableBlocks, timedBlocks, movingPlatforms, out currentRoomLadders);

            if (roomChanged && currentRoomLadders != null)
            {
                roomLadders = currentRoomLadders;
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i]; enemy.Update(platforms, player);
                if (enemy.IsDead && enemy.HurtTimer >= 85) { enemies.RemoveAt(i); continue; }
                if (pogoTriggeredThisTick) break;

                if (player.Hitbox.IntersectsWith(enemy.Hitbox))
                {
                    if (isGodMode) break;
                    if (player.TakeDamage()) isGameOver = true;
                    break;
                }
                if (enemy.IsEnemyAttacking && !enemy.EnemyAttackHitbox.IsEmpty && enemy.EnemyAttackHitbox.IntersectsWith(player.Hitbox))
                { if (isGodMode) break; if (player.TakeDamage()) isGameOver = true; break; }
            }

            for (int i = roomGolds.Count - 1; i >= 0; i--) { if (player.Hitbox.IntersectsWith(roomGolds[i].Hitbox)) { totalGold += roomGolds[i].Value; roomGolds.RemoveAt(i); } }
            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (canvasGraphics == null || virtualCanvas == null) return;

            // --- 🎬 SİNEMATİK GEÇİŞ ESNASINDA ARKA PLANDA ÇALIŞAN RENDER MOTORU ---
            if (isCameraScrolling && oldRoomSnapshot != null && newRoomSnapshot != null)
            {
                e.Graphics.Clear(Color.FromArgb(20, 24, 43));
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                int ox = (int)-cameraX;
                int oy = (int)-cameraY;

                if (transitionDirectionX == 1) // Sağa kayış
                {
                    e.Graphics.DrawImage(oldRoomSnapshot, ox, 0, this.ClientSize.Width, this.ClientSize.Height);
                    e.Graphics.DrawImage(newRoomSnapshot, targetWidth + ox, 0, this.ClientSize.Width, this.ClientSize.Height);
                }
                else if (transitionDirectionX == -1) // Sola kayış
                {
                    e.Graphics.DrawImage(oldRoomSnapshot, ox, 0, this.ClientSize.Width, this.ClientSize.Height);
                    e.Graphics.DrawImage(newRoomSnapshot, -targetWidth + ox, 0, this.ClientSize.Width, this.ClientSize.Height);
                }
                else if (transitionDirectionY == 1) // Aşağı kayış
                {
                    e.Graphics.DrawImage(oldRoomSnapshot, 0, oy, this.ClientSize.Width, this.ClientSize.Height);
                    e.Graphics.DrawImage(newRoomSnapshot, 0, targetHeight + oy, this.ClientSize.Width, this.ClientSize.Height);
                }
                else if (transitionDirectionY == -1) // Yukarı kayış
                {
                    e.Graphics.DrawImage(oldRoomSnapshot, 0, oy, this.ClientSize.Width, this.ClientSize.Height);
                    e.Graphics.DrawImage(newRoomSnapshot, 0, -targetHeight + oy, this.ClientSize.Width, this.ClientSize.Height);
                }
                return; // Geçiş esnasında alt katmanların mükerrer çizilmesini engelliyoruz
            }

            canvasGraphics.SmoothingMode = SmoothingMode.None;
            canvasGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            canvasGraphics.PixelOffsetMode = PixelOffsetMode.Half;

            // --- ZAFER EKRANI RENDERI ---
            if (currentGameState == GameState.Victory)
            {
                canvasGraphics.Clear(Color.FromArgb(12, 15, 28));
                using (Font titleFont = new Font("Impact", 64))
                using (Font subTitleFont = new Font("Arial Black", 20))
                using (Font statsFont = new Font("Courier New", 22, FontStyle.Bold))
                using (Font rankFont = new Font("Impact", 28))
                {
                    canvasGraphics.DrawString("USTALIK BELGESI ALINDI", titleFont, Brushes.Black, 494, 204);
                    canvasGraphics.DrawString("USTALIK BELGESI ALINDI", titleFont, Brushes.Goldenrod, 490, 200);

                    canvasGraphics.DrawString("TEBRIKLER! ELINDE KESER, SIRTINDA PELERINLE SANAYI TIPI", subTitleFont, Brushes.White, 290, 330);
                    canvasGraphics.DrawString("NINJA GIBI BUTUN KILITLERI KIRDIN, SANTIYEYI TESLIM ALDIN!", subTitleFont, Brushes.Cyan, 330, 380);

                    string durationStr = string.Format("{0:00}:{1:00}:{2:00}", finalCompletionTime.Hours, finalCompletionTime.Minutes, finalCompletionTime.Seconds);
                    canvasGraphics.DrawString($"TOPLAM SURE        : {durationStr}", statsFont, Brushes.LightCyan, 620, 480);
                    canvasGraphics.DrawString($"TOPLAM OLUM SAYISI: {totalDeaths}", statsFont, Brushes.Tomato, 620, 540);

                    string rankTitle = "YEVMIYECI CIRAK (Surgundeki Ninja)";
                    Brush rankBrush = Brushes.Gray;

                    if (totalDeaths <= 2 && finalCompletionTime.TotalMinutes <= 6) { rankTitle = "KALIPCI SEFI (Pro Speedrunner Ninja)"; rankBrush = Brushes.Gold; }
                    else if (totalDeaths <= 8) { rankTitle = "KESER USTASI (Santiye Kıdemli Ninjası)"; rankBrush = Brushes.SpringGreen; }
                    else if (totalDeaths <= 15) { rankTitle = "TASORON ALCI PANCI (Orta Kademe Ninja)"; rankBrush = Brushes.Orange; }

                    canvasGraphics.DrawString($"USTALIK UNVANI: {rankTitle}", rankFont, rankBrush, 520, 640);
                    canvasGraphics.DrawString("Enter veya Space tusuna basarak ana menuye donebilirsin usta", new Font("Arial", 16, FontStyle.Italic), Brushes.Gray, 640, 770);
                }

                if (File.Exists("savegame.dat")) File.Delete("savegame.dat");
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(virtualCanvas, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                return;
            }

            // --- ANA MENÜ RENDERI ---
            if (currentGameState == GameState.MainMenu)
            {
                MainMenuUI.Draw(canvasGraphics, menuSelection, startButton, exitButton);
                if (hasSaveFile)
                {
                    using (Font btnFont = new Font("Impact", 24))
                    {
                        canvasGraphics.FillRectangle(Brushes.DarkSlateGray, continueButton);
                        canvasGraphics.DrawRectangle(Pens.Cyan, continueButton);
                        canvasGraphics.DrawString("DEVAM ET", btnFont, Brushes.Cyan, continueButton.X + 80, continueButton.Y + 10);
                    }
                }
            }
            // --- LEVEL SELECT RENDERI ---
            else if (currentGameState == GameState.LevelSelect)
            {
                canvasGraphics.Clear(Color.FromArgb(10, 14, 28));
                using (Font titleFont = new Font("Impact", 52))
                using (Font roomFont = new Font("Arial Black", 35))
                using (Font infoFont = new Font("Arial", 18, FontStyle.Italic))
                {
                    canvasGraphics.DrawString("LEVEL SELECT ENGINE", titleFont, Brushes.Goldenrod, 530, 250);
                    canvasGraphics.DrawString("Esc: Ana Menu | Yon Tuslari: Sec | Enter-Space: Atla usta", infoFont, Brushes.Gray, 640, 360);

                    int itemWidth = 180; int spacing = 30;
                    int totalWidth = (selectAncorRooms.Length * itemWidth) + ((selectAncorRooms.Length - 1) * spacing);
                    int startX = (targetWidth - totalWidth) / 2;

                    for (int i = 0; i < selectAncorRooms.Length; i++)
                    {
                        int rx = startX + (i * (itemWidth + spacing)); int ry = 520;
                        bool isSelected = (i == selectRoomIndex);
                        Brush roomBrush = isSelected ? Brushes.Cyan : Brushes.DimGray;
                        if (isSelected) canvasGraphics.DrawRectangle(new Pen(Color.Cyan, 4f), rx - 10, ry - 10, itemWidth, 90);
                        canvasGraphics.DrawString($"R{selectAncorRooms[i]}", roomFont, roomBrush, rx, ry);
                    }
                }
            }
            // --- GİRİŞ AKTİF OYUN DÖNGÜSÜ ÇİZİMLERİ ---
            else
            {
                if (currentBgLayers != null && currentBgLayers.Count > 0)
                {
                    foreach (var layer in currentBgLayers)
                    {
                        layer.Draw(canvasGraphics, player.X, targetWidth, targetHeight);
                    }
                }
                else
                {
                    canvasGraphics.Clear(Color.FromArgb(20, 24, 43));
                }

                if (roomManager.CurrentRoom == 12)
                {
                    using (SolidBrush columnBrush = new SolidBrush(Color.FromArgb(30, 100, 50)))
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(15, 60, 30)))
                    {
                        for (int cx = 150; cx < targetWidth; cx += 450) { canvasGraphics.FillRectangle(columnBrush, cx, 100, 140, 740); canvasGraphics.FillRectangle(shadowBrush, cx + 100, 100, 40, 740); }
                    }
                }

                if (roomManager.CurrentRoom == 9 || roomManager.CurrentRoom == 10)
                {
                    using (SolidBrush waterBrush = new SolidBrush(Color.FromArgb(45, 40, 120, 220)))
                    using (Pen splashPen = new Pen(Color.FromArgb(90, 255, 255, 255), 2f))
                    {
                        if (roomManager.CurrentRoom == 9) { canvasGraphics.FillRectangle(waterBrush, 450, 100, 1050, 980); }
                        else { canvasGraphics.FillRectangle(waterBrush, 310, 100, 150, 900); canvasGraphics.FillRectangle(waterBrush, 1150, 100, 400, 900); }
                        int startW = (roomManager.CurrentRoom == 9) ? 600 : 330;
                        for (int wy = 140; wy < 900; wy += 90) { canvasGraphics.DrawArc(splashPen, startW, wy, 45, 20, 0, -180); if (roomManager.CurrentRoom == 10) canvasGraphics.DrawArc(splashPen, 1200, wy + 30, 50, 20, 0, -180); }
                    }
                }

                Brush platformBrush = (roomManager.CurrentRoom == 14) ? Brushes.SaddleBrown :
                                      ((roomManager.CurrentRoom == 11 || roomManager.CurrentRoom == 12 || roomManager.CurrentRoom == 13) ? Brushes.Goldenrod : Brushes.LightSlateGray);

                foreach (var platform in platforms)
                {
                    if (roomManager.CurrentRoom <= 7)
                    {
                        using (SolidBrush dirtBrush = new SolidBrush(Color.FromArgb(14, 28, 28)))
                        {
                            canvasGraphics.FillRectangle(dirtBrush, platform);
                        }

                        Image tileImg = Properties.Resources.mist_forest_background_tiles;
                        if (tileImg != null)
                        {
                            int srcStartX = 48;
                            int srcStartY = 16;
                            int yVisualOffset = 40;

                            int srcWidth = (tileImg.Width / 2) - srcStartX;
                            int srcHeight = tileImg.Height - srcStartY;

                            float scale = 2.5f;
                            int destWidth = (int)(srcWidth * scale);
                            int destHeight = (int)(srcHeight * scale);

                            for (int x = platform.X; x < platform.Right; x += destWidth)
                            {
                                int currentDestWidth = Math.Min(destWidth, platform.Right - x);
                                int currentSrcWidth = (int)(currentDestWidth / scale);

                                int currentDestHeight = Math.Min(destHeight, platform.Height + yVisualOffset);
                                int currentSrcHeight = (int)(currentDestHeight / scale);

                                Rectangle destRect = new Rectangle(x, platform.Y - yVisualOffset, currentDestWidth, currentDestHeight);
                                Rectangle srcRect = new Rectangle(srcStartX, srcStartY, currentSrcWidth, currentSrcHeight);

                                canvasGraphics.DrawImage(tileImg, destRect, srcRect, GraphicsUnit.Pixel);
                            }
                        }
                    }
                    else
                    {
                        canvasGraphics.FillRectangle(platformBrush, platform);
                    }
                }

                foreach (var gold in roomGolds) gold.Draw(canvasGraphics);
                foreach (var block in breakableBlocks) block.Draw(canvasGraphics);
                foreach (var tBlock in timedBlocks) tBlock.Draw(canvasGraphics);
                foreach (var mp in movingPlatforms) canvasGraphics.FillRectangle(Brushes.ForestGreen, mp.Hitbox);

                if (roomManager.CurrentRoom == 7)
                {
                    using (Pen spikePen = new Pen(Color.FromArgb(190, 200, 210), 3f))
                    {
                        int spikeY = 1020; int spikeWidth = 35; int spikeHeight = 45;
                        for (int sx = 0; sx < targetWidth; sx += spikeWidth) { Point[] spikePoints = { new Point(sx, spikeY), new Point(sx + (spikeWidth / 2), spikeY - spikeHeight), new Point(sx + spikeWidth, spikeY) }; canvasGraphics.FillPolygon(Brushes.SlateGray, spikePoints); canvasGraphics.DrawPolygon(spikePen, spikePoints); }
                    }
                }

                if (roomLadders != null)
                {
                    foreach (var ladder in roomLadders)
                    {
                        using (Pen ladderPen = new Pen(Color.FromArgb(230, 180, 40), 5f))
                        {
                            canvasGraphics.DrawLine(ladderPen, ladder.X, ladder.Y, ladder.X, ladder.Bottom);
                            canvasGraphics.DrawLine(ladderPen, ladder.Right, ladder.Y, ladder.Right, ladder.Bottom);
                            for (int ly = ladder.Y; ly <= ladder.Bottom; ly += 25)
                            {
                                canvasGraphics.DrawLine(ladderPen, ladder.X, ly, ladder.Right, ly);
                            }
                        }
                    }
                }

                if (roomManager.CurrentRoom == 14)
                {
                    canvasGraphics.FillRectangle(Brushes.SaddleBrown, 880, 710, 10, 40);
                    canvasGraphics.FillRectangle(Brushes.DarkRed, 840, 570, 90, 140);
                    canvasGraphics.DrawRectangle(Pens.Goldenrod, 840, 570, 90, 140);

                    using (Font textFont = new Font("Arial Black", 10, FontStyle.Bold))
                    using (Font priceFont = new Font("Impact", 12))
                    {
                        canvasGraphics.DrawString("SATIN ALMAK", textFont, Brushes.White, 842, 600);
                        canvasGraphics.DrawString("ICIN [UP]", textFont, Brushes.White, 855, 625);
                        canvasGraphics.DrawString("500 ALTIN", priceFont, Brushes.Gold, 853, 665);
                    }
                }

                if (checkpointTorch != null) checkpointTorch.Draw(canvasGraphics);

                foreach (var enemy in enemies) enemy.Draw(canvasGraphics);
                if (miniBoss != null) miniBoss.Draw(canvasGraphics);

                // --- 🛡️ SHIELD KNIGHT RETRO SPRITE ÇİZİM KATMANI ---
                if (shieldKnight != null)
                {
                    shieldKnight.Draw(canvasGraphics);
                }

                if (crimsonKnight != null)
                {
                    crimsonKnight.Draw(canvasGraphics);

                    if (crimsonKnight.IsPhase3)
                    {
                        using (Pen ragePen = new Pen(Color.FromArgb(160 + (int)(Math.Sin(DateTime.Now.Millisecond / 40.0) * 80), 255, 0, 0), 4f))
                        {
                            Rectangle auraRect = new Rectangle(crimsonKnight.X - 10, crimsonKnight.Y - 10, crimsonKnight.Width + 20, crimsonKnight.Height + 20);
                            canvasGraphics.DrawRectangle(ragePen, auraRect);
                        }

                        using (Font desperateFont = new Font("Impact", 26, FontStyle.Italic))
                        {
                            canvasGraphics.DrawString("!!! DESPERATE FORM !!!", desperateFont, Brushes.Red, 810, 160);
                        }
                    }
                }

                // KATMAN 4: Ana Karakter Çizimi (SABİTLENMİŞ RETRO SPRITE PİPELİNE)
                if (player.IsInvincible && (player.InvincibilityTimer % 6 < 3)) { }
                else
                {
                    Image currentSprite = player.GetCurrentFrameImage();
                    if (currentSprite != null)
                    {
                        Rectangle drawRect = new Rectangle(player.X, player.Y, player.Width, player.Height);

                        // --- 📑 TEMPORARY RENDER DEBUG LOG ENJEKSİYONU ---
                        string animState = player.IsAttacking ? "ATTACK" : (player.VerticalVelocity != 0 ? "JUMP" : (player.MoveLeft || player.MoveRight ? "RUN" : "IDLE"));
                        int frameIdx = player.AttackTimer;
                        System.Diagnostics.Debug.WriteLine($"[ANIM LOG] State: {animState} | FrameIdx: {frameIdx} | DestRect: {drawRect.ToString()}");

                        if (player.CurrentDirection == Player.Direction.Left)
                        {
                            GraphicsState state = canvasGraphics.Save();
                            canvasGraphics.TranslateTransform(drawRect.X + drawRect.Width, drawRect.Y);
                            canvasGraphics.ScaleTransform(-1, 1);
                            canvasGraphics.DrawImage(currentSprite, 0, 0, drawRect.Width, drawRect.Height);
                            canvasGraphics.Restore(state);
                        }
                        else
                        {
                            canvasGraphics.DrawImage(currentSprite, drawRect);
                        }
                    }
                    else
                    {
                        canvasGraphics.FillRectangle(Brushes.Black, player.Hitbox);
                    }
                }

                if (player.IsAttacking && !player.AttackHitbox.IsEmpty)
                {
                    float progress = (float)player.AttackTimer / player.AttackDuration;
                    int alpha = (int)(255 * (1.0f - progress)); if (alpha < 0) alpha = 0;

                    int arcSize = (int)(80 + (progress * 15)); Rectangle arcBounds; float startAngle, sweepAngle;
                    bool isDownAttacking = (GetAsyncKeyState((int)Keys.S) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.Down) & 0x8000) != 0;

                    if (player.VerticalVelocity != 0 && isDownAttacking)
                    {
                        int arcX = player.Hitbox.X - (arcSize - player.Hitbox.Width) / 2;
                        int arcY = player.Hitbox.Bottom - 25; arcBounds = new Rectangle(arcX, arcY, arcSize, arcSize);
                        startAngle = 0; sweepAngle = 180;
                    }
                    else
                    {
                        int arcY = player.Hitbox.Y + 25;
                        int facing = (player.CurrentDirection == Player.Direction.Right) ? 1 : -1;

                        if (facing == 1)
                        {
                            int arcX = player.Hitbox.Right - 45;
                            arcBounds = new Rectangle(arcX, arcY, arcSize, arcSize);
                            startAngle = -90 + (progress * 25); sweepAngle = 180;
                        }
                        else
                        {
                            int arcX = player.Hitbox.X - arcSize + 45;
                            arcBounds = new Rectangle(arcX, arcY, arcSize, arcSize);
                            startAngle = 270 - (progress * 25); sweepAngle = -180;
                        }
                    }

                    using (Pen neonPen = new Pen(Color.FromArgb((int)(alpha * 0.8f), 0, 235, 255), 3f)) { neonPen.StartCap = LineCap.Round; neonPen.EndCap = LineCap.Round; canvasGraphics.DrawArc(neonPen, arcBounds, startAngle, sweepAngle); }
                    using (Pen corePen = new Pen(Color.FromArgb(alpha, 255, 255, 255), 1.5f)) { corePen.StartCap = LineCap.Round; corePen.EndCap = LineCap.Round; canvasGraphics.DrawArc(corePen, arcBounds, startAngle, sweepAngle); }
                }

                HUDRenderer.Draw(canvasGraphics, roomManager.CurrentRoom, player.MaxHealth, player.CurrentHealth, totalGold, kalpDolu, kalpBos);
                if (currentGameState == GameState.Paused) PauseMenuUI.Draw(canvasGraphics, pauseSelection, resumeButton, settingsButton, mainMenuButton);
                if (isGameOver) GameOverUI.Draw(canvasGraphics);

                if (isEndingTriggered && fadeAlpha > 0)
                {
                    using (SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(fadeAlpha, Color.Black))) canvasGraphics.FillRectangle(fadeBrush, 0, 0, targetWidth, targetHeight);
                }
            }

            int offsetX = 0;
            int offsetY = 0;
            Random shakeRand = new Random();
            if (cameraShakeTimer > 0)
            {
                offsetX = shakeRand.Next(-12, 13);
                offsetY = shakeRand.Next(-12, 13);
            }

            if (arenaBroken && collapseVisualY < 1100)
            {
                using (SolidBrush collapseBrush = new SolidBrush(Color.FromArgb(14, 28, 28)))
                {
                    canvasGraphics.FillRectangle(collapseBrush, 0, collapseVisualY, 300, 210);
                    canvasGraphics.DrawLine(Pens.Red, 0, collapseVisualY, 300, collapseVisualY);

                    canvasGraphics.FillRectangle(collapseBrush, 1620, collapseVisualY, 300, 210);
                    canvasGraphics.DrawLine(Pens.Red, 1620, collapseVisualY, 1920, collapseVisualY);
                }
            }

            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(virtualCanvas, offsetX, offsetY, this.ClientSize.Width, this.ClientSize.Height);
        }

        void ResetGame()
        {
            if (isGameOver) totalDeaths++;

            isGameOver = false;
            totalGold = 0;

            // --- COLLAPSE RESET ---
            arenaBroken = false;
            collapseVisualY = 870;
            cameraShakeTimer = 0;

            GameMap.ResetWorld();
            roomManager.Reset();

            typeof(RoomManager).GetProperty("CurrentRoom").SetValue(roomManager, lastCheckpointRoom);
            player.Reset(lastCheckpointX, lastCheckpointY);
            player.CurrentHealth = 5;

            SaveProgress();
            SyncLoadRoom();

            this.Focus();
            this.Invalidate();
        }

        void SetScreenMode(bool fullscreen) { if (fullscreen) { this.FormBorderStyle = FormBorderStyle.None; this.WindowState = FormWindowState.Maximized; } else { this.FormBorderStyle = FormBorderStyle.FixedSingle; this.WindowState = FormWindowState.Normal; this.ClientSize = new Size(1280, 720); this.StartPosition = FormStartPosition.CenterScreen; } this.Refresh(); }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) { bool handled = inputManager.HandleProcessCmdKey(keyData, ref pauseSelection, ref currentGameState, () => { currentGameState = GameState.Playing; this.Focus(); }, () => { currentGameState = GameState.MainMenu; menuSelection = 0; this.Focus(); }); if (handled) return true; return base.ProcessCmdKey(ref msg, keyData); }
    }
}