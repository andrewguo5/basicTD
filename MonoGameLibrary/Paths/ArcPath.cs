using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Geometry;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Collision;

namespace MonoGameLibrary.Paths
{
    public class ArcPath : Path
    {
        private Sprite RoadSegment = null;
        public override Vector2 StartingPoint { get; set; }

        public override Vector2 EndingPoint
        {
            get
            {
                float endAngle = InitialAngle + ArcAngle;
                return AngleToPoint(endAngle);
            }
        }

        public override List<Vector2> ControlPoints
        {
            get
            {
                return new List<Vector2> { CenterPoint };
            }
        }
        public override float Length
        {
            get
            {
                return Radius * Math.Abs(ArcAngle);
            }
        }

        private Vector2 CenterPoint { get; set; }

        private float Radius
        {
            get
            {
                return Vector2.Distance(CenterPoint, StartingPoint);
            }
        }

        private float InitialAngle
        {
            get
            {
                return ComputeAngleFromVector(StartingPoint);
            }
        }

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

        public override void LoadSprites(TextureAtlas atlas)
        {
            // Optional: Implement sprite loading if needed
            RoadSegment = atlas.CreateSprite("road-segment");
            RoadSegment.Origin = new Vector2(0f, RoadSegment.Height / 2f);
            RoadSegment.Scale = new Vector2(MathHelper.TwoPi * Radius / 100f, 2f);
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
            if (RoadSegment == null)
            {
                return;
            }

            const int segments = 100;
            float step = Length / segments;
            Vector2 prevPoint = StartingPoint;

            for (int i = 1; i <= segments; i++)
            {
                float distance = i * step;
                Vector2 nextPoint = ComputePositionFromDistance(distance);
                Vector2 direction = nextPoint - prevPoint;
                float angle = (float)Math.Atan2(direction.Y, direction.X);

                RoadSegment.Draw(
                    spriteBatch, prevPoint, angle);

                prevPoint = nextPoint;
            }
        }

        public override bool HasCollided(Hitbox hitbox)
        {
            Circle innerCircle = new Circle(CenterPoint.ToPoint(), (int)(Radius - Constants.PathWidth * 1.5f));
            Circle outerCircle = new Circle(CenterPoint.ToPoint(), (int)(Radius + Constants.PathWidth / 2));

            bool circleCheck = outerCircle.Intersects(hitbox.circleBox) && !innerCircle.Intersects(hitbox.circleBox);

            // Compute the angle from the center to the hitbox center
            float hitboxAngle = ComputeAngleFromVector(hitbox.circleBox.Location.ToVector2());

            // Normalize angles to [0, 2π)
            float startAngle = InitialAngle;
            float endAngle = InitialAngle + ArcAngle;

            // Ensure startAngle < endAngle for comparison
            if (ArcAngle < 0)
            {
                float temp = startAngle;
                startAngle = endAngle;
                endAngle = temp;
            }

            // Normalize hitboxAngle to [0, 2π)
            hitboxAngle = MathHelper.WrapAngle(hitboxAngle);
            startAngle = MathHelper.WrapAngle(startAngle);
            endAngle = MathHelper.WrapAngle(endAngle);

            // Check if hitboxAngle is within the arc
            bool angleCheck;
            if (startAngle <= endAngle)
                angleCheck = hitboxAngle >= startAngle && hitboxAngle <= endAngle;
            else
                angleCheck = hitboxAngle >= startAngle || hitboxAngle <= endAngle;

            return circleCheck && angleCheck;
        }
    }
}