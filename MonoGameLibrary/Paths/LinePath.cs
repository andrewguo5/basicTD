using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Paths
{
    public class LinePath : Path
    {
        private Sprite RoadSegment = null;
        public override Vector2 StartingPoint { get; set; }
        public override Vector2 EndingPoint
        {
            get
            {
                // The ending point is calculated based on the starting point, direction, and length.
                return StartingPoint + Direction * _length;
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

        private Vector2 Direction { get; set; }

        // Private backing field for Length
        private float _length { get; set; }

        public override float Length { get { return _length; } }

        public LinePath(Vector2 startingPoint, Vector2 endingPoint)
        {
            StartingPoint = startingPoint;

            // Calculate the direction vector and length of the path
            Direction = Vector2.Normalize(endingPoint - StartingPoint);
            _length = Vector2.Distance(StartingPoint, endingPoint);
        }

        public LinePath(Vector2 startingPoint, Vector2 direction, float length, TextureAtlas atlas)
        {
            StartingPoint = startingPoint;
            Direction = Vector2.Normalize(direction);
            _length = length;
        }

        public override void LoadSprites(TextureAtlas atlas)
        {
            RoadSegment = atlas.CreateSprite("road-segment");
            RoadSegment.Origin = new Vector2(0f, RoadSegment.Height / 2f);
            RoadSegment.Scale = new Vector2(Length / RoadSegment.Width, 2f);
            RoadSegment.Rotation = (float)Math.Atan2(Direction.Y, Direction.X);
        }

        public override Vector2 ComputePositionFromDistance(float distance)
        {
            // Ensure the distance is non-negative
            if (distance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distance), "Distance cannot be negative.");
            }

            return StartingPoint + Direction * Math.Clamp(distance, 0.0f, _length);
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
            if (RoadSegment == null)
            {
                return;
            }

            RoadSegment.Draw(spriteBatch, StartingPoint);
        }
    }
}