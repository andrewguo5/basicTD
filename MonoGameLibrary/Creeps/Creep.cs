using MonoGameLibrary.Paths;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Creeps
{
    public class Creep
    {
        public Path Path;

        private float CurrentDistance;

        private Vector2 CurrentPosition;

        private float RemainingDistance => Path.Length - CurrentDistance;

        private float Speed;

        private AnimatedSprite AnimatedSprite;

        public Creep(Path path, float speed, AnimatedSprite animatedSprite)
        {
            Path = path;
            CurrentPosition = path.StartingPoint;
            CurrentDistance = 0.0f;
            Speed = speed;
            AnimatedSprite = animatedSprite;
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
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D whitePixel, bool debug)
        {
            // Draw the pulse at the current position
            AnimatedSprite.Draw(spriteBatch, CurrentPosition);

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