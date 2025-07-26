using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Geometry;

/// <summary>
/// Represents a generic, non-axis aligned rectangle defined by four points.
/// </summary>
public class Quadrilateral
{
    public Vector2 TopLeft { get; set; }
    public Vector2 TopRight { get; set; }
    public Vector2 BottomRight { get; set; }
    public Vector2 BottomLeft { get; set; }

    public Quadrilateral(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
    }

    /// <summary>
    /// Gets the center point of the rectangle.
    /// </summary>
    public Vector2 Center =>
        (TopLeft + TopRight + BottomRight + BottomLeft) / 4f;

    /// <summary>
    /// Determines if a point is inside the rectangle using the winding number method.
    /// </summary>
    public bool Contains(Vector2 point)
    {
        int windingNumber = 0;
        Vector2[] corners = { TopLeft, TopRight, BottomRight, BottomLeft, TopLeft };
        for (int i = 0; i < 4; i++)
        {
            if (corners[i].Y <= point.Y)
            {
                if (corners[i + 1].Y > point.Y &&
                    IsLeft(corners[i], corners[i + 1], point) > 0)
                    windingNumber++;
            }
            else
            {
                if (corners[i + 1].Y <= point.Y &&
                    IsLeft(corners[i], corners[i + 1], point) < 0)
                    windingNumber--;
            }
        }
        return windingNumber != 0;
    }

    private static float IsLeft(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        return (p1.X - p0.X) * (p2.Y - p0.Y) - (p2.X - p0.X) * (p1.Y - p0.Y);
    }

    /// <summary>
    /// Determines if the quadrilateral intersects with a circle.
    /// </summary>
    /// <param name="center">The center of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <returns>True if the circle intersects the quadrilateral; otherwise, false.</returns>
    public bool IntersectsCircle(Circle circle)
    {
        // Check if circle center is inside the quadrilateral
        if (Contains(circle.Location.ToVector2()))
            return true;

        // Check if any edge of the quadrilateral intersects the circle
        Vector2[] corners = { TopLeft, TopRight, BottomRight, BottomLeft };
        for (int i = 0; i < 4; i++)
        {
            Vector2 a = corners[i];
            Vector2 b = corners[(i + 1) % 4];
            if (circle.IntersectsLineSegment(a, b))
                return true;
        }

        return false;
    }

}
