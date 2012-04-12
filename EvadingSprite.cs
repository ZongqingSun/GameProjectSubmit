using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedSprites
{
    class EvadingSprite:Sprite
    {
        //用来检测什么时候激活躲避算法
        float evasionSpeedModifier;
        int evasionRange;
        bool evade = false;

        //创建SpriteManager类，以得到玩家位置
        SpriteManager spriteManager;

        public EvadingSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed,string collisionCueName,int scoreValue, SpriteManager spriteManager,
            float evasionSpeedModifier,
            int evasionRange)
            :base(textureImage, position, frameSize, collisionOffset, 
        currentFrame, sheetSize, speed, collisionCueName,scoreValue)
        {
            this.spriteManager = spriteManager;
            this.evasionSpeedModifier = evasionSpeedModifier;
            this.evasionRange = evasionRange;
        }

        public EvadingSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, int millisecondsPerFrame,string collisionCueName,
            int scoreValue,SpriteManager spriteManager, float evasionSpeedModifier,
            int evasionRange)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, millisecondsPerFrame,collisionCueName,scoreValue)
        {
            this.spriteManager = spriteManager;
            this.evasionSpeedModifier = evasionSpeedModifier;
            this.evasionRange = evasionRange;
        }

        public override Vector2 direction
        {
            get { return speed; }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //首先，让精灵以一定的速度动起来
            position += speed;

            // Use the player position to move the sprite closer in 
            // the X and/or Y directions 
            Vector2 player = spriteManager.GetPlayerPosition();

            if (evade)
            {
                //如果玩家在竖向移动，横向移动的精灵躲避
                if (speed.X == 0)
                {
                    if (player.X < position.X)
                        position.X += Math.Abs(speed.Y);
                    else if (player.X > position.X)
                        position.X -= Math.Abs(speed.Y);
                }

                //如果玩家在横向移动，竖向移动的精灵躲避
                if (speed.Y == 0)
                {
                    if (player.Y < position.Y)
                        position.Y += Math.Abs(speed.X);
                    else if (player.Y > position.Y)
                        position.Y -= Math.Abs(speed.X);
                }
            }
            else
            { 
                if(Vector2.Distance(position,player) < evasionRange)
                {
                    //如果用户进入了应该躲避的范围，
                    //改变方向，并且修改速度
                    speed *= -evasionSpeedModifier;
                    evade = true;
                }
            }
            base.Update(gameTime, clientBounds);
        }
    }
}
