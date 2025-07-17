using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

namespace MonoGameLibrary.Paths;

class PathSegment
{
    private float Angle { get; set; }
    private Vector2 Position { get; set; }
    private int Length { get; set; }
    private Vector2 Origin { get; set; }

    private Rectangle _rectangle;

    public PathSegment(float angle, Vector2 position, int length)
    {
        Angle = angle;
        Position = position;
        Length = length;
        _rectangle = new Rectangle((int)Position.X, (int)Position.Y - Constants.PathWidth / 2, 2, Constants.PathWidth);

    }

    // public void Draw(SpriteBatch spriteBatch, Texture2D whitePixel, Color color)
    // {
    //     spriteBatch.Draw(
    //         whitePixel,
    //         Position,
    //         _rectangle,
    //         color,
    //         Angle,

    //     )
    // }

}