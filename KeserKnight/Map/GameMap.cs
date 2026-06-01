using System;
using System.Collections.Generic;
using System.Drawing;
using KeserKnight.Entity;

namespace KeserKnight.Map
{
    public static class GameMap
    {
        // Tüm odaların nesne verilerini hafızada tutan ana sözlük yapıları (Bellek optimizasyonu sağlar)
        private static Dictionary<int, List<Rectangle>> allPlatforms = new Dictionary<int, List<Rectangle>>();
        private static Dictionary<int, List<Enemy>> allEnemies = new Dictionary<int, List<Enemy>>();
        private static Dictionary<int, List<Gold>> allGolds = new Dictionary<int, List<Gold>>();
        private static Dictionary<int, List<BreakableBlock>> allBreakableBlocks = new Dictionary<int, List<BreakableBlock>>();
        private static Dictionary<int, List<TimedBlock>> allTimedBlocks = new Dictionary<int, List<TimedBlock>>();
        private static Dictionary<int, List<MovingPlatform>> allMovingPlatforms = new Dictionary<int, List<MovingPlatform>>();
        private static Dictionary<int, List<Rectangle>> allLadders = new Dictionary<int, List<Rectangle>>();

        private static bool isInitialized = false;

        public static void InitializeWorld()
        {
            // Dünya sıfırlandığında eski verilerin üst üste binmemesi için listeleri temizliyoruz
            allPlatforms.Clear();
            allEnemies.Clear();
            allGolds.Clear();
            allBreakableBlocks.Clear();
            allTimedBlocks.Clear();
            allMovingPlatforms.Clear();
            allLadders.Clear();

            // ODA 1 - 13: Önceki aşamalarda kurulan platform ve düşman yerleşimleri
            allPlatforms[1] = new List<Rectangle> { new Rectangle(0, 850, 550, 230), new Rectangle(650, 750, 200, 40), new Rectangle(950, 650, 200, 40), new Rectangle(1400, 550, 570, 530) };
            allEnemies[1] = new List<Enemy> { new Enemy(1500, 420, 130, 130, 80) };
            allGolds[1] = new List<Gold> { new Gold(750, 700, 10, Color.Gold), new Gold(1050, 600, 10, Color.Gold), new Gold(1450, 500, 50, Color.Cyan) };
            allBreakableBlocks[1] = new List<BreakableBlock>(); allTimedBlocks[1] = new List<TimedBlock>(); allMovingPlatforms[1] = new List<MovingPlatform>(); allLadders[1] = new List<Rectangle>();

            allPlatforms[2] = new List<Rectangle> { new Rectangle(-20, 550, 420, 530), new Rectangle(550, 750, 800, 50), new Rectangle(1500, 650, 420, 630) };
            allEnemies[2] = new List<Enemy> { new Enemy(700, 620, 130, 130, 120), new Enemy(1100, 620, 130, 130, 100) };
            allGolds[2] = new List<Gold> { new Gold(750, 700, 10, Color.Gold), new Gold(950, 700, 50, Color.Cyan), new Gold(1150, 700, 10, Color.Gold) };
            allBreakableBlocks[2] = new List<BreakableBlock>(); allTimedBlocks[2] = new List<TimedBlock>(); allMovingPlatforms[2] = new List<MovingPlatform>(); allLadders[2] = new List<Rectangle>();

            allPlatforms[3] = new List<Rectangle> { new Rectangle(0, 650, 400, 430), new Rectangle(400, 1020, 1520, 60), new Rectangle(480, 850, 350, 40), new Rectangle(580, 650, 650, 40), new Rectangle(1280, 800, 400, 40), new Rectangle(1450, 450, 470, 630) };
            allEnemies[3] = new List<Enemy> { new Enemy(650, 520, 130, 130, 120), new Enemy(950, 520, 130, 130, 100) };
            allGolds[3] = new List<Gold> { new Gold(550, 800, 10, Color.Gold), new Gold(900, 590, 50, Color.Cyan), new Gold(1600, 390, 50, Color.Cyan) };
            allBreakableBlocks[3] = new List<BreakableBlock>(); allTimedBlocks[3] = new List<TimedBlock>(); allMovingPlatforms[3] = new List<MovingPlatform>(); allLadders[3] = new List<Rectangle>();

            allPlatforms[4] = new List<Rectangle> { new Rectangle(0, 580, 300, 500), new Rectangle(300, 720, 200, 360), new Rectangle(500, 680, 150, 400), new Rectangle(650, 580, 150, 500), new Rectangle(800, 850, 600, 230), new Rectangle(1400, 720, 520, 360), new Rectangle(0, 0, 400, 300), new Rectangle(950, 0, 970, 280) };
            allBreakableBlocks[4] = new List<BreakableBlock> { new BreakableBlock(1280, 720, 120, 130), new BreakableBlock(1280, 590, 120, 130) };
            allEnemies[4] = new List<Enemy> { new Enemy(950, 720, 130, 130, 100) };
            allGolds[4] = new List<Gold> { new Gold(1550, 620, 50, Color.Cyan), new Gold(150, 480, 10, Color.Gold) };
            allTimedBlocks[4] = new List<TimedBlock>(); allMovingPlatforms[4] = new List<MovingPlatform>(); allLadders[4] = new List<Rectangle>();

            allPlatforms[5] = new List<Rectangle> { new Rectangle(0, 720, 500, 360), new Rectangle(500, 850, 450, 230), new Rectangle(950, 550, 970, 530), new Rectangle(1010, 250, 910, 60), new Rectangle(0, 200, 950, 60) };
            allTimedBlocks[5] = new List<TimedBlock> { new TimedBlock(650, 620, 160, 50, 0) };
            allEnemies[5] = new List<Enemy>();
            allGolds[5] = new List<Gold> { new Gold(720, 520, 10, Color.Gold) };
            allBreakableBlocks[5] = new List<BreakableBlock>(); allMovingPlatforms[5] = new List<MovingPlatform>(); allLadders[5] = new List<Rectangle> { new Rectangle(950, 200, 60, 880) };

            allPlatforms[6] = new List<Rectangle> { new Rectangle(0, 250, 600, 60), new Rectangle(600, 500, 400, 40), new Rectangle(1000, 750, 920, 330), new Rectangle(0, 0, 1920, 100) };
            allBreakableBlocks[6] = new List<BreakableBlock>(); allTimedBlocks[6] = new List<TimedBlock>();
            allEnemies[6] = new List<Enemy> { new Enemy(1200, 620, 130, 130, 100) };
            allGolds[6] = new List<Gold> { new Gold(780, 420, 50, Color.Cyan) };
            allMovingPlatforms[6] = new List<MovingPlatform>(); allLadders[6] = new List<Rectangle> { new Rectangle(950, 500, 60, 580) };

            allPlatforms[7] = new List<Rectangle> { new Rectangle(0, 450, 350, 630), new Rectangle(1550, 450, 370, 630), new Rectangle(0, 1020, 1920, 60), new Rectangle(0, 0, 1920, 100) };
            allMovingPlatforms[7] = new List<MovingPlatform> { new MovingPlatform(500, 650, 250, 50, 600, MovingPlatform.MovementType.Horizontal) };
            allEnemies[7] = new List<Enemy>(); allGolds[7] = new List<Gold> { new Gold(900, 500, 10, Color.Gold) }; allBreakableBlocks[7] = new List<BreakableBlock>(); allTimedBlocks[7] = new List<TimedBlock>(); allLadders[7] = new List<Rectangle>();

            allPlatforms[8] = new List<Rectangle> { new Rectangle(0, 850, 1750, 230), new Rectangle(1750, 0, 170, 1080), new Rectangle(0, 0, 1920, 100) };
            allEnemies[8] = new List<Enemy>(); allGolds[8] = new List<Gold>(); allBreakableBlocks[8] = new List<BreakableBlock>(); allTimedBlocks[8] = new List<TimedBlock>(); allMovingPlatforms[8] = new List<MovingPlatform>(); allLadders[8] = new List<Rectangle> { new Rectangle(1715, 850, 35, 230) };

            allPlatforms[9] = new List<Rectangle> { new Rectangle(0, 850, 450, 230), new Rectangle(1550, 450, 420, 630), new Rectangle(0, 0, 1920, 100) };
            allTimedBlocks[9] = new List<TimedBlock> { new TimedBlock(500, 720, 160, 50, 0), new TimedBlock(750, 620, 160, 50, 45), new TimedBlock(1000, 520, 160, 50, 0), new TimedBlock(1250, 480, 160, 50, 45) };
            allEnemies[9] = new List<Enemy>(); allGolds[9] = new List<Gold> { new Gold(830, 500, 50, Color.Cyan), new Gold(1100, 400, 50, Color.Cyan) }; allBreakableBlocks[9] = new List<BreakableBlock>(); allMovingPlatforms[9] = new List<MovingPlatform>(); allLadders[9] = new List<Rectangle>();

            allPlatforms[10] = new List<Rectangle> { new Rectangle(0, 880, 1700, 200), new Rectangle(200, 580, 1720, 60), new Rectangle(0, 280, 1920, 60), new Rectangle(0, 0, 1920, 100) };
            allLadders[10] = new List<Rectangle> { new Rectangle(1650, 580, 50, 300), new Rectangle(200, 280, 50, 300) };
            allEnemies[10] = new List<Enemy> { new Enemy(600, 450, 130, 130, 100), new Enemy(1100, 450, 130, 130, 120) };
            allGolds[10] = new List<Gold> { new Gold(850, 200, 50, Color.Cyan) };
            allBreakableBlocks[10] = new List<BreakableBlock>(); allTimedBlocks[10] = new List<TimedBlock>(); allMovingPlatforms[10] = new List<MovingPlatform>();

            allPlatforms[11] = new List<Rectangle> { new Rectangle(0, 650, 400, 430), new Rectangle(700, 450, 1220, 630), new Rectangle(0, 0, 1920, 100) };
            allTimedBlocks[11] = new List<TimedBlock> { new TimedBlock(480, 530, 160, 50, 0) };
            allEnemies[11] = new List<Enemy> { new Enemy(1200, 320, 130, 130, 100) };
            allGolds[11] = new List<Gold> { new Gold(1600, 350, 50, Color.Cyan) };
            allBreakableBlocks[11] = new List<BreakableBlock>(); allMovingPlatforms[11] = new List<MovingPlatform>(); allLadders[11] = new List<Rectangle>();

            allPlatforms[12] = new List<Rectangle> { new Rectangle(0, 840, 1920, 240), new Rectangle(0, 0, 1920, 100) };
            allEnemies[12] = new List<Enemy> { new Enemy(1000, 710, 130, 130, 130) };
            allGolds[12] = new List<Gold> { new Gold(1700, 740, 50, Color.Cyan), new Gold(200, 740, 50, Color.Cyan) };
            allBreakableBlocks[12] = new List<BreakableBlock>(); allTimedBlocks[12] = new List<TimedBlock>(); allMovingPlatforms[12] = new List<MovingPlatform>(); allLadders[12] = new List<Rectangle>();

            allPlatforms[13] = new List<Rectangle>
            {
                new Rectangle(0, 480, 450, 600),
                new Rectangle(1450, 480, 470, 600),
                new Rectangle(1450, 480, 100, 40),
                new Rectangle(1550, 560, 100, 40),
                new Rectangle(1650, 640, 100, 40),
                new Rectangle(1750, 720, 170, 360),
                new Rectangle(0, 0, 1920, 100)
            };
            allMovingPlatforms[13] = new List<MovingPlatform> { new MovingPlatform(500, 600, 200, 45, 400, MovingPlatform.MovementType.Horizontal) };
            allTimedBlocks[13] = new List<TimedBlock> { new TimedBlock(1150, 550, 160, 45, 0) };
            allEnemies[13] = new List<Enemy>();
            allGolds[13] = new List<Gold> { new Gold(600, 450, 50, Color.Cyan), new Gold(1230, 420, 10, Color.Gold) };
            allBreakableBlocks[13] = new List<BreakableBlock>(); allLadders[13] = new List<Rectangle>();

            allPlatforms[14] = new List<Rectangle>
            {
                new Rectangle(0, 870, 550, 210),
                new Rectangle(550, 710, 420, 370),
                new Rectangle(970, 790, 300, 290),
                new Rectangle(1270, 870, 650, 210),
                new Rectangle(0, 0, 1920, 100)
            };
            allEnemies[14] = new List<Enemy>();
            allGolds[14] = new List<Gold> { new Gold(760, 620, 50, Color.Cyan) };
            allBreakableBlocks[14] = new List<BreakableBlock>(); allTimedBlocks[14] = new List<TimedBlock>(); allMovingPlatforms[14] = new List<MovingPlatform>(); allLadders[14] = new List<Rectangle>();

            // ODA 15: ANA BOSS ARENASI DÜMDÜZ SAVAŞ YOLU
            // Oyuncunun ve patronun yeteneklerini tam sergileyebilmesi için hiçbir platform engeli içermez.
            allPlatforms[15] = new List<Rectangle>
            {
                new Rectangle(0, 870, 1920, 210),       // Ekranın solundan sağına uzanan tek parça arena zemini
                new Rectangle(0, 0, 1920, 100)          // Tavandaki üst sınır bloğu
            };
            allEnemies[15] = new List<Enemy>();         // Yapay zeka ajanını Form1 içerisinden özel olarak besleyeceğiz
            allGolds[15] = new List<Gold>();
            allBreakableBlocks[15] = new List<BreakableBlock>();
            allTimedBlocks[15] = new List<TimedBlock>();
            allMovingPlatforms[15] = new List<MovingPlatform>();
            allLadders[15] = new List<Rectangle>();

            isInitialized = true;
        }

        public static void LoadRoom(int roomNumber, List<Rectangle> activePlatforms, List<Enemy> activeEnemies, List<Gold> activeGolds, List<BreakableBlock> activeBlocks, List<TimedBlock> activeTimedBlocks, List<MovingPlatform> activeMovingPlatforms, out List<Rectangle> activeLadders)
        {
            if (!isInitialized) InitializeWorld();

            activePlatforms.Clear();
            activeEnemies.Clear();
            activeGolds.Clear();
            activeBlocks.Clear();
            activeTimedBlocks.Clear();
            activeMovingPlatforms.Clear();

            activeLadders = new List<Rectangle>();
            if (allLadders.ContainsKey(roomNumber)) activeLadders.AddRange(allLadders[roomNumber]);
            if (allPlatforms.ContainsKey(roomNumber)) activePlatforms.AddRange(allPlatforms[roomNumber]);

            if (allEnemies.ContainsKey(roomNumber))
            {
                foreach (var enemy in allEnemies[roomNumber]) activeEnemies.Add(new Enemy(enemy.X, enemy.Y, enemy.Width, enemy.Height, 100));
            }
            if (allGolds.ContainsKey(roomNumber))
            {
                foreach (var gold in allGolds[roomNumber]) activeGolds.Add(new Gold(gold.Hitbox.X, gold.Hitbox.Y, gold.Value, gold.GoldColor));
            }
            if (allBreakableBlocks.ContainsKey(roomNumber))
            {
                foreach (var block in allBreakableBlocks[roomNumber]) activeBlocks.Add(new BreakableBlock(block.Hitbox.X, block.Hitbox.Y, block.Hitbox.Width, block.Hitbox.Height));
            }
            if (allTimedBlocks.ContainsKey(roomNumber))
            {
                foreach (var block in allTimedBlocks[roomNumber]) activeTimedBlocks.Add(new TimedBlock(block.Hitbox.X, block.Hitbox.Y, block.Hitbox.Width, block.Hitbox.Height, block.IsActive ? 0 : 45));
            }
            if (allMovingPlatforms.ContainsKey(roomNumber))
            {
                foreach (var mp in allMovingPlatforms[roomNumber])
                {
                    int distance = 600;
                    if (roomNumber == 10) distance = (mp.Type == MovingPlatform.MovementType.Vertical) ? 450 : 550;
                    if (roomNumber == 13) distance = 400;
                    activeMovingPlatforms.Add(new MovingPlatform(mp.Hitbox.X, mp.Hitbox.Y, mp.Hitbox.Width, mp.Hitbox.Height, distance, mp.Type));
                }
            }
        }

        public static void SaveRoomState(int roomNumber, List<Enemy> currentEnemies, List<Gold> currentGolds, List<BreakableBlock> currentBlocks)
        {
            if (!isInitialized) return;
            allEnemies[roomNumber] = new List<Enemy>(currentEnemies);
            allGolds[roomNumber] = new List<Gold>(currentGolds);
            allBreakableBlocks[roomNumber] = new List<BreakableBlock>(currentBlocks);
        }

        public static GuardianGriffon GetBossInstance(int roomNumber)
        {
            if (roomNumber == 8) return new GuardianGriffon(
                1450, 520, 250, 330,
                Properties.Resources.pence_Griffoth,
                Properties.Resources.boss_fire_pose
            );
            return null;
        }

        public static void ResetWorld() { InitializeWorld(); }
    }
}