#region File Description
/*
    SpriteManager.cs
 *  Author: Zongqing Sun and Gan Zhang
 *  Purpose: Used to control all the sprites' actions.
 */
#endregion

#region using statements
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
#endregion

namespace AnimatedSprites
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region variables declaration
        SpriteBatch spriteBatch;
        UserControlledSprite player;
        User2ControlledSprite player2;
        Boolean isHit = false;
        int particleLife = 0;

        int broadCount = 12;

        //Particles
        int particlesSpeedTime = 30;
        Texture2D smokeflakes;
        List<Particles> particleList;
        int maxCount = 200;

        //action point for player
        Point playerRunFramePoint = new Point(0, 2);
        Point playerSprintFramePoint = new Point(0, 3);
        Point playerRunFramePointRight = new Point(19, 2);
        Point playerSprintFramePointRight = new Point(19, 3);

        Point playerSprintSheetSize = new Point(17, 1);
        Point playerRunSheetSize = new Point(16, 1);
        Point playerIdleSheetSize = new Point(9, 1);

        Point playerIdleFramePointRight = new Point(19, 0);
        Point playerIdleFramePoint = new Point(0, 0);

        List<BroadsSprite> broadsList = new List<BroadsSprite>();
        List<Bullets> bulletsList = new List<Bullets>();
        // List<Vector2> broads = new List<Vector2>();
        List<Sprite> spriteList = new List<Sprite>();
        List<AutomatedSprite> livesList = new List<AutomatedSprite>();
        List<AutomatedSprite> livesList2 = new List<AutomatedSprite>();
        KeyboardState prevKeyboard;
        //bullets
        Texture2D TextureForBullet1;
        Texture2D TextureForBullet2;

        //power up variable
        int powerUpExpiration = 0;
 
        int nextSpawnTimeChange = 5000;
        int timeSinceLastSpawnChange = 0;

        //sprites' scores
        int automatedSpritePointValue = 10;
        int chasingSpritePointValue = 20;
        int evadingSpritePointValue = 0;

        //The probabilities for enemies
        int likelihoodAutomated = 75;
        int likelihoodChasing = 20;
        int likelihoodEvading = 5;

        //For creating enemies
        int enemySpawnMinMilliseconds = 1000;
        int enemySpawnMaxMilliseconds = 2000;
        int enemyMinSpeed = 2;
        int enemyMaxSpeed = 6;

        //For creating broads
        int broadSpawnMinMilliseconds = 800;
        int broadSpawnMaxMilliseconds = 1200;
        int broadMinSpeed = 2;
        int broadMaxSpeed = 5;

        int bulletCreateMilliseconds = 500;
        int bulletTimeSinceLastSpawnChange = 0;

        int downAccTime = 50;

        //The time between two enemies appearence
        int nextSpawnTime = 0;

        //The time between two broads appearence
        int nextBroadTime = 0;
        #endregion
        

        
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>  
        #region Initialization
        public SpriteManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public override void Initialize()
        {
            ResetSpawnTime();
            particleList = new List<Particles>(maxCount);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            //load smoke image
            smokeflakes = Game.Content.Load<Texture2D>(@"images/smoke");

            //create initial borads
            int x = 100, y = 100;
            for (int i = 1; i < broadCount; i++)
            {
                if (i == 5)
                {
                    x -= 30;
                }
                broadsList.Add(new BroadsSprite(
                    Game.Content.Load<Texture2D>(@"Images/broad"),
                    new Vector2(x, y), new Point(100, 30), 0,
                    new Point(0, 0), new Point(1, 1), Vector2.Zero,
                    null, 0, 0.5f));

                broadsList.Add(new BroadsSprite(
                    Game.Content.Load<Texture2D>(@"Images/broad"),
                    new Vector2(Game.Window.ClientBounds.Width - x, y), new Point(100, 30), 0,
                    new Point(0, 0), new Point(1, 1), Vector2.Zero,
                    null, 0, 0.5f));

                x += 120;
                y += 80;

            }

            //load bullet image
            TextureForBullet1 = Game.Content.Load<Texture2D>(@"Images/skullball");

            //create players
            player = new UserControlledSprite(
                Game.Content.Load<Texture2D>(@"Images/character"),
                new Vector2(Game.Window.ClientBounds.Width / 2,
                    Game.Window.ClientBounds.Height - 64),
                new Point(64, 64),
                10,
                playerIdleFramePoint,
                new Point(9, 1),
                new Vector2(6, 6),
                30,
                false,
                "", 0);
            player2 = new User2ControlledSprite(
                Game.Content.Load<Texture2D>(@"Images/user2character"),
                new Vector2(20,
                    Game.Window.ClientBounds.Height - 64),
                new Point(64, 64),
                10,
                playerIdleFramePoint,
                new Point(9, 1),
                new Vector2(6, 6),
                30,
                false,
                "", 0);

            //Life icon
            for (int i = 0; i < ((Game1)Game).NumberLivesRemaining; ++i)
            {
                int offset = 10 + i * 40;
                livesList.Add(new AutomatedSprite(
                    Game.Content.Load<Texture2D>(@"images\threerings"),
                    new Vector2(offset, 35), new Point(75, 75), 10,
                    new Point(0, 0), new Point(6, 8), Vector2.Zero,
                    null, 0, .5f));
            }
            for (int i = 0; i < ((Game1)Game).NumberLivesRemaining2; ++i)
            {
                int offset = 950 - i * 40;
                livesList2.Add(new AutomatedSprite(
                    Game.Content.Load<Texture2D>(@"images\threerings"),
                    new Vector2(offset, 35), new Point(75, 75), 10,
                    new Point(0, 0), new Point(6, 8), Vector2.Zero,
                    null, 0, .5f));
            }
            base.LoadContent();
        }
        #endregion
        

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        #region update and draw
        public override void Update(GameTime gameTime)
        {
            nextSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
            nextBroadTime -= gameTime.ElapsedGameTime.Milliseconds;
            bulletTimeSinceLastSpawnChange += gameTime.ElapsedGameTime.Milliseconds;
            downAccTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextSpawnTime < 0)
            {
                SpawnEnemy();

                // Rest spawn timer
                ResetSpawnTime();
            }

            if (nextBroadTime < 0)
            {
                SpawnBoard();

                ResetBroadTime();
            }

            UpdateSprites(gameTime);

            AdjustSpawnTimes(gameTime);

            CheckPowerUpExpiration(gameTime);
            CheckParticleLife(gameTime);

            base.Update(gameTime);
        }

        protected void UpdateSprites(GameTime gameTime)
        {
            // update player
            player.Update(gameTime, Game.Window.ClientBounds);
            player2.Update(gameTime, Game.Window.ClientBounds);

            CheckCollisionsForBoards(gameTime, player);
            CheckCollisionsForBoards2(gameTime, player2);

            PlayerActionCheck(player);
            PlayerActionCheck2(player2);

            //Update all sprites

            for (int i = 0; i < bulletsList.Count; ++i)
            {
                Bullets b = bulletsList[i];
                b.Update(gameTime, Game.Window.ClientBounds);
            }

            CheckCollisionsForSprites(gameTime);
            CheckCollisionsForPlayer2(gameTime);


            UpdateBullets();

            UpdateParticles(gameTime);

            foreach (Sprite sprite in livesList)
                sprite.Update(gameTime, Game.Window.ClientBounds);
            foreach (Sprite sprite in livesList2)
                sprite.Update(gameTime, Game.Window.ClientBounds);
            foreach (BroadsSprite bs in broadsList)
                bs.Update(gameTime, Game.Window.ClientBounds);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            //Draw the Player
            player.Draw(gameTime, spriteBatch);
            player2.Draw(gameTime, spriteBatch);
            //Draw all sprites
            foreach (Sprite s in spriteList)
                s.Draw(gameTime, spriteBatch);

            foreach (Sprite sprite in livesList)
                sprite.Draw(gameTime, spriteBatch);

            foreach (Sprite sprite in livesList2)
                sprite.Draw(gameTime, spriteBatch);

            foreach (Sprite bullet in bulletsList)
                bullet.Draw(gameTime, spriteBatch);
            foreach (Sprite broad in broadsList)
                broad.Draw(gameTime, spriteBatch);
            foreach (Particles p in particleList)
                p.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
        

        #region player actions

        #region check two players movements
        private void PlayerActionCheck(UserControlledSprite Player)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (GetPlayerOriginalFrame() != playerRunFramePoint)
                {
                    ChangeState(Game.Content.Load<Texture2D>(@"Images/character"), playerRunFramePoint, playerRunSheetSize, false, Player);
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Right) && prevKeyboard.IsKeyUp(Keys.Right) == false)
            {
                if (GetPlayerOriginalFrame() != playerIdleFramePoint)
                {
                    ChangeState(Game.Content.Load<Texture2D>(@"Images/character"), playerIdleFramePoint, playerIdleSheetSize, false, Player);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (GetPlayerOriginalFrame() != playerRunFramePointRight)
                {
                    ChangeState(Game.Content.Load<Texture2D>(@"Images/character2"), playerRunFramePointRight, playerRunSheetSize, true, Player);
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Left) && prevKeyboard.IsKeyUp(Keys.Left) == false)
            {
                if (GetPlayerOriginalFrame() != playerIdleFramePointRight)
                {
                    ChangeState(Game.Content.Load<Texture2D>(@"Images/character2"), playerIdleFramePointRight, playerIdleSheetSize, true, Player);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && bulletTimeSinceLastSpawnChange > bulletCreateMilliseconds)
            {
                bulletTimeSinceLastSpawnChange = 0;
                BulletsCreate();
                prevKeyboard = Keyboard.GetState();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                ChangeBullet(Player.Bullet1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.P) && Player.HadBullet2)
            {
                ChangeBullet(Player.Bullet2);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L) && Player.HadBullet2)
            {
                ChangeBullet(Player.Bullet1);
                Player.HadBullet2 = false;
                Player.ResetSpeed();
                Vector2 position = new Vector2(Player.Position.X-100,Player.Position.Y-100);
                Vector2 speed = Player.Speed*(-1);
                spriteList.Add(
                        new EvadingSprite(Game.Content.Load<Texture2D>(@"images\plus"),
                           position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 4), speed, "pluscollision", chasingSpritePointValue, this, 0.2f, 100));
                
            }
        }

        private void PlayerActionCheck2(User2ControlledSprite Player)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (Player.GetOriginalCurrentFrame != playerRunFramePoint)
                {
                    ChangeState2(Game.Content.Load<Texture2D>(@"Images/user2character"), playerRunFramePoint, playerRunSheetSize, false, Player);
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.D) && prevKeyboard.IsKeyUp(Keys.D) == false)
            {
                if (Player.GetOriginalCurrentFrame != playerIdleFramePoint)
                {
                    ChangeState2(Game.Content.Load<Texture2D>(@"Images/user2character"), playerIdleFramePoint, playerIdleSheetSize, false, Player);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (Player.GetOriginalCurrentFrame != playerRunFramePointRight)
                {
                    ChangeState2(Game.Content.Load<Texture2D>(@"Images/user2character2"), playerRunFramePointRight, playerRunSheetSize, true, Player);
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.A) && prevKeyboard.IsKeyUp(Keys.A) == false)
            {
                if (Player.GetOriginalCurrentFrame != playerIdleFramePointRight)
                {
                    ChangeState2(Game.Content.Load<Texture2D>(@"Images/user2character2"), playerIdleFramePointRight, playerIdleSheetSize, true, Player);
                }
            }
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && bulletTimeSinceLastSpawnChange > bulletCreateMilliseconds)
            {
                bulletTimeSinceLastSpawnChange = 0;
                BulletsCreate();
                prevKeyboard = Keyboard.GetState();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                ChangeBullet(Player.Bullet1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2) && Player.HadBullet2)
            {
                ChangeBullet(Player.Bullet2);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3) && Player.HadBullet2)
            {
                ChangeBullet(Player.Bullet1);
                player.HadBullet2 = false;
                player.ResetSpeed();
            }*/
        }
        #endregion

        #region the function to change player state
        private void ChangeState(Texture2D Texture, Point FramePoint, Point sheetSize, Boolean right, UserControlledSprite Player)
        {
            Player.TextImage = Texture;
            Player.GetOriginalCurrentFrame = FramePoint;
            Player.SheetSize = sheetSize;
            Player.IsRightCheck = right;
            prevKeyboard = Keyboard.GetState();
        }

        private void ChangeState2(Texture2D Texture, Point FramePoint, Point sheetSize, Boolean right, User2ControlledSprite Player)
        {
            Player.TextImage = Texture;
            Player.GetOriginalCurrentFrame = FramePoint;
            Player.SheetSize = sheetSize;
            Player.IsRightCheck = right;
            prevKeyboard = Keyboard.GetState();
        }
        #endregion


        //check the collisions
        #region check collisions for players
        private void CheckCollisionsForBoards(GameTime gameTime, UserControlledSprite player)
        {
            Boolean hit = false;
            BroadsSprite b = broadsList[1];
            //change the players rectangle to a bigger one
            Rectangle forPlayer = new Rectangle(player.collisionRect.X - 10, player.collisionRect.Y - 10,
                player.collisionRect.Width + 20, player.collisionRect.Height + 20);

            for (int i = 0; i < broadsList.Count; i++)
            {
                BroadsSprite b1 = broadsList[i];
                if (b1.collisionRect.Intersects(forPlayer))
                {
                    if (player.FrameSize.Y >= b1.Position.Y - player.Position.Y && b1.Position.Y > player.Position.Y + 50)
                    {
                        hit = true;
                        b = b1;
                    }
                }
            }
            if (hit)
            {
                player.DownSpeed = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && GetPlayerPosition().Y == b.Position.Y - player.FrameSize.Y + 1)
                {
                    player.JumpSpeed = 10;
                    player.JumpState = true;

                }
                else if ((player.JumpSpeed < 0 || player.JumpSpeed == 10) && player.FrameSize.Y >= b.Position.Y - player.Position.Y && b.Position.Y > player.Position.Y + 50)
                {
                    player.Position = new Vector2(player.Position.X, b.Position.Y - player.FrameSize.Y + 1);
                    player.JumpSpeed = 10;
                    player.JumpState = false;
                }

            }
            else
            {
                if (!player.JumpState)
                {
                    if (player.Position.Y < Game.Window.ClientBounds.Height - player.FrameSize.Y)
                    {

                        player.DownSpeed = player.DownSpeed - 0.8f;
                        player.Position = new Vector2(player.Position.X, player.Position.Y - player.DownSpeed);
                    }
                }
            }
            hit = false;


        }

        private void CheckCollisionsForBoards2(GameTime gameTime, User2ControlledSprite player)
        {
            Boolean hit = false;
            BroadsSprite b = broadsList[1];
            //change the players rectangle to a bigger one
            Rectangle forPlayer = new Rectangle(player.collisionRect.X - 10, player.collisionRect.Y - 10,
                player.collisionRect.Width + 20, player.collisionRect.Height + 20);

            for (int i = 0; i < broadsList.Count; i++)
            {
                BroadsSprite b1 = broadsList[i];
                if (b1.collisionRect.Intersects(forPlayer))
                {
                    if (player.FrameSize.Y >= b1.Position.Y - player.Position.Y && b1.Position.Y > player.Position.Y + 50)
                    {
                        hit = true;
                        b = b1;
                    }
                }
            }
            if (hit)
            {
                player.DownSpeed = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && GetPlayerPosition().Y == b.Position.Y - player.FrameSize.Y + 1)
                {
                    player.JumpSpeed = 10;
                    player.JumpState = true;

                }
                else if ((player.JumpSpeed < 0 || player.JumpSpeed == 10) && player.FrameSize.Y >= b.Position.Y - player.Position.Y && b.Position.Y > player.Position.Y + 50)
                {
                    player.Position = new Vector2(player.Position.X, b.Position.Y - player.FrameSize.Y + 1);
                    player.JumpSpeed = 10;
                    player.JumpState = false;
                }

            }
            else
            {
                if (!player.JumpState)
                {
                    if (player.Position.Y < Game.Window.ClientBounds.Height - player.FrameSize.Y)
                    {

                        player.DownSpeed = player.DownSpeed - 0.8f;
                        player.Position = new Vector2(player.Position.X, player.Position.Y - player.DownSpeed);
                    }
                }
            }
            hit = false;


        }
        #endregion

        #endregion


        #region check collisions
        private void CheckCollisionsForSprites(GameTime gameTime)
        {
            for (int i = 0; i < spriteList.Count; ++i)
            {
                Sprite s = spriteList[i];
                s.Update(gameTime, Game.Window.ClientBounds);

                if (s.collisionRect.Intersects(player.collisionRect))
                {
                    //Play collision sound
                    if ((player.JumpSpeed < 0 || player.JumpSpeed == 10) && player.Position.Y <= s.Position.Y - 30)
                    {
                        player.JumpSpeed = 10;
                        player.JumpState = true;
                    }
                    else
                    {
                        if (s.collisionCueName != null)
                        {
                            ((Game1)Game).PlayCue(s.collisionCueName);
                        }
                        if (s is AutomatedSprite)
                        {
                            if (livesList.Count > 0)
                            {
                                livesList.RemoveAt(livesList.Count - 1);
                                --((Game1)Game).NumberLivesRemaining;
                            }
                        }
                        else if (s.collisionCueName == "pluscollision")
                        { 
                            //load bullet 2
                            if (!player.HadBullet2)
                            {
                                player.HadBullet2 = true;
                                ChangeBullet(player.Bullet2);
                                TextureForBullet2 = s.TextImage;
                                player.ModifySpeed(.6f);
                            }
                        }
                        else if (s.collisionCueName == "skullcollision")
                        {
                            //Speed multi by 0.5
                            powerUpExpiration = 5000;
                            player.ModifySpeed(.5f);
                        }
                        else if (s.collisionCueName == "boltcollision")
                        {
                            //Speed multi by 2
                            powerUpExpiration = 5000;
                            player.ModifySpeed(2);
                        }
                        else if (s.collisionCueName == "toussez3")
                        {
                            if (livesList.Count > 0)
                            {
                                livesList.RemoveAt(livesList.Count - 1);
                                --((Game1)Game).NumberLivesRemaining;
                            }
                        }
                    }
                    //remove sprites
                    spriteList.RemoveAt(i);
                    --i;

                }

                //remove sprites that out of bounds
                if (s.IsOutOfBounds(Game.Window.ClientBounds))
                {
                    ((Game1)Game).AddScore(spriteList[i].scoreValue);
                    spriteList.RemoveAt(i);
                    --i;
                }

                //collsions among bullets and enemies
                for (int j = 0; j < bulletsList.Count; j++)
                {
                    if (s.collisionRect.Intersects(bulletsList[j].collisionRect))
                    {
                        if (particleList.Count < maxCount)
                        {
                            particleList.Add(new Particles(Game.Content.Load<Texture2D>(@"images/smoke"), spriteList[i].Position, new Vector2(64, 64),
                                ((Game1)Game).rnd.Next(1, 20), 0, Color.Blue, ((float)((Game1)Game).rnd.Next(1, 10)) / 1000f, 0, 100));
                        }
                        bulletsList.RemoveAt(j);
                        if (i < spriteList.Count && i >= 0)
                        {
                            spriteList.RemoveAt(i);
                            --i;
                            break;
                        }
                        // ((Game1)Game).AddScore(spriteList[i].scoreValue);

                        --j;
                        

                    }

                }

            }
        }

        //chenc collisions for player2
        private void CheckCollisionsForPlayer2(GameTime gameTime)
        {
            for (int i = 0; i < bulletsList.Count; i++)
            {
                Bullets b = bulletsList[i];
                b.Update(gameTime,Game.Window.ClientBounds);
                if (b.collisionRect.Intersects(player2.collisionRect))
                {
                    if (b.collisionCueName != null)
                    {
                        ((Game1)Game).PlayCue(b.collisionCueName);
                    }
                    livesList2.RemoveAt(livesList2.Count - 1);
                    ((Game1)Game).NumberLivesRemaining2--;
                    bulletsList.RemoveAt(i);
                    isHit = true;
                    particleLife = 2000;
                }
                if(i < bulletsList.Count)
                if (bulletsList[i].IsOutOfBounds(Game.Window.ClientBounds))
                {
                    ((Game1)Game).AddScore2(20);
                    bulletsList.RemoveAt(i);
                    --i;
                }
            }

            Boolean createEnemy = false;
            for (int i = 0; i < spriteList.Count; i++)
            {
                Sprite s = spriteList[i];
                //s.Update(gameTime, Game.Window.ClientBounds);
                if (s.collisionRect.Intersects(player2.collisionRect))
                {
                    

                    if (s.collisionCueName == "pluscollision")
                    {
                        
                        createEnemy = true;
                        spriteList.RemoveAt(i);
                        i--;

                        if (s.collisionCueName != null)
                        {
                            ((Game1)Game).PlayCue(s.collisionCueName);
                        }
                    }

                }
            }

            if (createEnemy)
            {
                Vector2 speed = Vector2.Zero;
                if (player2.Position.Y > player.Position.Y)
                {
                    speed += new Vector2(0, -((Game1)Game).rnd.Next(enemyMinSpeed, enemyMaxSpeed));
                }
                else
                {
                    speed += new Vector2(0,
                ((Game1)Game).rnd.Next(enemyMinSpeed,
                enemyMaxSpeed));
                }

                if (player2.Position.X > player.Position.X)
                {
                    speed += new Vector2(-((Game1)Game).rnd.Next(
                enemyMinSpeed, enemyMaxSpeed), 0);
                }
                else
                {
                    speed += new Vector2(((Game1)Game).rnd.Next(enemyMinSpeed, enemyMaxSpeed), 0);
                }
                spriteList.Add(
                new ChasingSprite2(Game.Content.Load<Texture2D>(@"images\penguin"),
                    player2.Position, new Point(56, 80), 10, new Point(0, 0),
                    new Point(3, 9), speed, "toussez3", chasingSpritePointValue, this));
            }
        }
        #endregion


        #region Helper Functions
        //Reset enemies spawn time
        private void ResetSpawnTime()
        {
            nextSpawnTime = ((Game1)Game).rnd.Next(enemySpawnMinMilliseconds,
                enemySpawnMaxMilliseconds);
        }

        //set the borad spawn time
        private void ResetBroadTime()
        {
            nextBroadTime = ((Game1)Game).rnd.Next(broadSpawnMinMilliseconds,
                broadSpawnMaxMilliseconds);
        }

        //Particles update
        private void UpdateParticles(GameTime gameTime)
        {
            particlesSpeedTime -= gameTime.ElapsedGameTime.Milliseconds;
            for (int i = 0; i < particleList.Count; i++)
            {
                Particles p = particleList[i];
                p.Update(gameTime);
                if (p.IsDead)
                {
                    particleList.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (particlesSpeedTime <= 0)
                    {

                        particlesSpeedTime = 30;
                    }
                }
            }
        }

        //funtion to create enemy
        private void SpawnEnemy()
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;

            //default framesize
            Point frameSize = new Point(75, 75);

            switch (((Game1)Game).rnd.Next(4))
            {
                case 0: //From left to right
                    position = new Vector2(-frameSize.X,
                        ((Game1)Game).rnd.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                        - frameSize.Y));

                    speed = new Vector2(((Game1)Game).rnd.Next(enemyMinSpeed, enemyMaxSpeed), 0);
                    break;

                case 1: //Right to left
                    position = new
                    Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    ((Game1)Game).rnd.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                    - frameSize.Y));

                    speed = new Vector2(-((Game1)Game).rnd.Next(
                        enemyMinSpeed, enemyMaxSpeed), 0);
                    break;

                case 2://Buttom to top
                    position = new Vector2(((Game1)Game).rnd.Next(0,
                        Game.GraphicsDevice.PresentationParameters.BackBufferWidth
                        - frameSize.X),
                        Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

                    speed = new Vector2(0, -((Game1)Game).rnd.Next(enemyMinSpeed, enemyMaxSpeed));
                    break;

                case 3: //Top to buttom
                    position = new Vector2(((Game1)Game).rnd.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferWidth
                        - frameSize.X), -frameSize.Y);

                    speed = new Vector2(0,
                        ((Game1)Game).rnd.Next(enemyMinSpeed,
                        enemyMaxSpeed));
                    break;
            }

            //create sprite in random
            int random = ((Game1)Game).rnd.Next(100);
            if (random < likelihoodAutomated)
            {
                //create AutomatedSprite
                if (((Game1)Game).rnd.Next(2) == 0)
                {
                    //create firefox
                    spriteList.Add(
                        new AutomatedSprite(Game.Content.Load<Texture2D>(@"images\firefox"),
                            position, new Point(56, 80), 10, new Point(0, 0),
                            new Point(3, 9), speed, "toussez3", automatedSpritePointValue));
                }
                else
                {
                    //create fox
                    spriteList.Add(
                        new AutomatedSprite(Game.Content.Load<Texture2D>(@"images\fox"),
                            position, new Point(56, 80), 10, new Point(0, 0),
                            new Point(3, 9), speed, "toussez3", automatedSpritePointValue));
                }

            }
            else if (random < likelihoodAutomated + likelihoodChasing)
            {
                //createChasingSprite
                if (((Game1)Game).rnd.Next(2) == 0)
                {
                    //create pacman
                    spriteList.Add(
                        new ChasingSprite(Game.Content.Load<Texture2D>(@"images\pacman"),
                            position, new Point(56, 80), 10, new Point(0, 0),
                            new Point(3, 9), speed, "skullcollision", chasingSpritePointValue, this));
                }
                else
                {
                    //create plus
                    spriteList.Add(
                        new EvadingSprite(Game.Content.Load<Texture2D>(@"images\plus"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 4), speed, "pluscollision", chasingSpritePointValue, this, 1f, 100));
                }
            }
            else
            {
                spriteList.Add(
                new EvadingSprite(Game.Content.Load<Texture2D>(@"images\android"),
                    position, new Point(56, 80), 10, new Point(0, 0),
                    new Point(3, 9), speed, "boltcollision", evadingSpritePointValue, this, .75f, 150));
            }
        }

        //create borads
        public void SpawnBoard()
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;

            Point frameSize = new Point(100, 30);

            switch (((Game1)Game).rnd.Next(6))
            {
                case 4: //Left to right 
                    position = new Vector2(-frameSize.X,
                        ((Game1)Game).rnd.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                        - 150));

                    speed = new Vector2(((Game1)Game).rnd.Next(broadMinSpeed, broadMaxSpeed), 0);
                    break;

                case 5: //Right to left
                    position = new
                    Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    ((Game1)Game).rnd.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                    - 150));

                    speed = new Vector2(-((Game1)Game).rnd.Next(
                        broadMinSpeed, broadMaxSpeed), 0);
                    break;
            }

            broadsList.Add(new BroadsSprite(
                    Game.Content.Load<Texture2D>(@"Images/broad"),
                    position, frameSize, 0,
                    new Point(0, 0), new Point(1, 1), speed,
                    null, 0, 0.5f));


        }

        //create bullets
        private void BulletsCreate()
        {
            Vector2 speed = new Vector2(7, 0);
            Vector2 position = new Vector2(GetPlayerPosition().X + player.FrameSize.X / 2, GetPlayerPosition().Y + player.FrameSize.Y / 2 - 5);

            //Ä¬ÈÏ¿ò¼Ü´óÐ¡
            Point frameSize = new Point(75, 75);
            if (player.IsRightCheck)
                speed = speed * -1;

            if (player.Bullet1)
            {
                bulletsList.Add(new Bullets(TextureForBullet1,
                                position, frameSize, 10, new Point(0, 0),
                                new Point(6, 8), speed, "toussez3", 0, 0.3f));
            }
            else if (player.Bullet2)
            {
                bulletsList.Add(new Bullets(TextureForBullet2,
                                position, frameSize, 10, new Point(0, 0),
                                new Point(6, 4), speed + new Vector2(0, 2), "toussez3", 0, 0.3f));
                bulletsList.Add(new Bullets(TextureForBullet2,
                                position, frameSize, 10, new Point(0, 0),
                                new Point(6, 4), speed, "toussez3", 0, 0.3f));
                bulletsList.Add(new Bullets(TextureForBullet2,
                                position, frameSize, 10, new Point(0, 0),
                                new Point(6, 4), speed + new Vector2(0, -2), "toussez3", 0, 0.3f));
            }
        }

        //set current bullets
        private void ChangeBullet(Boolean bullet)
        {
            if (player.Bullet1 == bullet)
            {
                player.Bullet1 = true;
                player.Bullet2 = false;
            }
            else if (player.Bullet2 == bullet)
            {
                player.Bullet1 = false;
                player.Bullet2 = true;
            }
        }

        //update bullet
        private void UpdateBullets()
        {
            for (var i = bulletsList.Count - 1; i >= 0; i--)
            {
                if (bulletsList[i].IsOutOfBounds(Game.Window.ClientBounds))
                {
                    bulletsList.RemoveAt(i);
                }
            }
        }

        //get player position
        public Vector2 GetPlayerPosition()
        {
            return player.Position;
        }

        public Vector2 GetPlayerPosition2()
        {
            return player2.Position;
        }

        //get player original frame
        public Point GetPlayerOriginalFrame()
        {
            return player.GetOriginalCurrentFrame;
        }

        //adjust enemies spawn time
        protected void AdjustSpawnTimes(GameTime gameTime)
        {
            if (enemySpawnMaxMilliseconds > 500)
            {
                timeSinceLastSpawnChange += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastSpawnChange > nextSpawnTimeChange)
                {
                    timeSinceLastSpawnChange -= nextSpawnTimeChange;
                    if (enemySpawnMaxMilliseconds > 1000)
                    {
                        enemySpawnMaxMilliseconds -= 100;
                        enemySpawnMinMilliseconds -= 100;
                    }
                    else
                    {
                        enemySpawnMaxMilliseconds -= 10;
                        enemySpawnMinMilliseconds -= 10;
                    }
                }
            }
        }

        protected void CheckPowerUpExpiration(GameTime gameTime)
        {
            if (powerUpExpiration > 0)
            {
                powerUpExpiration -= gameTime.ElapsedGameTime.Milliseconds;
                if (powerUpExpiration <= 0)
                {
                    powerUpExpiration = 0;
                    player.ResetScale();
                    player.ResetSpeed();
                }
            }
        }

        protected void CheckParticleLife(GameTime gameTime)
        {
            if (particleLife > 0)
            {
                particleLife -= gameTime.ElapsedGameTime.Milliseconds;
                if (particleLife <= 0)
                {
                    isHit = false;
                    particleLife = 0;
                }
            }
        }

        public Boolean IsHit()
        {
            return isHit;
        }
        #endregion
       
    }
}
