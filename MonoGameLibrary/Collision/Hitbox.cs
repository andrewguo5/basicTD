using Microsoft.Xna.Framework;
using MonoGameLibrary.Geometry;

namespace MonoGameLibrary.Collision;

public class Hitbox
{
    public Circle circleBox;
    public Rectangle rectangleBox => new Rectangle(
        (int)(circleBox.Location.X - circleBox.Radius),
        (int)(circleBox.Location.Y - circleBox.Radius),
        (int)(circleBox.Radius * 2),
        (int)(circleBox.Radius * 2)
    );

    public Hitbox(Vector2 location, int radius)
    {
        circleBox = new Circle(location.ToPoint(), radius);
    }

}