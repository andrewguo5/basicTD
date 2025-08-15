using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BasicTD.Frontend;

public class Scaffold
{
    private readonly Texture2D _pixel;
    private readonly GraphicsDevice _graphicsDevice;

    public Scaffold(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _pixel = new Texture2D(_graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void DrawHorizontalLine(SpriteBatch spriteBatch, int x, int y, int length, int thickness = 1)
    {
        spriteBatch.Draw(_pixel, new Rectangle(x, y, length, thickness), Color.Red);
    }

    public void DrawVerticalLine(SpriteBatch spriteBatch, int x, int y, int length, int thickness = 1)
    {
        spriteBatch.Draw(_pixel, new Rectangle(x, y, thickness, length), Color.Red);
    }

    public void DrawHorizontalLineAtY(SpriteBatch spriteBatch, int y, int thickness = 1)
    {
        DrawHorizontalLine(spriteBatch, 0, y, _graphicsDevice.Viewport.Width, thickness);
    }

    public void DrawVerticalLineAtX(SpriteBatch spriteBatch, int x, int thickness = 1)
    {
        DrawVerticalLine(spriteBatch, x, 0, _graphicsDevice.Viewport.Height, thickness);
    }

    public void DrawFilledRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        spriteBatch.Draw(_pixel, rectangle, color);
    }
}
