using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Paths
{
    public class ArcPath : Path
    {
        public override Vector2 StartingPoint { get; set; }

        public override Vector2 EndingPoint
        {
            get
            {
                // The ending point is calculated based on the center, radius, and arc angle.
                float endAngle = InitialAngle + ArcAngle;
                return AngleToPoint(endAngle);
            }
        }

        public override List<Vector2> ControlPoints
        {
            get
            {
                // For an arc, the control points are the starting point and center point.
                return new List<Vector2> { CenterPoint };
            }
        }
        public override float Length
        {
            get
            {
                // The length of the arc is given by the formula: radius * angle
                return Radius * Math.Abs(ArcAngle);
            }
        }

        /// <summary>
        /// The center of the Circle which defines the arc.
        /// </summary>
        private Vector2 CenterPoint { get; set; }

        /// <summary>
        /// Represents the Radius of the arc.
        /// </summary>
        private float Radius
        {
            get
            {
                // The radius is the distance from the center point to the starting point.
                return Vector2.Distance(CenterPoint, StartingPoint);
            }
        }

        private float InitialAngle
        {
            get
            {
                // The initial angle is the angle from the center to the starting point.
                return ComputeAngleFromVector(StartingPoint);
            }
        }

        /// <summary>
        /// Represents the angle of the arc in radians.
        /// A full circle is 2 * Math.PI radians.
        /// </summary>
        public float ArcAngle { get; set; }

        private float Direction
        {
            get
            {
                // The direction of the arc is determined by the sign of the arc angle.
                return Math.Sign(ArcAngle);
            }
        }


        public ArcPath(Vector2 startingPoint, Vector2 centerPoint, float arcAngle)
        {
            StartingPoint = startingPoint;
            CenterPoint = centerPoint;
            ArcAngle = arcAngle;
        }

        public override Vector2 ComputePositionFromDistance(float distance)
        {
            if (distance < 0)
                throw new ArgumentOutOfRangeException(nameof(distance), "Distance must be non-negative.");

            // If the distance exceeds the length of the arc, return the ending point
            if (distance >= Length)
                return EndingPoint;

            // Calculate t as the ratio of the distance to the length of the arc
            float t = distance / Length;

            // Calculate the angle at that t value
            float angle = ComputeAngleFromT(t);

            // Return the point on the arc at that angle
            return AngleToPoint(angle);
        }

        public Vector2 AngleToPoint(float angle)
        {
            // Convert the angle to a point on the arc
            float x = CenterPoint.X + Radius * (float)Math.Cos(angle);
            float y = CenterPoint.Y + Radius * (float)Math.Sin(angle);
            return new Vector2(x, y);
        }

        public float ComputeAngleFromT(float t)
        {
            if (t < 0 || t > 1)
                throw new ArgumentOutOfRangeException(nameof(t), "t must be in the range [0, 1].");

            // Calculate the angle at the given t value
            return InitialAngle + t * ArcAngle;
        }

        public float ComputeAngleFromVector(Vector2 vector)
        {
            // Calculate the angle from the center to the vector
            float deltaY = vector.Y - CenterPoint.Y;
            float deltaX = vector.X - CenterPoint.X;
            return (float)Math.Atan2(deltaY, deltaX);
        }

        public static ArcPath LoadFromXML(XElement _path, Vector2 origin)
        {
            string startingPointStr = _path.Attribute("startingPoint").Value;
            string centerPointStr = _path.Attribute("centerPoint").Value;
            float arcAngleDegrees = float.Parse(_path.Attribute("arcAngle").Value);
            float arcAngle = MathHelper.ToRadians(arcAngleDegrees);

            var startingPointParts = startingPointStr.Split(',');
            var centerPointParts = centerPointStr.Split(',');

            var startingPoint = new Vector2(
                float.Parse(startingPointParts[0]),
                float.Parse(startingPointParts[1])
            ) + origin;

            var centerPoint = new Vector2(
                float.Parse(centerPointParts[0]),
                float.Parse(centerPointParts[1])
            ) + origin;

            return new ArcPath(startingPoint, centerPoint, arcAngle);
        }
        public static ArcPath LoadFromXML(XElement _path)
        {
            return LoadFromXML(_path, Vector2.Zero);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            const int segments = 100;
            float step = Length / segments;
            Vector2 prevPoint = StartingPoint;

            for (int i = 1; i <= segments; i++)
            {
                float distance = i * step;
                Vector2 nextPoint = ComputePositionFromDistance(distance);
                Vector2 direction = nextPoint - prevPoint;
                float angle = (float)Math.Atan2(direction.Y, direction.X);
                float length = direction.Length() + 2f;
                float width = 20f;
                Vector2 offsetStart = prevPoint;// + new Vector2(width * 0.5f, 0);

                spriteBatch.Draw(
                    pixel,
                    offsetStart,
                    null,
                    Color.Wheat,
                    angle,
                    Vector2.Zero,
                    new Vector2(length, width),
                    SpriteEffects.None,
                    0f
                );

                prevPoint = nextPoint;
            }
        }
    }
}