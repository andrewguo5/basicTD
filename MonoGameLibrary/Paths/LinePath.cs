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
        private Vector2 _startingPoint;
        public override Vector2 Offset { get; set; }
        public override float Scale { get; set; } = 1.0f;
        public override Vector2 StartingPoint => _startingPoint * Scale + Offset;
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

        public override float Length { get { return _length * Scale; } }

        public LinePath(Vector2 startingPoint, Vector2 endingPoint) : this(startingPoint, endingPoint, Vector2.Zero, 1.0f) { }

        public LinePath(Vector2 startingPoint, Vector2 endingPoint, Vector2 offset)
            : this(startingPoint, endingPoint, offset, 1.0f)
        {
        }

        public LinePath(Vector2 startingPoint, Vector2 endingPoint, float scale)
            : this(startingPoint, endingPoint, Vector2.Zero, scale)
        {
        }

        public LinePath(Vector2 startingPoint, Vector2 endingPoint, Vector2 offset, float scale)
        {
            Offset = offset;
            Scale = scale;
            _startingPoint = startingPoint;
            Vector2 _endingPoint = endingPoint * scale + Offset;

            // Calculate the direction vector and length of the path
            Direction = Vector2.Normalize(_endingPoint - StartingPoint);
            _length = Vector2.Distance(startingPoint, endingPoint);
        }

        public override void LoadSprites(TextureAtlas atlas)
        {
            RoadSegment = atlas.CreateSprite("road-segment");
            RoadSegment.Origin = new Vector2(0f, RoadSegment.Height / 2f);
            RoadSegment.Scale = new Vector2(Length / RoadSegment.Width, RoadSegment.Height / TDConstants.PathWidth);
            RoadSegment.Rotation = (float)Math.Atan2(Direction.Y, Direction.X);
        }

        public override Vector2 ComputePositionFromDistance(float distance)
        {
            // Ensure the distance is non-negative
            if (distance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distance), "Distance cannot be negative.");
            }

            return StartingPoint + Direction * Math.Clamp(distance, 0.0f, Length);
        }

        public static LinePath LoadFromXML(XElement _path, Vector2 offset, float scale)
        {
            string startingPointStr = _path.Attribute("startingPoint").Value;
            string endingPointStr = _path.Attribute("endingPoint").Value;

            var startingPointParts = startingPointStr.Split(',');
            var endingPointParts = endingPointStr.Split(',');

            var startingPoint = new Vector2(
                float.Parse(startingPointParts[0]),
                float.Parse(startingPointParts[1])
            );

            var endingPoint = new Vector2(
                float.Parse(endingPointParts[0]),
                float.Parse(endingPointParts[1])
            );

            return new LinePath(startingPoint, endingPoint, offset, scale);
        }

        public static LinePath LoadFromXML(XElement _path)
        {
            return LoadFromXML(_path, Vector2.Zero, 1.0f);
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
            float halfHeight = TDConstants.PathWidth / 2f; // Half the width of the path
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