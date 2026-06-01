using System;
using System.Windows.Forms;
using KeserKnight.Entity;
using KeserKnight.Combat;

namespace KeserKnight.Core
{
    public class InputManager
    {
        public void HandleMenuInput(KeyEventArgs e, ref int menuSelection, Action onStartGame)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S) menuSelection = 1;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W) menuSelection = 0;

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                if (menuSelection == 0) onStartGame?.Invoke();
                else if (menuSelection == 1) Application.Exit();
            }
        }

        public void HandleGameKeyDown(KeyEventArgs e, Player player, AttackSystem attackSystem, bool isGameOver, Action onResetGame)
        {
            if (isGameOver && e.KeyCode == Keys.R)
            {
                onResetGame?.Invoke();
                return;
            }

            if (isGameOver) return;

            if (e.KeyCode == Keys.A) player.MoveLeft = true;
            if (e.KeyCode == Keys.D) player.MoveRight = true;

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.W)
            {
                if (!player.IsJumping)
                {
                    player.VerticalVelocity = player.JumpPower;
                    player.IsJumping = true;
                }
            }

            if (e.KeyCode == Keys.L)
            {
                if (!player.IsAttacking)
                {
                    
                    attackSystem.HandleAttackInput(player);

                    int lungeDistance = 12;

                    if (player.CurrentDirection == Player.Direction.Right)
                    {
                        player.X += lungeDistance;
                    }
                    else if (player.CurrentDirection == Player.Direction.Left)
                    {
                        player.X -= lungeDistance;
                    }
                }
            }
        }

        
        public void HandleGameKeyUp(KeyEventArgs e, Player player)
        {
            if (e.KeyCode == Keys.A) player.MoveLeft = false;
            if (e.KeyCode == Keys.D) player.MoveRight = false;

            
            // Oyuncu zıplama tuşunu bıraktığı an (Space veya W)
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.W)
            {
                // Karakter eğer hala yukarı doğru yükseliyorsa (VerticalVelocity negatifse)
                if (player.IsJumping && player.VerticalVelocity < 0)
                {
                    
                    // Bu değer karakterin havada tık diye süzülüp erken düşmesini sağlar.
                    player.VerticalVelocity *= 0.4f;
                }
            }
            
        }

        // --- ENÜM DESTEKLİ HIZLANDIRILMIŞ METOT ---
        // 'string' yerine doğrudan Form1'in kendi GameState enum yapısını referans alarak 
        // metinsel gecikmeleri sıfıra indiriyoruz kanki.
        public bool HandleProcessCmdKey(Keys keyData, ref int pauseSelection, ref Form1.GameState gameState, Action resumeAction, Action mainMenuAction)
        {
            // 1. Oyun oynanırken ESC basılırsa oyunu durdur
            if (gameState == Form1.GameState.Playing && keyData == Keys.Escape)
            {
                gameState = Form1.GameState.Paused;
                pauseSelection = 0;
                return true;
            }

            // 2. Oyun zaten duraklatılmışsa (ESC Menüsündeyken)
            else if (gameState == Form1.GameState.Paused)
            {
                if (keyData == Keys.Escape)
                {
                    resumeAction?.Invoke();
                    gameState = Form1.GameState.Playing;
                    return true;
                }

                // Yukarı Taşıma (W veya Yukarı Ok)
                if (keyData == Keys.W || keyData == Keys.Up)
                {
                    pauseSelection = (pauseSelection - 1 + 3) % 3;
                    return true;
                }

                // Aşağı Taşıma (S veya Aşağı Ok)
                if (keyData == Keys.S || keyData == Keys.Down)
                {
                    pauseSelection = (pauseSelection + 1) % 3;
                    return true;
                }

                // --- ENTER VE SPACE TETİKLEME MOTORU ---
                if (keyData == Keys.Enter || keyData == Keys.Space)
                {
                    if (pauseSelection == 0) // DEVAM ET
                    {
                        resumeAction?.Invoke();
                        gameState = Form1.GameState.Playing;
                    }
                    else if (pauseSelection == 1) // AYARLAR
                    {
                        MessageBox.Show("Ayarlar Menüsü Yakında Eklenecek Usta!", "KeserKnight Ayarlar");
                    }
                    else if (pauseSelection == 2) // ANA MENÜ
                    {
                        mainMenuAction?.Invoke();
                        gameState = Form1.GameState.MainMenu;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}