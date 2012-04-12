using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedSprites
{
    abstract class Sprite
    {
        protected float originalScale = 1;
        Vector2 originalSpeed;
        public int scoreValue { get; protected set; }
        Texture2D textureImage;
        protected Point frameSize;
        Point currentFrame;
        Point sheetSize;
        int collisionOffset;
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame;
        const int defaultMillisecondsPerFrame = 16;
        protected Vector2 speed;
        protected Vector2 position;
        protected float scale = 1;
        public string collisionCueName{get;private set;}

        public Sprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset,
            Point currentFrame,
            Point sheetSize, Vector2 speed, string collisionCueName,
            int scoreValue)
            : this(textureImage, position, frameSize,
                collisionOffset, currentFrame, sheetSize, speed, 
            defaultMillisecondsPerFrame,collisionCueName,scoreValue)
        { 
        }

        public Sprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset,
            Point currentFrame,
            Point sheetSize, Vector2 speed, string collisionCueName,
            int scoreValue, float scale)
            : this(textureImage, position, frameSize,
                collisionOffset, currentFrame, sheetSize, speed,
            defaultMillisecondsPerFrame, collisionCueName, scoreValue)
        {
            this.scale = scale;
        }

        public Sprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset,
            Point currentFrame,
            Point sheetSize, Vector2 speed,
            int millisecondsPerFrame, string collisionCueName, int scoreValue)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            originalSpeed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.collisionCueName = collisionCueName;
            this.scoreValue = scoreValue;
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X,
                    frameSize.Y),
                    Color.White,
                    0,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0);
        }

        public abstract Vector2 direction
        {
            get;
        }

        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                    (int)(position.X + (collisionOffset * scale)),
                    (int)(position.Y + (collisionOffset * scale)),
                    (int)((frameSize.X - (collisionOffset * 2)) * scale),
                    (int)((frameSize.Y - (collisionOffset * 2)) * scale)
                    );
            }
        }

        public bool IsOutOfBounds(Rectangle clientRect)
        {
            if (position.X < -frameSize.X ||
                position.X > clientRect.Width ||
                position.Y < -frameSize.Y ||
                position.Y > clientRect.Height)
            {
                return true;
            }

            return false;
        }

        public Vector2 GetPosition
        {
            get { return position; }
        }

        //更改形状大小的方法
        public void ModifyScale(float modifier)
        {
            scale *= modifier;
        }

        public void ResetScale()
        {
            scale = originalScale;
        }

        //更改速度大小的方法
        public void ModifySpeed(float modifier)
        {
            speed *= modifier;
        }
        public void ResetSpeed()
        {
            speed = originalSpeed;
        }
    }
}
