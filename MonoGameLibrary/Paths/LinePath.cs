using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Geometry;

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
        private bool IsAxisAligned
        {
            get
            {
                // Check if the path is axis-aligned (horizontal or vertical)
                return Direction.X == 0 || Direction.Y == 0;
            }
        }

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


        public override bool HasCollided(Hitbox hitbox)
        {
            // For axis-aligned paths, we can use bounding box collision, but this doesn't
            // really work too well by the edges of the rectangle. 
            // RoadSegment.Height x2
            // Create a quadrilateral representing the path's area
            float halfHeight = Constants.PathWidth / 2f; // Half the width of the path
            Vector2 perp = new Vector2(-Direction.Y, Direction.X); // Perpendicular vector
            Vector2 offset = perp * halfHeight;

            Vector2 p1 = StartingPoint + offset;
            Vector2 p2 = StartingPoint - offset;
            Vector2 p3 = EndingPoint - offset;
            Vector2 p4 = EndingPoint + offset;

            Quadrilateral pathQuad = new Quadrilateral(p1, p2, p3, p4);

            return pathQuad.IntersectsCircle(hitbox.circleBox);
        }
    }
}