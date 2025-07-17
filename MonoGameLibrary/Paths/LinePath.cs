using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Paths
{
    public class LinePath : Path
    {
        /// <summary>
        /// Represents the starting point of the path relative to Origin
        /// </summary>
        public override Vector2 StartingPoint { get; set; }

        /// <summary>
        /// Represents the ending point of the path relative to Origin
        /// </summary>
        public override Vector2 EndingPoint
        {
            get
            {
                // The ending point is calculated based on the starting point, direction, and length.
                return StartingPoint + _direction * _length;
            }
        }

        public override List<Vector2> ControlPoints
        {
            get
            {
                // For a line path, the control points are just the starting and ending points.
                return new List<Vector2>();
            }
        }

        /// <summary>
        /// Represents the direction of the path from StartingPoint to EndingPoint as
        /// a normalized vector.
        /// </summary>
        private Vector2 _direction { get; set; }

        /// <summary>
        /// Represents the length of the path from StartingPoint to EndingPoint.
        /// </summary>
        /// <remarks>
        /// If the StartingPoint and EndingPoint are the same, the length is zero.
        /// </remarksj>
        /// <returns>The length of the path.</returns>
        private float _length { get; set; }

        public override float Length { get { return _length; } }

        /// <summary>
        /// Constructs a new LinePath with the specified starting and ending points.
        /// </summary>
        /// <param name="startingPoint">The starting point of this path.</param>
        /// <param name="endingPoint">The ending point of this path.</param>
        public LinePath(Vector2 startingPoint, Vector2 endingPoint)
        {
            StartingPoint = startingPoint;

            // Calculate the direction vector and length of the path
            _direction = Vector2.Normalize(endingPoint - StartingPoint);
            _length = Vector2.Distance(StartingPoint, endingPoint);
        }

        /// <summary>
        /// Constructs a new LinePath with the specified starting point, direction, and length.
        /// </summary>
        /// <param name="startingPoint">The starting point of this path.</param>
        /// <param name="direction">The direction of the path as a normalized vector.</param>    
        public LinePath(Vector2 startingPoint, Vector2 direction, float length)
        {
            StartingPoint = startingPoint;
            _direction = Vector2.Normalize(direction);
            _length = length;
        }

        /// <summary>
        /// Computes the position along the path given a distance from the starting point.
        /// The distance is clamped to the length of the path.
        /// </summary>
        /// <param name="distance">The distance along the path to find the position for.</param>
        /// <returns>A vector indicating the position of the given distance along the path.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Distance cannot be negative.</exception>
        public override Vector2 ComputePositionFromDistance(float distance)
        {
            // Ensure the distance is non-negative
            if (distance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distance), "Distance cannot be negative.");
            }

            return StartingPoint + _direction * Math.Clamp(distance, 0.0f, _length);
        }

        public static LinePath LoadFromXML(XElement _path, Vector2 origin)
        {
            string startingPointStr = _path.Attribute("startingPoint").Value;
            string endingPointStr = _path.Attribute("endingPoint").Value;

            var startingPointParts = startingPointStr.Split(',');
            var endingPointParts = endingPointStr.Split(',');

            var startingPoint = new Vector2(
                float.Parse(startingPointParts[0]),
                float.Parse(startingPointParts[1])
            ) + origin;

            var endingPoint = new Vector2(
                float.Parse(endingPointParts[0]),
                float.Parse(endingPointParts[1])
            ) + origin;

            return new LinePath(startingPoint, endingPoint);
        }

        public static LinePath LoadFromXML(XElement _path)
        {
            return LoadFromXML(_path, Vector2.Zero);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            Vector2 start = StartingPoint;
            Vector2 end = EndingPoint;
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();
            float width = 20f;
            Vector2 offsetStart = start;// + new Vector2(width * 0.5f, 0);

            spriteBatch.Draw(
                pixel,                      // texture
                offsetStart,                // position
                null,                       // sourceRectangle
                Color.Wheat,                // color
                angle,                      // rotation
                Vector2.Zero,               // origin
                new Vector2(length, width), // scale
                SpriteEffects.None,         // effects
                0f                          // layerDepth
            );
        }
    }
}