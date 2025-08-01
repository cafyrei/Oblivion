using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Oblivion
{
    public class TextureManager_2
    {
        // Game Stages Variables;
        private MainMenu _mainMenu;
        private GameStage_2 _gameStage;

        private Ending _ending;
        private GameOverScreen _gameOver;
        private List<ZombieEnemies> _zombieEnemy;
        private SpriteAnimation2D _zombieEnemyAnimation;
        // Player Variables
        private Texture2D _samuraiTexture;
        private Texture2D _minorEnemyTexture;
        private Player _player;
        private SpriteAnimation2D _playerAnimation;
        private HPBar _HpBar;

        // Enemy Variables
        private SpriteAnimation2D _minorEnemyAnimation;
        private List<MinorEnemy> _minorEnemies;
        public static float attackAnimationSpeed = .125f;

        // Background Variables
        private List<ScrollingBackground> _scrollingBackground;
        private Platform _platform1;

        // Camera Variables
        private Camera2D _camera;
        private Texture2D _zombieEnemyTexture;
        public static int tileWidth = 2800;
        public static int tileHeight = 720;

        //HUD Variables
        private ObjectiveHUD objHUD;

        //Collectible Variables
        private Texture2D _collectibleTexture;
        private SpriteAnimation2D _collectibleAnimation;
        private List<Collectible> _collectibles;
        private static float _collectibleAnimationSpeed = .125f;


        private List<Boss> Bossing;
        private Texture2D BossTexture;
        private SpriteAnimation2D BossAnimation;
        private Boss boss;


        // Portal Variable
        Portal boss_Portal;
        private Texture2D portal_Texture;
        private SpriteAnimation2D _ToriiGate;

        // Wallpaper Variable
        private Texture2D background_Texture1;
        private Background background1;
        private Rectangle backgroundRect1;
        private Texture2D background_Texture2;
        private Background background2;
        private Rectangle backgroundRect2;

        private TextureManager texturemanager1;


        public void Load(ContentManager Content, GraphicsDevice graphicsDevice)
        {

            _ending = new Ending(Content, graphicsDevice);
            #region Player And Background Declaration
            try
            {
                _samuraiTexture = Content.Load<Texture2D>("Player/Samurai Spritesheet");
            }
            catch (ContentLoadException e)
            {
                Console.WriteLine("Error Loading _samuraiTexture Error Details : " + e);
            }

            _playerAnimation = new SpriteAnimation2D(
                frameWidth: 64,
                frameHeight: 64,
                rowFrameCount: new Dictionary<int, int>
                {
                    {0,6}, // Atk 1
                    {1,6}, // Atk 2
                    {2,6}, // Death
                    {3,5}, // Hit
                    {4,6}, // Idle
                    {5,6}, // Jump
                    {6,7}, // Sprint
                    {7,6}  // Walk
                },
                frameTime: 0.2f
            );

            _HpBar = new HPBar(Content);

            _player = new Player(_samuraiTexture, _playerAnimation, Content, _HpBar)
            {
                Position = new Vector2(50, 150),
                Layer = 0.94f,
            };

            _scrollingBackground = new List<ScrollingBackground>()
            {
                new ScrollingBackground(Content.Load<Texture2D>("Parallax_Layers/Stage 2 Layer 0"), _player, 1f)
                {
                    Layer = 0.1f,
                },
                new ScrollingBackground(Content.Load<Texture2D>("Parallax_Layers/Stage 2 Layer 2"), _player, 15f)
                {
                    Layer = 0.92f,
                },
                new ScrollingBackground(Content.Load<Texture2D>("Parallax_Layers/Stage 2 Layer 3"), _player, 30f)
                {
                    Layer = 0.95f,
                }
            };

            background_Texture1 = Content.Load<Texture2D>("Parallax_Layers/Stage 2 Layer 1");
            backgroundRect1 = new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            background1 = new Background(background_Texture1, backgroundRect1, Color.White);

            background_Texture2 = Content.Load<Texture2D>("Parallax_Layers/Stage 2 Layer 4");
            backgroundRect2 = new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            background2 = new Background(background_Texture2, backgroundRect2, Color.White);
            #endregion

            #region Minor and Major Enemies
            try
            {
                _minorEnemyTexture = Content.Load<Texture2D>("Enemies/Skelebones");
            }
            catch (ContentLoadException e)
            {
                Console.WriteLine("Error Loading Skeleton Texture Error Details : " + e);
            }

            _minorEnemyAnimation = new SpriteAnimation2D(
                frameHeight: 64,
                frameWidth: 96,
                rowFrameCount: new Dictionary<int, int>
                {
                    {0, 10}, // Walk
                    {1, 10}, // Attack
                    {2, 10}, // Death
                    {3, 8}, // Idle
                    {4, 5} // Hit
                },
                frameTime: attackAnimationSpeed
            );

            try
            {
                _zombieEnemyTexture = Content.Load<Texture2D>("Enemies/ZombieSP");
            }
            catch (ContentLoadException e)
            {
                Console.WriteLine("Error Loading Zombie Texture Error Details : " + e);
            }
            _zombieEnemyAnimation = new SpriteAnimation2D(
                frameHeight: 52,
                frameWidth: 64,
                rowFrameCount: new Dictionary<int, int>
                {
                    {0, 13}, // Attack
                    {1, 13}, // Death
                    {2, 12}, // Walk
                    {3, 3} // Hit
                },
                frameTime: attackAnimationSpeed
            );

            _zombieEnemy = new List<ZombieEnemies>();
            SpawnZombieEnemies(5); // Spawn 5 zombies

            _minorEnemies = new List<MinorEnemy>(); // Initialize the list of enemies
            SpawnWhiteEnemies(7); // Spawn enemies

            Bossing = new List<Boss>();
            #endregion

            #region Game Collectibles Declaration
            _collectibleTexture = Content.Load<Texture2D>("Collectibles/Health Soul");

            _collectibleAnimation = new SpriteAnimation2D(frameHeight: 64, frameWidth: 64, rowFrameCount: new Dictionary<int, int>
            {{0,7}},
            frameTime: _collectibleAnimationSpeed
            );

            _collectibles = new List<Collectible>();
            SpawnCollectibles(4);
            #endregion

            #region Game Over
            _gameOver = new GameOverScreen(Content, graphicsDevice);
            #endregion

            #region Portal (Torii)
            portal_Texture = Content.Load<Texture2D>("Platform/Tori_Portal");


            _ToriiGate =
               new SpriteAnimation2D(
               frameWidth: 64,
               frameHeight: 77,
               rowFrameCount: new Dictionary<int, int> { { 1, 4 } },
               frameTime: _collectibleAnimationSpeed);

            boss_Portal = new Portal(portal_Texture, _ToriiGate, new Vector2(2640,237));

            #endregion


            _camera = new Camera2D(graphicsDevice.Viewport);
            _mainMenu = new MainMenu(Content, graphicsDevice);
            _platform1 = new Platform("../../../Data/Stage2map.csv", Content, graphicsDevice);
            // Game Stage Constructor
            _gameStage = new GameStage_2(
                _scrollingBackground,
                _player,
                _minorEnemies,
                _platform1,
                _collectibles,
                () =>
                {
                    Game1.currentState = Game1.GameState.MainMenu;
                    MainMenu.ResetFlags();
                    _mainMenu.StartFadeIn();
                },
                boss_Portal,
                this,
                _zombieEnemy
            );

            _gameStage.Load(Content, graphicsDevice);

            foreach (var bg in _scrollingBackground)
            {
                bg.SetCamera(_camera);
            }

            objHUD = new ObjectiveHUD(Content);
        }


        private void SpawnZombieEnemies(int count)
        {
            Vector2[] spawnPositions = new Vector2[]
            {
                new Vector2(1200, 310),
                new Vector2(1600, 110),
                new Vector2(2000, 100),
                new Vector2(1915, 117),
                new Vector2(558, 117),
                new Vector2(1225, 145),
                new Vector2(1695, 309),
                new Vector2 (2303, 465)
            };

            foreach (var pos in spawnPositions)
            {
                var animationClone = new SpriteAnimation2D(_zombieEnemyAnimation);

                var zombie = new ZombieEnemies(_zombieEnemyTexture, animationClone, pos.X - 100, pos.X + 100, Camera)
                {
                    Position = pos,
                    Layer = 0.93f,
                };

                zombie.SetTarget(_player);
                _zombieEnemy.Add(zombie);
            }
        }


        private void SpawnWhiteEnemies(int count)
        {
            Vector2[] spawnPositions = new Vector2[]
            {
                new Vector2(400, 310),
                new Vector2(1500, 400),
                new Vector2(320, 465)
            };

            foreach (var pos in spawnPositions)
            {
                var animationClone = new SpriteAnimation2D(_minorEnemyAnimation);

                var enemy = new MinorEnemy(_minorEnemyTexture, animationClone, pos.X - 100, pos.X + 100, Camera)
                {
                    Position = pos,
                    Layer = 0.93f,
                };

                enemy.SetTarget(_player);
                _minorEnemies.Add(enemy);
            }
        }

        private void SpawnCollectibles(int count)
        {
            Vector2[] spawnPositions = new Vector2[]
            {
                new Vector2(1715, 305),
                new Vector2(1495, 115),
                new Vector2(1135, 175),
                new Vector2(1268, 465),
                new Vector2(875, 465),
                new Vector2(2335, 465),
                new Vector2(2543, 370),
                new Vector2(1862, 460)
            };

            foreach (var position in spawnPositions)
            {
                var animationClone = new SpriteAnimation2D(_collectibleAnimation);

                var collectible = new Collectible(_collectibleTexture, animationClone, position);

                _collectibles.Add(collectible);
            }
        }
        public void ResetGameStage(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            // Reset Player
            _player = new Player(_samuraiTexture, new SpriteAnimation2D(_playerAnimation), Content, _HpBar)
            {
                Position = new Vector2(50, 150),
                Layer = 0.94f,
            };

            _minorEnemies = new List<MinorEnemy>();
            SpawnWhiteEnemies(7);

            _zombieEnemy = new List<ZombieEnemies>();
            SpawnZombieEnemies(5);


            _collectibles = new List<Collectible>();
            SpawnCollectibles(4);

       _gameStage = new GameStage_2(
            _scrollingBackground,
            _player,
            _minorEnemies,
            _platform1,
            _collectibles,
            () =>
            {
                Game1.ResetToStage1();
            },
            boss_Portal,
            this,
            _zombieEnemy
        );


        }

        // Properties
        public MainMenu MainMenu => _mainMenu;
        public GameStage_2 GameStage2 => _gameStage;
        public ObjectiveHUD objectiveHUD => objHUD;
        public HPBar HPBarAccess => _HpBar;
        public Camera2D Camera { get => _camera; }
        public Player Player1 { get => _player; set => _player = value; }
        public GameOverScreen GameOver { get => _gameOver; }
       public Background Background1 => background1;
        public Background Background2 => background2;
        public Ending Ending { get => _ending; set => _ending = value; }
    }


}
