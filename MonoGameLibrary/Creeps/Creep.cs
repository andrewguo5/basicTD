using MonoGameLibrary.Paths;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MonoGameLibrary.Creeps
{
    public class Creep
    {
        public Path Path;
        private float CurrentDistance;
        public Vector2 CurrentPosition;
        private float RemainingDistance => Path.Length - CurrentDistance;
        private int Health = 3;
        private float Speed;
        private AnimatedSprite AnimatedSprite;
        private Queue<DamageTimer> DamageTimers = new();
        private float DamageRecency = 0.0f;
        public bool Alive { get; private set; }
        public bool Dead => !Alive && (DamageRecency <= 0.0f);

        public Creep(Path path, float speed, AnimatedSprite animatedSprite)
        {
            Path = path;
            CurrentPosition = path.StartingPoint;
            CurrentDistance = 0.0f;
            Speed = speed;
            AnimatedSprite = animatedSprite;
            Alive = true;
        }

        public void TakeDamage(int damage, float delaySeconds)
        {
            DamageTimers.Enqueue(new DamageTimer(damage, delaySeconds));
        }

        public void Die()
        {
            Speed = 0;
            Alive = false;
        }

        public void Update(GameTime gameTime)
        {
            Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            AnimatedSprite.Update(gameTime);
        }

        public void Update(float seconds)
        {
            // Calculate the distance to move based on speed and elapsed time
            float distanceToMove = Speed * seconds;

            if (distanceToMove > RemainingDistance)
            {
                // If the distance to move exceeds the remaining distance, loop back
                float residualDistance = distanceToMove - RemainingDistance;
                CurrentPosition = Path.ComputePositionFromDistance(residualDistance);
                CurrentDistance = residualDistance; // Reset current distance to the residual distance
            }
            else
            {
                float newDistance = CurrentDistance + distanceToMove;
                CurrentPosition = Path.ComputePositionFromDistance(newDistance);
                CurrentDistance = newDistance;
            }

            Queue<DamageTimer> newQueue = new();
            while (DamageTimers.Count > 0)
            {
                DamageTimer timer = DamageTimers.Dequeue();
                timer.Tick(seconds);
                if (timer.Expired)
                {
                    DamageRecency = 1.0f;
                    Health -= timer.Damage;
                }
                else
                {
                    newQueue.Enqueue(timer);
                }
            }
            DamageTimers = newQueue;

            DamageRecency = Math.Max(0.0f, DamageRecency - 4f * seconds);

            if (Health <= 0)
            {
                Die();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D whitePixel, bool debug)
        {
            // Lerp between red and white based on DamageRecency (1 = recent damage, 0 = no recent damage)
            if (Alive)
            {
                Color color = Color.Lerp(Color.White, Color.Blue, DamageRecency);
                AnimatedSprite.Draw(spriteBatch, CurrentPosition, color);
            }
            else
            {
                Color color = Color.Lerp(Color.Transparent, Color.Blue, DamageRecency);
                AnimatedSprite.Draw(spriteBatch, CurrentPosition, color);
            }

            if (debug)
            {
                // Draw a red square for debug purposes
                spriteBatch.Draw(
                    whitePixel,                 // texture
                    CurrentPosition,            // position
                    null,                       // sourceRectangle
                    Color.Red,                  // color
                    0.0f,                       // rotation
                    Vector2.Zero,               // origin
                    Vector2.One * 20.0f,        // scale
                    SpriteEffects.None,         // effects
                    0f                          // layerDepth
                );
            }
        }
    }

    public class DamageTimer
    {
        private float RemainingTime;
        public bool Expired => RemainingTime < 0;
        public int Damage;

        public DamageTimer(int damage, float DelaySeconds)
        {
            RemainingTime = DelaySeconds;
            Damage = damage;
        }

        public void Tick(float seconds)
        {
            RemainingTime -= seconds;
        }
    }
}