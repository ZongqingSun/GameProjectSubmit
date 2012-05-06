#region File Description
/*
   Particle.cs
 *  Author: Zongqing Sun and Gan Zhang
 *  Purpose: A simple particle class
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
    class Particles
    {
        private Texture2D particleTexture;
        private Vector2 position, origin;
        private int lifespan;
        private float decay;
        private bool isDead = false;
        private Color color;
        private int age;
        private byte value;
        private int depth;
        private float scale;
        private float rotate;
        private float angle;

        public Particles(Texture2D particleTexture,Vector2 position, Vector2 origin,
            int depth, int age, 
            Color color,float rotate, 
            float angle,int lifespan)
            
        {
            this.particleTexture = particleTexture;
            this.position = position;
            this.origin = origin;
            this.depth = depth;
            this.scale = this.depth/100f;
            this.decay = this.depth / 30f;
            if (this.decay < 1)
                decay = 1;
            this.age = age;
            this.color = color;
            this.rotate = rotate;
            this.angle = angle;
            this.lifespan = lifespan;
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
         
            if (!isDead)
            {
                age += (int)decay;
                angle += rotate;

                position.Y += age;

                if (age >= lifespan)
                {
                    isDead = true;
                    ResetAge();
                }
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(particleTexture, 
                position,
                null,
                color,
                angle, 
                origin, 
                scale, 
                SpriteEffects.None, 
                0);
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public void ResetAge()
        {
            age = 0;
        }

        public Boolean IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }
    }
}
