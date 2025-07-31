using MonoGameLibrary.Paths;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Graphics;
using System;

namespace MonoGameLibrary.Creeps
{
    public class Creep
    {
        public Path Path;

        private float CurrentDistance;

        public Vector2 CurrentPosition;

        private float RemainingDistance => Path.Length - CurrentDistance;

        private float Speed;

        private AnimatedSprite AnimatedSprite;

        // State to indicate that this creep is within a tower's range
        private bool TakingDamage = false;
        private float DamageRecency = 0.0f;

        public Creep(Path path, float speed, AnimatedSprite animatedSprite)
        {
            Path = path;
            CurrentPosition = path.StartingPoint;
            CurrentDistance = 0.0f;
            Speed = speed;
            AnimatedSprite = animatedSprite;
        }

        public void TakeDamage(float damage)
        {
            // Implement damage logic here
            // For example, reduce health or trigger death if health reaches zero
            TakingDamage = true;
            DamageRecency = 1.0f; // Reset the damage recency timer
        }

        public void Update(GameTime gameTime)
        {
            Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            // Update the animated sprite
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

            // Reduce damage recency timer
            DamageRecency = Math.Max(0.0f, DamageRecency - 4f * seconds);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D whitePixel, bool debug)
        {
            // Draw the pulse at the current position
            // Lerp between red and white based on DamageRecency (1 = recent damage, 0 = no recent damage)
            Color color = Color.Lerp(Color.White, Color.Red, DamageRecency);
            AnimatedSprite.Draw(spriteBatch, CurrentPosition, color);
            // color = TakingDamage ? Color.Red : Color.White;
            // AnimatedSprite.Draw(spriteBatch, CurrentPosition, color);

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
}