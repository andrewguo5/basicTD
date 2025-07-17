using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary;

public readonly struct Circle : IEquatable<Circle>
{
    private static readonly Circle s_empty = new Circle();

    public readonly int X;

    public readonly int Y;

    public readonly int Radius;

    public readonly Point Location => new Point(X, Y);

    public static Circle Empty => s_empty;

    public readonly bool IsEmpty => X == 0 && Y == 0 && Radius == 0;

    public readonly int Top => Y - Radius;

    public readonly int Bottom => Y + Radius;

    public readonly int Left => X - Radius;

    public readonly int Right => X + Radius;

    public Circle(int x, int y, int radius)
    {
        X = x;
        Y = y;
        Radius = radius;
    }

    public Circle(Point location, int radius)
    {
        X = location.X;
        Y = location.Y;
        Radius = radius;
    }

    /// <summary>
    /// Detects if this circle intersects with another circle. 
    /// Math summary: If the distance between the two circle's centers is less
    /// than the sum of the circle's radii, then the two circles overlap.
    /// </summary>
    /// <param name="other">The other circle to check intersection with</param>
    /// <returns>true if the two circles overlap, otherwise false.</returns>
    public bool Intersects(Circle other)
    {
        int radiiSquared = (this.Radius + other.Radius) * (this.Radius + other.Radius);
        float distanceSquared = Vector2.DistanceSquared(this.Location.ToVector2(), other.Location.ToVector2());
        return distanceSquared < radiiSquared;
    }

    // IEquatable interface: Equals, GetHashCode, ==, !=
    /// <summary>
    /// Checks for equality for generic objects by checking first whether it is a circle
    /// </summary>
    /// <param name="obj">Another object.</param>
    /// <returns>true if the other object is a circle and is the same circle, else false.</returns>
    public override readonly bool Equals(object obj) => obj is Circle other && Equals(other);

    /// <summary>
    /// Checks equality between circles. Circles are equal if they have the same center and radius.
    /// </summary>
    /// <param name="other">Another circle.</param>
    /// <returns>true if the two circles are the same, else false.</returns>
    public readonly bool Equals(Circle other) => this.X == other.X && this.Y == other.Y && this.Radius == other.Radius;

    /// <summary>
    /// Hashes this circle.
    /// </summary>
    /// <returns>The hash code for this circle as a 32-bit signed integer.</returns>
    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Radius);

    public static bool operator ==(Circle lhs, Circle rhs) => lhs.Equals(rhs);

    public static bool operator !=(Circle lhs, Circle rhs) => !lhs.Equals(rhs);
}