#region File Description
/*
    ChasingSprit2e.cs
 *  Author: Zongqing Sun and Gan Zhang
 *  Purpose: Class for chasing sprites second edition
 */
#endregion

#region using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AnimatedSprites
{
    class ChasingSprite2 : Sprite
    {
        //a SpriteManager class，to get the player postion
        SpriteManager spriteManager;

        public ChasingSprite2(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, string collisionCueName, int scoreValue, SpriteManager spriteManager)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, collisionCueName, scoreValue)
        {
            this.spriteManager = spriteManager;
        }

        public ChasingSprite2(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, int millisecondsPerFrame, string collisionCueName, Boolean isRight,
            int scoreValue, SpriteManager spriteManager)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, millisecondsPerFrame, isRight, collisionCueName, scoreValue)
        {
            this.spriteManager = spriteManager;
        }

        public override Vector2 direction
        {
            get { return speed; }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {


            // Use the player position to move the sprite closer in 
            // the X and/or Y directions 
            Vector2 player = spriteManager.GetPlayerPosition();

      
                if (player.X < position.X)
                    position.X -= Math.Abs(speed.Y);
                else if (player.X > position.X)
                    position.X += Math.Abs(speed.Y);
            
                if (player.Y < position.Y)
                    position.Y -= Math.Abs(speed.X);
                else if (player.Y > position.Y)
                    position.Y += Math.Abs(speed.X);
            
            base.Update(gameTime, clientBounds);
        }
    }
}
