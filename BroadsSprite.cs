#region File Description
/*
    BroadSprite.cs
 *  Author: Zongqing Sun and Gan Zhang
 *  Purpose: Class for board sprites
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
    class BroadsSprite:Sprite
    {
        Boolean isHit = false;
        public BroadsSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, string collisionCueName, int scoreValue)
            :base(textureImage, position, frameSize, collisionOffset, 
        currentFrame, sheetSize, speed, collisionCueName,scoreValue)
        {
        }

        public BroadsSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, string collisionCueName, int scoreValue, float scale)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, collisionCueName, scoreValue,scale)
        {
        }

        public BroadsSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, int millisecondsPerFrame, Boolean isRight,string collisionCueName, int scoreValue)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, millisecondsPerFrame,isRight,collisionCueName,scoreValue)
        { 
        }

        public override Vector2 direction
        {
            get { return speed; }
        }

        public Boolean hit
        {
            get { return isHit; }
            set { isHit = value; }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            position += direction;

            base.Update(gameTime, clientBounds);
        }
    }
}
