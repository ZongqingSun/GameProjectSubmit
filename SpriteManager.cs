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


namespace AnimatedSprites
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        UserControlledSprite player;
        List<Sprite> spriteList = new List<Sprite>();
        List<AutomatedSprite> livesList = new List<AutomatedSprite>();
        //能力提升的变量
        int powerUpExpiration = 0;

        //使精灵产生间隔减小
        int nextSpawnTimeChange = 5000;
        int timeSinceLastSpawnChange = 0;

        //各种精灵的分数值
        int automatedSpritePointValue = 10;
        int chasingSpritePointValue = 20;
        int evadingSpritePointValue = 0;

        //表示各种精灵生成的几率
        int likelihoodAutomated = 75;
        int likelihoodChasing = 20;
        int likelihoodEvading = 5;

        //用于量产精灵
        int enemySpawnMinMilliseconds = 1000;
        int enemySpawnMaxMilliseconds = 2000;
        int enemyMinSpeed = 2;
        int enemyMaxSpeed = 6;

        //下一个敌人精灵产生的时间，初始化为0
        int nextSpawnTime = 0;

        public SpriteManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            ResetSpawnTime();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            player = new UserControlledSprite(
                Game.Content.Load<Texture2D>(@"Images/threerings"),
                new Vector2(Game.Window.ClientBounds.Width/2,
                    Game.Window.ClientBounds.Height/2),
                new Point(75,75),
                10,
                new Point(0,0),
                new Point(6,8),
                new Vector2(6,6),
                "",0);

            for (int i = 0; i < ((Game1)Game).NumberLivesRemaining; ++i)
            {
                int offset = 10 + i * 40;
                livesList.Add(new AutomatedSprite(
                    Game.Content.Load<Texture2D>(@"images\threerings"),
                    new Vector2(offset,35), new Point(75,75),10,
                    new Point(0,0), new Point(6,8), Vector2.Zero,
                    null,0,.5f));
            }

                base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            nextSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextSpawnTime < 0)
            {
                SpawnEnemy();

                // Rest spawn timer
                ResetSpawnTime();
            }

            UpdateSprites(gameTime);

            AdjustSpawnTimes(gameTime);

            CheckPowerUpExpiration(gameTime);

            base.Update(gameTime);
        }

        protected void UpdateSprites(GameTime gameTime)
        {
            // update player
            player.Update(gameTime, Game.Window.ClientBounds);

            //Update all sprites
            for (int i = 0; i < spriteList.Count; ++i)
            {
                Sprite s = spriteList[i];
                s.Update(gameTime, Game.Window.ClientBounds);

                //与玩家控制元素碰撞时发生的行为
                if (s.collisionRect.Intersects(player.collisionRect))
                {
                    //Play collision sound
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
                    else if(s.collisionCueName == "pluscollision")
                    {
                        //形状加大2倍
                        powerUpExpiration = 5000;
                        player.ModifyScale(2);
                    }
                    else if (s.collisionCueName == "skullcollision")
                    { 
                        //速度减半
                        powerUpExpiration = 5000;
                        player.ModifySpeed(.5f);
                    }
                    else if (s.collisionCueName == "boltcollision")
                    { 
                        //速度加倍
                        powerUpExpiration = 5000;
                        player.ModifySpeed(2);
                    }
                    //移除精灵
                    spriteList.RemoveAt(i);
                    --i;
                }

                //移除超出边界的精灵
                if (s.IsOutOfBounds(Game.Window.ClientBounds))
                {
                    ((Game1)Game).AddScore(spriteList[i].scoreValue);
                    spriteList.RemoveAt(i);
                    --i;
                }
            }

            foreach (Sprite sprite in livesList)
                sprite.Update(gameTime,Game.Window.ClientBounds);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            //Draw the Player
            player.Draw(gameTime, spriteBatch);

            //Draw all sprites
            foreach (Sprite s in spriteList)
                s.Draw(gameTime, spriteBatch);

            foreach (Sprite sprite in livesList)
                sprite.Draw(gameTime,spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //重置敌人出现的时间
        private void ResetSpawnTime()
        {
            nextSpawnTime = ((Game1)Game).rnd.Next(enemySpawnMinMilliseconds, 
                enemySpawnMaxMilliseconds);
        }

        //产生精灵的方法
        private void SpawnEnemy()
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;

            //默认框架大小
            Point frameSize = new Point(75,75);

            //随机选择将敌人出现在屏幕哪个位置，
            //之后随机在屏幕边缘选择一个位置并且随机产生一个速度
            switch (((Game1)Game).rnd.Next(4))
            { 
                case 0: //从左到右
                    position = new Vector2(-frameSize.X,
                        ((Game1)Game).rnd.Next(0,Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                        - frameSize.Y));

                    speed = new Vector2(((Game1)Game).rnd.Next(enemyMinSpeed,enemyMaxSpeed),0);
                    break;

                case 1: //从右到左
                    position = new
                    Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    ((Game1)Game).rnd.Next(0,Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                    -frameSize.Y));

                    speed = new Vector2(-((Game1)Game).rnd.Next(
                        enemyMinSpeed, enemyMaxSpeed), 0);
                    break;
                
                case 2://从下到上
                    position = new Vector2(((Game1)Game).rnd.Next(0,
                        Game.GraphicsDevice.PresentationParameters.BackBufferWidth
                        - frameSize.X),
                        Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

                    speed = new Vector2(0,-((Game1)Game).rnd.Next(enemyMinSpeed,enemyMaxSpeed));
                    break;

                case 3: //从上到下
                    position = new Vector2(((Game1)Game).rnd.Next(0,Game.GraphicsDevice.PresentationParameters.BackBufferWidth
                        - frameSize.X), -frameSize.Y);

                    speed = new Vector2(0,
                        ((Game1)Game).rnd.Next(enemyMinSpeed,
                        enemyMaxSpeed));
                    break;
            }

            //随机产生精灵
            int random = ((Game1)Game).rnd.Next(100);
            if (random < likelihoodAutomated)
            { 
                //产生 AutomatedSprite
                if (((Game1)Game).rnd.Next(2) == 0)
                {
                    //产生fourblades
                    spriteList.Add(
                        new AutomatedSprite(Game.Content.Load<Texture2D>(@"images\fourblades"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 8), speed, "fourbladescollision", automatedSpritePointValue));
                }
                else
                {
                    //产生threeblades
                    spriteList.Add(
                        new AutomatedSprite(Game.Content.Load<Texture2D>(@"images\threeblades"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 8), speed, "threebladescollision", automatedSpritePointValue));
                }
                
            }
            else if (random < likelihoodAutomated + likelihoodChasing)
            {
                //创建ChasingSprite
                if (((Game1)Game).rnd.Next(2) == 0)
                {
                    //创建skull
                    spriteList.Add(
                        new ChasingSprite(Game.Content.Load<Texture2D>(@"images\skullball"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 8), speed, "skullcollision", chasingSpritePointValue, this));
                }
                else
                {
                    //创建plus
                    spriteList.Add(
                        new ChasingSprite(Game.Content.Load<Texture2D>(@"images\plus"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 4), speed, "pluscollision", chasingSpritePointValue, this));
                }
            }
            else
            {
                spriteList.Add(
                new EvadingSprite(Game.Content.Load<Texture2D>(@"images\bolt"),
                    position, new Point(75, 75), 10, new Point(0, 0),
                    new Point(6, 8), speed, "boltcollision", evadingSpritePointValue, this, .75f, 150));
            }
            
        }

        //得到player的位置信息
        public Vector2 GetPlayerPosition()
        {
            return player.GetPosition;
        }

        //调整敌人出现间隔的方法
        protected void AdjustSpawnTimes(GameTime gameTime)
        { 
            //如果时间间隔大于500毫秒，就减少它
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

        //检测行之变化的过期时间
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
    }
}
