#region File Description
/*
    Game1.cs
 *  Author: Zongqing Sun and Gan Zhang
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;
#endregion


namespace AnimatedSprites
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Particle declare
        ParticleEffect particleEffect;
        Renderer particleRenderer;
        #endregion
        #region variables declaration
        //total life number
        int numberLivesRemaining = 3;
        int numberLivesRemaining2 = 3;
        Boolean player1Win = true;
        public int NumberLivesRemaining
        {
            get { return numberLivesRemaining; }
            set
            {
                numberLivesRemaining = value;
                player1Win = false;
                if (numberLivesRemaining == 0)
                {
                    currentGameState = GameState.GameOver;
                    spriteManager.Enabled = false;
                    spriteManager.Visible = false;
                }
            }
        }
        public int NumberLivesRemaining2
        {
            get { return numberLivesRemaining2; }
            set
            {
                numberLivesRemaining2 = value;
                player1Win = true;
                if (numberLivesRemaining2 == 0)
                {
                    currentGameState = GameState.GameOver;
                    spriteManager.Enabled = false;
                    spriteManager.Visible = false;
                }
            }
        }

        //Game state variables
        enum GameState { Start, InGame, GameOver };
        GameState currentGameState = GameState.Start;

        //background
        Texture2D backgroundTexture;

        //current score
        int currentScore = 0;
        int currentScore2 = 0;
        //font
        SpriteFont scoreFont;

        //sound files
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;
        //random variable
        public Random rnd { get; private set; }

        // SpriteManager instance
        SpriteManager spriteManager;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }
        #endregion
        

        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }


        #region Initialization
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            #region paticle initial
            particleEffect = new ParticleEffect
            {
                new Emitter
                {
                    Budget = 1000,
                    Term = 3f,

                    Name = "FirstEmitter",
                    BlendMode = EmitterBlendMode.Alpha,
                    ReleaseQuantity = 3,
                    ReleaseRotation = new VariableFloat{Value = 0f, Variation = MathHelper.PiOver2},
                    ReleaseSpeed = new VariableFloat{Value = 64f, Variation = 32f},
                    ReleaseScale = 48f, 

                    ParticleTextureAssetName = "images\\smoke",

                    Modifiers = new ModifierCollection
                    {
                        new OpacityModifier
                        {
                            Initial = 1f,
                            Ultimate = 0f,
                        },
                        new ColourModifier
                        {
                            InitialColour = Color.DarkRed.ToVector3(),
                            UltimateColour = Color.YellowGreen.ToVector3(),
                        },
                    },
                },
            };

            particleRenderer = new SpriteBatchRenderer 
            { 
                GraphicsDeviceService = graphics,
            };
            #endregion
            

            rnd = new Random();

            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteManager = new SpriteManager(this);
            Components.Add(spriteManager);

            particleEffect.Initialise();

            spriteManager.Enabled = false;
            spriteManager.Visible = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            particleRenderer.LoadContent(Content);
            particleEffect.LoadContent(Content);
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            scoreFont = Content.Load<SpriteFont>(@"fonts\Score");
            backgroundTexture = Content.Load<Texture2D>(@"Images\background1");

            //Start the soundtrack audio
            trackCue = soundBank.GetCue("qingsongyukuai");
            trackCue.Play();

            //Play the start sound
            soundBank.PlayCue("start");
        }
        #endregion

        

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #region Update,Draw
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (currentGameState)
            {
                case GameState.Start:
                    if (Keyboard.GetState().GetPressedKeys().Length > 0)
                    {
                        currentGameState = GameState.InGame;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                    }
                    break;
                case GameState.InGame:
                    if(spriteManager.IsHit())
                    particleEffect.Trigger(spriteManager.GetPlayerPosition2()+new Vector2(32,32));
                    float defaultSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    particleEffect.Update(defaultSeconds);

                    break;
                case GameState.GameOver:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        Exit();
                    if (Keyboard.GetState().IsKeyDown(Keys.R))
                    {
                        currentGameState = GameState.InGame;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                    }
                    break;
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            audioEngine.Update();

            base.Update(gameTime);
        }

        // this function is called when we want to demo the explosion effect. it
        // updates the timeTillExplosion timer, and starts another explosion effect
        // when the timer reaches zero.
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameState)
            {
                #region start
                case GameState.Start:
                    GraphicsDevice.Clear(Color.AliceBlue);

                    //Draw text for intro splash screen
                    spriteBatch.Begin();
                    string text = "Let's start!";
                    spriteBatch.DrawString(scoreFont, text,
                        new Vector2((Window.ClientBounds.Width / 2)
                            - (scoreFont.MeasureString(text).X / 2),
                            (Window.ClientBounds.Height / 2)
                            - (scoreFont.MeasureString(text).Y / 2)),
                            Color.SaddleBrown);

                    text = "(Press any key to begin)";
                    spriteBatch.DrawString(scoreFont, text,
                        new Vector2((Window.ClientBounds.Width / 2)
                            - (scoreFont.MeasureString(text).X / 2),
                            (Window.ClientBounds.Height / 2)
                            - (scoreFont.MeasureString(text).Y / 2) + 30),
                            Color.SaddleBrown);
                    spriteBatch.End();
                    break;
                #endregion
                #region InGame
                case GameState.InGame:
                    
                    GraphicsDevice.Clear(Color.AliceBlue);
                    
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                    if (spriteManager.IsHit())
                    particleRenderer.RenderEffect(particleEffect);
                    particleRenderer.RenderEffect(particleEffect);
                    
                    spriteBatch.End();
                    break;
                #endregion
                #region GameOver
                case GameState.GameOver:
                    GraphicsDevice.Clear(Color.AliceBlue);

                    spriteBatch.Begin();
                    string gameover = "Game Over!";
                    int score = 0;
                    if (player1Win)
                    {
                        gameover += " Player1 Win!";
                        score = currentScore;
                    }
                    else
                    {
                        gameover += " Player2 Win";
                        score = currentScore2;
                    }
                    
                    spriteBatch.DrawString(scoreFont, gameover,
                       new Vector2((Window.ClientBounds.Width / 2)
                           - (scoreFont.MeasureString(gameover).X / 2),
                           (Window.ClientBounds.Height / 2)
                           - (scoreFont.MeasureString(gameover).Y / 2)),
                           Color.SaddleBrown);

                    gameover = "(Press ENTER to exit)";
                    spriteBatch.DrawString(scoreFont, gameover,
                       new Vector2((Window.ClientBounds.Width / 2)
                           - (scoreFont.MeasureString(gameover).X / 2),
                           (Window.ClientBounds.Height / 2)
                           - (scoreFont.MeasureString(gameover).Y / 2) + 60),
                           Color.SaddleBrown);
                    spriteBatch.End();
                    break;
                #endregion
            }

            base.Draw(gameTime);
        }
        #endregion
        

        public void AddScore(int score)
        {
            currentScore += score;
        }

        public void AddScore2(int score)
        {
            currentScore2 += score;
        }

        public void PlayCue(string CueName)
        {
            soundBank.PlayCue(CueName);
        }
    }
}
