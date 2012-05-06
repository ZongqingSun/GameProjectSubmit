#region File Description
/*
    User2ControlledSprite.cs
 *  Author: Zongqing Sun and Gan Zhang
 *  Purpose: Class for player2
 */
#endregion

#region using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion


namespace AnimatedSprites
{
    class User2ControlledSprite : Sprite
    {
        //private MouseState prevMouseState;
        bool jumpState = false;
        float blockJumpSpeed = 10;
        float downSpeed = 0;
        private Boolean bullet1 = true;
        private Boolean bullet2 = false;
        private Boolean hadBullet2 = false;

        public User2ControlledSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, string collisionCueName, int scoreValue)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, collisionCueName, scoreValue)
        {

        }

        public User2ControlledSprite(Texture2D textureImage, Vector2 position,
        Point frameSize, int collisionOffset, Point currentFrame,
        Point sheetSize, Vector2 speed, int millisecondsPerFrame, Boolean isRight, string collisionCueName,
            int scoreValue)
            : base(textureImage, position, frameSize, collisionOffset,
        currentFrame, sheetSize, speed, millisecondsPerFrame, isRight, collisionCueName, scoreValue)
        {

        }

        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    inputDirection.X -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    inputDirection.X += 1;
                /* if (Keyboard.GetState().IsKeyUp(Keys.Up))
                 {
                     inputDirection.Y -= 1;
                     jumpState = true;
                 }
                 */

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {

                }

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    jumpState = true;
                }
                if (jumpState)
                {
                    blockJumpSpeed = blockJumpSpeed - 1;
                    inputDirection.Y -= blockJumpSpeed * 0.5f;
                    downSpeed = 0;
                }
               
                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
                if (gamepadState.ThumbSticks.Left.X != 0)
                    inputDirection.X += gamepadState.ThumbSticks.Left.X;
                if (gamepadState.ThumbSticks.Left.Y != 0)
                    inputDirection.Y += gamepadState.ThumbSticks.Left.Y;

                return inputDirection * speed;
            }
        }


        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {

            position += direction;

            if (position.Y > clientBounds.Height - frameSize.Y)
            {
                position.Y = clientBounds.Height - frameSize.Y;
                blockJumpSpeed = 10;
                downSpeed = 0;
                jumpState = false;
            }
            position.X = MathHelper.Clamp(position.X, 0, clientBounds.Width - frameSize.X);
            position.Y = MathHelper.Clamp(position.Y, 0, clientBounds.Height - frameSize.Y);





            base.Update(gameTime, clientBounds);
        }

        public float JumpSpeed
        {
            get { return blockJumpSpeed; }
            set { blockJumpSpeed = value; }
        }

        public Boolean JumpState
        {
            get { return jumpState; }
            set { jumpState = value; }
        }

        public float DownSpeed
        {
            get { return downSpeed; }
            set { downSpeed = value; }
        }

        public Boolean Bullet1
        {
            get { return bullet1; }
            set { bullet1 = value; }
        }

        public Boolean Bullet2
        {
            get { return bullet2; }
            set { bullet2 = value; }
        }

        public Boolean HadBullet2
        {
            get { return hadBullet2; }
            set { hadBullet2 = value; }
        }
    }
}
