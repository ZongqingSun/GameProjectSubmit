using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedSprites
{
    class BouncingSprite:AutomatedSprite
    {
        public BouncingSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, string collisionCueName, int scoreValue)
            :base(textureImage, position, frameSize, collisionOffset, 
        currentFrame, sheetSize, speed,collisionCueName,scoreValue)
        {
        }

        public BouncingSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, int millisecondsPerFrame, string collisionCueName, int scoreValue)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, millisecondsPerFrame,collisionCueName,scoreValue)
        { 
        }

        public override Vector2 direction
        {
            get { return speed; }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            if (position.X < 0)
                speed.X *= -1;
            if (position.Y < 0)
                speed.Y *= -1;
            if (position.X > clientBounds.Width - frameSize.X)
            {
                speed.X *= -1 ;
            }
            if (position.Y > clientBounds.Height - frameSize.Y)
            {
                speed.Y *= -1;
            }
            base.Update(gameTime, clientBounds);
        }
    }
}
