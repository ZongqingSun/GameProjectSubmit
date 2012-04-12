using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedSprites
{
    class ChasingSprite:Sprite
    {
        //创建SpriteManager类，以得到玩家位置
        SpriteManager spriteManager;

        public ChasingSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, string collisionCueName, int scoreValue,SpriteManager spriteManager)
            :base(textureImage, position, frameSize, collisionOffset, 
        currentFrame, sheetSize, speed, collisionCueName,scoreValue)
        {
            this.spriteManager = spriteManager;
        }

        public ChasingSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, int millisecondsPerFrame,string collisionCueName,
            int scoreValue,SpriteManager spriteManager)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, millisecondsPerFrame,collisionCueName,scoreValue)
        {
            this.spriteManager = spriteManager;
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
            Vector2 player = spriteManager.GetPlayerPosition( ); 

            //如果玩家在竖向移动，横向移动的精灵追逐
            if (speed.X == 0)
            {
                if (player.X < position.X)
                    position.X -= Math.Abs(speed.Y);
                else if (player.X > position.X)
                    position.X += Math.Abs(speed.Y);
            }

            //如果玩家在横向移动，竖向移动的精灵追逐
            if (speed.Y == 0)
            {
                if (player.Y < position.Y)
                    position.Y -= Math.Abs(speed.X);
                else if (player.Y > position.Y)
                    position.Y += Math.Abs(speed.X);
            }
            base.Update(gameTime, clientBounds);
        }
    }
}
