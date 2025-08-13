using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Paths
{
    public abstract class Path
    {
        public abstract Vector2 Offset { get; set; }
        public abstract float Scale { get; set; }

        /// <summary>
        /// Represents the starting point of the path.
        /// </summary>
        public abstract Vector2 StartingPoint { get; }

        /// <summary>
        /// The end point of the path is always computed based on 
        /// the starting point and the path's parameters.
        /// </summary>
        public abstract Vector2 EndingPoint { get; }

        /// <summary>
        /// A list of relevant control points for the path, if any.
        /// Useful for visualizing the path for debug purposes.
        /// </summary>
        public abstract List<Vector2> ControlPoints { get; }

        /// <summary>
        /// Returns the full length of the path.
        /// </summary>
        public abstract float Length { get; }

        public abstract void LoadSprites(TextureAtlas atlas);

        /// <summary>
        /// Computes the position along the path given a distance from the starting point.
        /// The distance is clamped to the length of the path.
        /// </summary>
        /// <param name="distance">The distance along the path to find the position for.</param>
        /// <returns>A vector indicating the position of the given distance along the path.</returns>
        public abstract Vector2 ComputePositionFromDistance(float distance);

        /// <summary>
        /// Draws the path using the provided SpriteBatch and pixel texture.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="pixel">A 1x1 white pixel texture used for drawing lines.</param>
        public abstract void Draw(SpriteBatch spriteBatch, Texture2D pixel);

        public virtual bool HasCollided(Hitbox hitbox) { return false; }
    }
}