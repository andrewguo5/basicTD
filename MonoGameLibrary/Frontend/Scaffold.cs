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

    public void DrawHorizontalLine(SpriteBatch spriteBatch, int x, int y, int length, int colorOption = 0)
    {
        Color color = colorOption switch
        {
            1 => Color.LimeGreen,
            2 => Color.Cyan,
            _ => Color.Red
        };
        spriteBatch.Draw(_pixel, new Rectangle(x, y, length, 1), color);
    }

    public void DrawVerticalLine(SpriteBatch spriteBatch, int x, int y, int length, int colorOption = 0)
    {
        Color color = colorOption switch
        {
            1 => Color.LimeGreen,
            2 => Color.Cyan,
            _ => Color.Red
        };
        spriteBatch.Draw(_pixel, new Rectangle(x, y, 1, length), color);
    }

    public void DrawHorizontalLineAtY(SpriteBatch spriteBatch, int y, int colorOption = 0)
    {
        DrawHorizontalLine(spriteBatch, 0, y, _graphicsDevice.Viewport.Width, colorOption);
    }

    public void DrawVerticalLineAtX(SpriteBatch spriteBatch, int x, int colorOption = 0)
    {
        DrawVerticalLine(spriteBatch, x, 0, _graphicsDevice.Viewport.Height, colorOption);
    }

    public void DrawFilledRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        spriteBatch.Draw(_pixel, rectangle, color);
    }

    public void DrawRectanglePerimeter(SpriteBatch spriteBatch, Rectangle rectangle, int colorOption = 0)
    {
        // Top
        DrawHorizontalLine(spriteBatch, rectangle.Left, rectangle.Top, rectangle.Width, colorOption);
        // Bottom
        DrawHorizontalLine(spriteBatch, rectangle.Left, rectangle.Bottom - 1, rectangle.Width, colorOption);
        // Left
        DrawVerticalLine(spriteBatch, rectangle.Left, rectangle.Top, rectangle.Height, colorOption);
        // Right
        DrawVerticalLine(spriteBatch, rectangle.Right - 1, rectangle.Top, rectangle.Height, colorOption);
    }
}

