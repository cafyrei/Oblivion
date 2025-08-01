using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Oblivion
{
    public class GameStage_2
    {
        private List<ScrollingBackground> _scrollingBackground;
        private Player _player;
        private List<MinorEnemy> _minorEnemies;
        private List<ZombieEnemies> _zombieEnemies;
        private Platform _platform;
        private List<Collectible> _collectible;

        public Background bg1;
        public Background bg2;

        private bool _gamePause = false;
        KeyboardState _previousKeyboardState;

        private PauseMenu _pauseMenu;
        private SpriteFont _font;
        private Portal _torii_gate;
        private bool _toriiGateSpawn = false;
        private TextureManager_2 _textureManager;
        public static int aliveEnemies { get; private set; }
        public bool GamePause { get => _gamePause; }

        ContentManager Content1;
        GraphicsDevice graphicsDevice1;

        private readonly Action _onExitToMenu;
        public GameStage_2(
            List<ScrollingBackground> scrollingBackground,
            Player player,
            List<MinorEnemy> minorEnemy,
            Platform platform,
            List<Collectible> collectible,
            Action onExitToMenu,
            Portal _torii_Gate,
            TextureManager_2 textureManager,
            List<ZombieEnemies> zombieEnemies
        )
        {
            _scrollingBackground = scrollingBackground;
            _player = player;
            _minorEnemies = minorEnemy;
            _platform = platform;
            _previousKeyboardState = Keyboard.GetState();
            _onExitToMenu = onExitToMenu;
            _collectible = collectible;
            _torii_gate = _torii_Gate;
            _zombieEnemies = zombieEnemies;

            _textureManager = textureManager;
        }

        public void Load(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            _font = Content.Load<SpriteFont>("Fonts/PauseFont");
            _pauseMenu = new PauseMenu(graphicsDevice, _font, Content);
            _pauseMenu.OnResumeGame += gameResume;
            _pauseMenu.BackToMenu += backToMenu;
            _pauseMenu.OnSaveGame += SaveProgress;

            Content1 = Content;
            graphicsDevice1 = graphicsDevice;
        }

        public void Update(GameTime gameTime, Camera2D camera)
        {
            KeyboardState _currentKeyboardState = Keyboard.GetState();
            AudioManager.PlayBossStageBGM();

            if (_currentKeyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                _gamePause = !_gamePause;
            }

            _previousKeyboardState = _currentKeyboardState;

            _pauseMenu.Update();

            if (_player.Position.Y > Game1.ScreenHeight)
            {
                Game1.currentState = Game1.GameState.GameOver;
            }

            if (!_gamePause)
            {
                foreach (var sb in _scrollingBackground)
                {
                    sb.Update(gameTime);
                }

                _minorEnemies.RemoveAll(enemy => enemy.IsDead);

                foreach (var enemy in _minorEnemies)
                {
                    enemy.Update(gameTime, _platform.collision, camera);
                }

                _zombieEnemies.RemoveAll(enemy => enemy.IsDead);

                _player.Update(gameTime, _platform.collision, _minorEnemies, _zombieEnemies);

                foreach (var zombie in _zombieEnemies)
                {
                    zombie.Update(gameTime, _platform.collision, camera);
                }

                aliveEnemies = _minorEnemies.Count(e => !e.IsDead) + _zombieEnemies.Count(e => !e.IsDead) - 1;

                if (aliveEnemies == 0 && !_toriiGateSpawn)
                {
                    _toriiGateSpawn = true;
                    AudioManager.StopMusic();
                    AudioManager.PlaySFX(AudioManager._gatesOpenedrSFX, .5f);
                }

                if (_toriiGateSpawn)
                {
                    _torii_gate.DoneGame(gameTime, _player, aliveEnemies);
                }


                foreach (var collectible in _collectible)
                {
                    collectible.Update(gameTime, _player);
                }
            }

        }
        private void gameResume()
        {
            _gamePause = false;
        }

        private void backToMenu()
        {
            AudioManager.StopMusic();

            // Just change the game state to Main Menu
            Game1.currentState = Game1.GameState.MainMenu;
            Game1.ResetToStage1();

            _onExitToMenu?.Invoke();
        }

        private void SaveProgress()
        {
            Game1.gameLevel = 2;
            PlayerData data = new PlayerData
            {
                Health = _player.CurrentHealth,
                CurrentStage = Game1.gameLevel, // Problem Variable
                SpawnPosition = new SerializableVector2(_player.Position)
            };

            SaveSystem.SavePlayerData(data);
        }

        public void LoadProgress(PlayerData data)
        {
            if (data == null) return;

            _player.CurrentHealth = data.Health;
            _player.Position = data.SpawnPosition.ToVector2();
        }


        public Vector2 GetPlayerPosition()
        {
            return _player.Position;
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _scrollingBackground[0].Draw(gameTime, _spriteBatch);
            _scrollingBackground[1].Draw(gameTime, _spriteBatch);
            _platform.Draw(_spriteBatch);
            foreach (var enemy in _minorEnemies)
            {
                enemy.Draw(_spriteBatch);
            }

            foreach (var zombie in _zombieEnemies)
            {
                zombie.Draw(_spriteBatch);
            }

            foreach (var collectible in _collectible)
            {
                collectible.Draw(_spriteBatch);
            }

            if (_toriiGateSpawn)
            {
                _torii_gate.Draw(_spriteBatch);
            }
            _player.Draw(_spriteBatch);
            _scrollingBackground[2].Draw(gameTime, _spriteBatch);
        }
        public void DrawUI(SpriteBatch _spriteBatch, Viewport _screenViewPort)
        {
            if (_gamePause)
            {
                _pauseMenu.Draw(_spriteBatch, _screenViewPort);
            }
        }
    }
}
